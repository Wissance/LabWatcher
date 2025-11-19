using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
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

                OperationResultDto<SpectrumSamplesInfoDto> spectrumInfo = await manager.GetSpectrumSamplesAsync(spectrumId);
                if (!spectrumInfo.Success)
                {
                    _logger.LogError($"Error occurred during getting Spectrum samples in \"GetSpectrumDetailsCommand\" : {spectrumInfo.Message}");
                    await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, CommandAnswerLocalizationDefs.UnknownError);
                    return false;
                }
                // todo(UMV): form answer
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during the handling {_context.Command} command, {e.Message}");
                return false;
            }
        }
        
        private readonly CommandContext _context;
        private readonly ILogger< GetSpectrumDetailsCommand> _logger;
    }
}