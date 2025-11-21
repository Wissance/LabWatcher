using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Telegram.Bot;
using Wissance.MossbauerLab.Watcher.Web.Data;
using Wissance.MossbauerLab.Watcher.Web.Services.State;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public class CheckStateCommand : ICommand
    {
        public CheckStateCommand(CommandContext context)
        {
            _context = context;
            _logger = context.LoggerFactory.CreateLogger<CheckStateCommand>();
        }

        public async Task<bool> ExecuteAsync(string[] parameters)
        {
            try
            {
                /* checking here :
                 *   1. Shared folder is not empty 2
                 *   2. MossbauerComputer is responding to ping
                 *   3. NAS is responding to ping
                 *   4. DB exists and OK
                 * */
                StateCheckerService checker = new StateCheckerService(_context.Context, _context.FileStore,
                                                                      _context.Config, _context.LoggerFactory);
                SpectrometryLabState state = await checker.CheckStateAsync();
                StringBuilder sb = new StringBuilder();
                sb.Append(CommandAnswerLocalizationDefs.SpectrometryLabState + Environment.NewLine);
                foreach (ControllingItem controllingItem in state.StateItems)
                {
                    sb.Append($"* {controllingItem} : {controllingItem.StateStr} {Environment.NewLine}");
                }

                await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, sb.ToString());
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during the handling {_context.Command} command, {e.Message}");
                return false;
            }
        }
        
        private readonly CommandContext _context;
        private readonly ILogger<CheckStateCommand> _logger;
    }
}