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


        private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            
        }

        private async Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
        }

        // private readonly TelegramSendRequisites _tgRequisites;
        private readonly ReceiverOptions _receiverOptions;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<CommandProcessorService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;
    }
}