using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Wissance.MossbauerLab.Watcher.Dto;
using Wissance.MossbauerLab.Watcher.Web.Managers;
using Wissance.MossbauerLab.Watcher.Web.Services.Store;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public class ListSpectraCommand : ICommand
    {
        public ListSpectraCommand(CommandContext context)
        {
            _context = context;
            _logger = _context.LoggerFactory.CreateLogger<ListSpectraCommand>();
        }
        
        
        public async Task<bool> ExecuteAsync(string[] parameters)
        {
            try
            {
                SpectrumManager manager = new SpectrumManager(_context.Context, _context.LoggerFactory, 
                    new WindowsShareStoreService(_context.Config.Sm2201SpectraStoreSettings, _context.LoggerFactory), _context.Config);
                OperationResultDto<Tuple<IList<SpectrumInfoDto>,long>> result = await manager.GetAsync(1, 100000, 
                    new SortOption("MeasureStartDate", "desc"));
                int pages = (int)Math.Ceiling((double) result.Data.Item2 / 20);
                if (pages == 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("```");
                    sb.Append($"  id   имя   начало измерений   окончание измерений   статус{Environment.NewLine}");
                    sb.Append($"   нет данных{Environment.NewLine}");
                    sb.Append("```");
                }

                for (int i = 0; i < pages; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("```");
                    sb.Append($"  страница №  *{i}*{Environment.NewLine}");
                    sb.Append($"  id   имя   начало измерений   окончание измерений   статус{Environment.NewLine}");
                    if (!result.Success)
                    {
                        await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, CommandAnswerLocalizationDefs.UnknownError);
                        return false;
                    }

                    foreach (SpectrumInfoDto spectrum in result.Data.Item1)
                    {
                        sb.Append($"  {spectrum.Id}    {spectrum.Name} {spectrum.MeasureStartDate:yyyy-MM-dd HH:mm:ss} {spectrum.Last:yyyy-MM-dd HH:mm:ss}   архивный");
                    }

                    sb.Append("```");
                    await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, sb.ToString(), ParseMode.Markdown);
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during the handling {_context.Command} command, {e.Message}");
                return false;
            }
        }

        private readonly CommandContext _context;
        private readonly ILogger<ListSpectraCommand> _logger;
    }
}