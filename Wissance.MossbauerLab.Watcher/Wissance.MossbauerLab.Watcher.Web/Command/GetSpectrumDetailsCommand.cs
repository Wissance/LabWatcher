using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Wissance.MossbauerLab.Watcher.Dto;
using Wissance.MossbauerLab.Watcher.Web.Managers;
using Wissance.MossbauerLab.Watcher.Web.Services.Store;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public class GetSpectrumDetailsCommand : ICommand
    {
        public GetSpectrumDetailsCommand(CommandContext context)
        {
            _context = context;
            _logger = _context.LoggerFactory.CreateLogger<GetSpectrumDetailsCommand>();
        }

        public async Task<bool> ExecuteAsync(string[] parameters)
        {
            try
            {
                if (parameters.Length < 1)
                {
                    await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id,
                        CommandAnswerLocalizationDefs.SpectrumIdNotProvidedError);
                    return false;
                }

                SpectrumManager manager = new SpectrumManager(_context.Context, _context.LoggerFactory, 
                    new WindowsShareStoreService(_context.Config.Sm2201SpectraStoreSettings, _context.LoggerFactory), _context.Config);
                int spectrumId = -1;
                bool parseRes = Int32.TryParse(parameters[0], out spectrumId);
                if (!parseRes)
                {
                    _logger.LogError($"Provided as id value \"{parameters[0]}\" can't be parsed to int");
                    await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id,
                        CommandAnswerLocalizationDefs.SpectrumIdCantBeExtractedError);
                    return false;
                }

                OperationResultDto <SpectrumInfoDto> spectrum = await manager.GetByIdAsync(spectrumId);
                if (!spectrum.Success)
                {
                    _logger.LogError($"Error occurred during getting Spectrum by id in \"GetSpectrumDetailsCommand\" : {spectrum.Message}");
                    await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, CommandAnswerLocalizationDefs.UnknownError);
                    return false;
                }
                
                OperationResultDto<SpectrumSamplesInfoDto> spectrumSamples = await manager.GetSpectrumSamplesAsync(spectrumId);
                if (!spectrumSamples.Success)
                {
                    _logger.LogError($"Error occurred during getting Spectrum samples in \"GetSpectrumDetailsCommand\" : {spectrumSamples.Message}");
                    await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, CommandAnswerLocalizationDefs.UnknownError);
                    return false;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("```");
                sb.Append(string.Format(AnswerTemplate, spectrumId, spectrum.Data.Name, 
                    1,
                    spectrum.Data.MeasureStartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    spectrum.Data.First.HasValue ? spectrum.Data.First?.ToString("yyyy-MM-dd HH:mm:ss") : "",
                    spectrum.Data.Last.HasValue ? spectrum.Data.Last?.ToString("yyyy-MM-dd HH:mm:ss") : "",
                    CommandAnswerLocalizationDefs.GetArchivedCaption(spectrum.Data.IsArchived)
                    ));
                int n = 1;
                foreach (string sample in spectrumSamples.Data.Samples)
                {
                    sb.Append($"  - {n}. {sample}");
                    n++;
                }
                sb.Append("```");
                await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, sb.ToString(),
                    ParseMode.Markdown);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during the handling {_context.Command} command, {e.Message}");
                return false;
            }
        }

        private const string AnswerTemplate = "* id спетра : {0}\n   " +
                                              "* имя спектра : *{1}*\n   " +
                                              "* канал: {2}\n   " +
                                              "* дата начала измерения : `{3}`\n   " +
                                              "* первый сохраненный файл : `{4}`\n   " +
                                              "* дата окончания измерения : `{5}`\n   " +
                                              "* статус : {6}\n   " +
                                              "* список файлов:";
        
        private readonly CommandContext _context;
        private readonly ILogger< GetSpectrumDetailsCommand> _logger;
    }
}