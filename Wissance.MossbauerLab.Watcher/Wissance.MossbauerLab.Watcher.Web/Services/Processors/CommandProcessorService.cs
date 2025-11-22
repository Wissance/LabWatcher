using System;
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
    /// <summary>
    /// CommandProcessorService is a singleton service that working all time app is working
    /// </summary>
    public class CommandProcessorService : IHostedService, IDisposable
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
                    UpdateType.ChatMember
                },
                // true = Ignore messages that were sent during App was offline
                ThrowPendingUpdates = true, 
            };
            _cancellationTokenSource = new CancellationTokenSource();
            _config = config;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<CommandProcessorService>();
            // _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, _cancellationTokenSource.Token);
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("CommandProcessor \"Start\" begin");
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, _cancellationTokenSource.Token);
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    break;
                }
            }
            _logger.LogDebug("CommandProcessor \"Start\" end");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("CommandProcessor \"Stop\" begin");
            // probably code from dispose must be placed here ...
            _logger.LogDebug("CommandProcessor \"Stop\" end");
        }

        public void Dispose()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource.Cancel();
            Task closeTask = _botClient.CloseAsync();
            closeTask.Wait();
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
                switch (update.Type)
                {
                    case UpdateType.Message:
                        // todo(UMV) : allow messages for only the chat members
                        Message rawMessage = update.Message;
                        bool shouldProcess = await ShouldProcessCommand(rawMessage);
                        if (!shouldProcess)
                        {
                            await _botClient.SendTextMessageAsync(rawMessage.Chat.Id, CommandAnswerLocalizationDefs.UserOperationIsNotPermitted);
                            break;
                        }

                        // rawMessage.From.Username
                        if (rawMessage != null && rawMessage.Text != null)
                        {
                            string trimmedMessage = rawMessage.Text.Trim(new[] {' '});
                            string[] messageParts = trimmedMessage.Split(new char[] {' '});
                            // 0 is cmd 
                            if (messageParts.Length < 1)
                            {
                                _logger.LogError($"An error occurred during command detecting: number of parts can't be less then 1, message text: \"{rawMessage.Text}\"");
                            }

                            string[] parameters = messageParts.Skip(1).Select(p => p).ToArray();
                            CommandContext context = CreateContext(messageParts[0], rawMessage);
                            ICommand command = CommandFactory.Create(context);
                            if (command == null)
                            {
                                await _botClient.SendTextMessageAsync(rawMessage.Chat.Id, "Пока не реализовано, в процессе разработки", 
                                    cancellationToken: _cancellationTokenSource.Token);
                                return;
                            }

                            await command.ExecuteAsync(parameters);
                        }
                        break;
                    case UpdateType.CallbackQuery:
                        Message callbackMessage = update.Message;
                        bool shouldProcessCallback = await ShouldProcessCommand(callbackMessage);
                        if (!shouldProcessCallback)
                        {
                            await _botClient.SendTextMessageAsync(callbackMessage.Chat.Id, CommandAnswerLocalizationDefs.UserOperationIsNotPermitted);
                            break;
                        }
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

        private CommandContext CreateContext(string command, Message rawMessage)
        {
            return new CommandContext(command, _botClient, _modelContext, _fileStore, rawMessage, _config,
                _cancellationTokenSource.Token, _loggerFactory);
        }
        
        private readonly ModelContext _modelContext;
        private readonly IFileStoreService _fileStore;
        private readonly ReceiverOptions _receiverOptions;
        private readonly ITelegramBotClient _botClient;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<CommandProcessorService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ApplicationConfig _config;
    }
}