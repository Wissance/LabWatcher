using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Common.Data;
using Wissance.MossbauerLab.Watcher.Common.Utils;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Data.Entities;
using Wissance.MossbauerLab.Watcher.Services.Notification;
using Wissance.MossbauerLab.Watcher.Services.Store;
using Wissance.MossbauerLab.Watcher.Web.Config;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.MossbauerLab.Watcher.Web.Managers
{
    public class ServiceManager
    {
        public ServiceManager(ModelContext dbContext, ILoggerFactory loggerFactory, IFileStoreService storeService,
            EmailNotifier emailNotifier, TelegramNotifier tgNotifier, ApplicationConfig config)
        {
            _context = dbContext;
            _storeService = storeService;
            _emailNotifier = emailNotifier;
            _tgNotifier = tgNotifier;
            _config = config;
            _logger = loggerFactory.CreateLogger<ServiceManager>();
        }

        public async Task<OperationResultDto<bool>> ManualSendEmailOfLastSavedSpectraAsync()
        {
            try
            {
                // TODO(UMV): make service Layer to get actual spectra
                // these are spectra that were measured today
                IList<SpectrumEntity> actualSpectra = await _context.Spectra.Where(s => s.Last != null && s.Last.Value.Date == DateTime.Now.Date).ToListAsync();
                // 2. If Now - Last < threshold (2-3 hours, then send)
                IList<SpectrumEntity> lastSavedSpectra = actualSpectra.Where(s => DateTime.Now <= s.Last.Value.AddHours(_config.NotificationSettings.Threshold)).ToList();
                // 3. Get last saved spectra
                IList<SpectrumReadyData> dataToSend = new List<SpectrumReadyData>();
                foreach (SpectrumEntity spectrum in lastSavedSpectra)
                {
                    _logger.LogDebug("Getting last saved file (prepare data to send) for spectrum {0}", spectrum.Name);
                    string relativeDir = "";
                    if (!string.IsNullOrEmpty(_config.Sm2201SpectraStoreSettings.Address))
                    {
                        relativeDir = $@"\\{_config.Sm2201SpectraStoreSettings.Address}\{_config.Sm2201SpectraStoreSettings.Folder}\{spectrum.Name}";
                    }
                    else
                    {
                        relativeDir = Path.Combine(_config.Sm2201SpectraStoreSettings.Folder, spectrum.Name);
                    }
                    Tuple<FileInfo, byte[]> lastSavedSpec = await _storeService.GetLastChangedFileAsync(relativeDir);
                    dataToSend.Add(new SpectrumReadyData(spectrum.Name, Sm2201SpectrumNameParser.Parse(spectrum.Name).Channel, 
                        spectrum.Last.Value, lastSavedSpec.Item2, lastSavedSpec.Item1));
                }

                bool sendResult = await _emailNotifier.NotifySpectrumSavedAsync(dataToSend);
                OperationResultDto<bool> result = new OperationResultDto<bool>(sendResult,
                    sendResult ? (int)HttpStatusCode.OK : (int)HttpStatusCode.InternalServerError,
                    sendResult ? string.Empty : "Something goes wrong during e-mail send, see the logs", sendResult);
                return result;
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during \"ManualSendEmailAsync\", {e.Message}";
                _logger.LogError(msg);
                _logger.LogError(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
            }
        }

        private readonly ILogger<ServiceManager> _logger;
        private readonly IModelContext _context;
        private readonly IFileStoreService _storeService;
        private readonly ISpectrumMeasureEventsNotifier _emailNotifier;
        private readonly ISpectrumMeasureEventsNotifier _tgNotifier;
        private readonly ApplicationConfig _config;
    }
}