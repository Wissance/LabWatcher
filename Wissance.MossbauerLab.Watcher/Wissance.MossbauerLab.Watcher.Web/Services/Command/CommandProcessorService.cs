using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;
using Wissance.MossbauerLab.Watcher.Data;

namespace Wissance.MossbauerLab.Watcher.Web.Services.Command
{
    /// <summary>
    /// CommandProcessorService is a singleton service that working all time app is working
    /// </summary>
    public class CommandProcessorService : IDisposable
    {
        public CommandProcessorService(ModelContext modelContext, TelegramSendRequisites tgRequisites, ILoggerFactory loggerFactory)
        {
            _botClient = new TelegramBotClient(tgRequisites.BotKey);
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
            _logger = loggerFactory.CreateLogger<CommandProcessorService>();
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, _cancellationTokenSource.Token);
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
        ///    3. /get-spectra-list for return all spectra, equivalent to GET ~/api/spectrum
        ///    4. /get-spectrum-info {spectrum_id} return spectrum state, measure date and files list (like GET ~/api/Spectrum/{id}/samples)
        ///    5. /get-spectrum-files {from} {to} {where} return zip with files
        ///    6. /check-state returns current state
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
                        }

                        break;
                    
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during Telegram update handle, error: {e.Message}");
            }
        }

        private string OnStartCmdHandle(string[] args)
        {
            return null;
        }
        
        private string OnHelpCmdHandle(string[] args)
        {
            return null;
        }

        private async Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
        }

        private const string StartCmd = "/start";
        private const string HelpCmd = "/help";
        
        // private readonly TelegramSendRequisites _tgRequisites;
        private readonly ReceiverOptions _receiverOptions;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<CommandProcessorService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;
    }
}