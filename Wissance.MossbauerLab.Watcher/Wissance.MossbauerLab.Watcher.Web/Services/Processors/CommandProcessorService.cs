using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;
using Wissance.MossbauerLab.Watcher.Common.Utils.Telegram;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Services.Store;
using Wissance.MossbauerLab.Watcher.Web.Command;
using Wissance.MossbauerLab.Watcher.Web.Config;

namespace Wissance.MossbauerLab.Watcher.Web.Services.Processors
{
    internal class IncompleteMessage
    {
        public string Command { get; set; }
        public DateTimeOffset Time { get; set; }
    }

    /// <summary>
    /// CommandProcessorService is a singleton service that working all time app is working
    /// </summary>
    public class CommandProcessorService : BackgroundService
        //IHostedService, IDisposable
    {
        public CommandProcessorService(ModelContext modelContext, IFileStoreService fileStore, ApplicationConfig config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _modelContext = modelContext;
            _fileStore = fileStore;
            _botClient = new TelegramBotClient(_config.NotificationSettings.TelegramSettings.BotKey);
            // UpdateTypes must be limited by 
            _receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = new UpdateType[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery,
                    UpdateType.ChatMember
                },
                // true = Ignore messages that were sent during App was offline
                ThrowPendingUpdates = true, 
            };
            _cancellationTokenSource = new CancellationTokenSource();
            _config = config;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<CommandProcessorService>();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("CommandProcessor \"Execute\" begin");
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, _cancellationTokenSource.Token);
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    await _botClient.CloseAsync();
                    break;
                }
            }
            _logger.LogDebug("CommandProcessor \"Start\" end");
        }
        
        /// <summary>
        ///    Updates handling from TgBot. This function could handle multiple events, however we are plan to handling
        ///    messages here, there are following messages:
        ///    1. /start for interactive mode start , responses with greeting and command list like /help
        ///    2. /help for view message types
        ///    3. /listSpectra for return all spectra, equivalent to GET ~/api/spectrum
        ///    4. /getSpectrumInfo {spectrum_id} return spectrum state, measure date and files list (like GET ~/api/Spectrum/{id}/samples)
        ///    5. /getSpectrumFiles {id} {from} {to} {where} return zip with files
        ///    6. /checkState returns current state
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                Message rawMessage = null;
                Tuple<bool, string, string[]> commandParams = null;
                switch (update.Type)
                {
                    case UpdateType.Message:
                        // todo(UMV) : allow messages for only the chat members
                        rawMessage = update.Message;
                        bool shouldProcess = await ShouldProcessCommand(rawMessage);
                        if (!shouldProcess)
                        {
                            await _botClient.SendTextMessageAsync(rawMessage.Chat.Id, CommandAnswerLocalizationDefs.UserOperationIsNotPermitted);
                            break;
                        }
                        
                        commandParams = GetCommandParams(rawMessage);
                        
                        if (_incompleteMessages.ContainsKey(rawMessage.From.Id))
                        {
                            // Item2 int (/getSpectrumInfo, /getSpectrumFiles) 
                            bool isIncompleteCommand = int.TryParse(commandParams.Item2, out int spectrumId);
                            if (isIncompleteCommand)
                            {
                                string command = _incompleteMessages[rawMessage.From.Id].Command;
                                _incompleteMessages.Remove(rawMessage.From.Id);
                                List<string> parameters = new List<string>() {commandParams.Item2};
                                if (commandParams.Item3.Any())
                                {
                                    parameters.AddRange(commandParams.Item3);
                                }

                                commandParams = new Tuple<bool, string, string[]>(true, command, parameters.ToArray());
                            }

                        }

                        if (!commandParams.Item1)
                            break;
                        await BuildAndExecuteCmd(rawMessage, commandParams.Item2, commandParams.Item3);
                        
                        break;
                    case UpdateType.CallbackQuery:
                        rawMessage = update.CallbackQuery.Message;
                        rawMessage.Text = update.CallbackQuery.Data;
                        rawMessage.From = update.CallbackQuery.From;
                        bool shouldProcessCallback = await ShouldProcessCommand(rawMessage);
                        if (!shouldProcessCallback)
                        {
                            await _botClient.SendTextMessageAsync(rawMessage.Chat.Id, CommandAnswerLocalizationDefs.UserOperationIsNotPermitted);
                            break;
                        }
                        commandParams = GetCommandParams(rawMessage);
                        if (!commandParams.Item1)
                            break;
                        // todo(UMV):ProcessIncompleteCmd
                        if (commandParams.Item2 == CommandAnswerLocalizationDefs.GetSpectrumInfoCmd ||
                            commandParams.Item2 == CommandAnswerLocalizationDefs.GetSpectrumFilesCmd)
                        {
                            _incompleteMessages[rawMessage.From.Id] = new IncompleteMessage()
                            {
                                Command = commandParams.Item2,
                                Time = DateTimeOffset.UtcNow
                            };
                            await _botClient.SendTextMessageAsync(rawMessage.Chat.Id, CommandAnswerLocalizationDefs.RequestRequiredParameters);
                            break;
                        }

                        await BuildAndExecuteCmd(rawMessage, commandParams.Item2, commandParams.Item3);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during Telegram update handle, error: {e.Message}");
            }
        }

        private async Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            _logger.LogError($"An error occurred during \"CommandProcessorService\": {error.Message}");
            await Task.Delay(10, cancellationToken);
        }

        private async Task<bool> ShouldProcessCommand(Message message)
        {
            if (message.From == null)
                return false;
            long messageSenderId = message.From.Id;
            try
            {
                ChatMember member = await _botClient.GetChatMemberAsync(ChatIdBuilder.Build(_config.NotificationSettings.TelegramSettings),
                    messageSenderId, _cancellationTokenSource.Token);
                return member.Status == ChatMemberStatus.Member || member.Status == ChatMemberStatus.Administrator ||
                       member.Status == ChatMemberStatus.Creator || member.Status == ChatMemberStatus.Restricted;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        
        private Tuple<bool, string, string[]> GetCommandParams(Message message)
        {
            if (message == null || message.Text == null)
                return  new Tuple<bool, string, string[]>(false, String.Empty, new string[] { });
            string trimmedMessage = message.Text.Trim(new[] {' '});
            string[] messageParts = trimmedMessage.Split(new char[] {' '});
            // 0 is cmd 
            if (messageParts.Length < 1)
            {
                _logger.LogError($"An error occurred during command detecting: number of parts can't be less then 1, message text: \"{message.Text}\"");
                return new Tuple<bool, string, string[]>(false, String.Empty, new string[] { });
            }

            string[] parameters = messageParts.Skip(1).Select(p => p).ToArray();
            return new Tuple<bool, string, string[]>(true, messageParts[0], parameters);
        }

        private CommandContext CreateContext(string command, Message rawMessage)
        {
            return new CommandContext(command, _botClient, _modelContext, _fileStore, rawMessage, _config,
                _cancellationTokenSource.Token, _loggerFactory);
        }

        private async Task BuildAndExecuteCmd(Message message, string cmd, string[] parameters)
        {
            CommandContext context = CreateContext(cmd, message);
            ICommand command = CommandFactory.Create(context);
            if (command == null)
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, CommandAnswerLocalizationDefs.NotSupportedYetDevelopmentInProgress, 
                    cancellationToken: _cancellationTokenSource.Token);
                return;
            }

            await command.ExecuteAsync(parameters);
        }

        private readonly ModelContext _modelContext;
        private readonly IFileStoreService _fileStore;
        private readonly ReceiverOptions _receiverOptions;
        private readonly ITelegramBotClient _botClient;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<CommandProcessorService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ApplicationConfig _config;
        private readonly IDictionary<long, IncompleteMessage> _incompleteMessages = new ConcurrentDictionary<long, IncompleteMessage>();
    }
}