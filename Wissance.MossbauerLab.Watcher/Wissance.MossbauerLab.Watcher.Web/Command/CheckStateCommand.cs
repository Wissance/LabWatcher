using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
                throw new NotImplementedException();
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