using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Web.Config;
using Wissance.MossbauerLab.Watcher.Web.Services.Notification;
using Wissance.MossbauerLab.Watcher.Web.Services.Store;

namespace Wissance.MossbauerLab.Watcher.Web.Services.Jobs
{
    public class SpectraNotifyJob : IJob
    {
        public SpectraNotifyJob(IFileStoreService storeService, ModelContext context, EmailNotifier mailNotifier, ILoggerFactory loggerFactory, ApplicationConfig config)
        {
            _storeService = storeService;
            _context = context;
            _config = config;
            _logger = loggerFactory.CreateLogger<SpectraNotifyJob>();
            _mailNotifier = mailNotifier;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // 1. Get spectra that we updated today last.Date() == Now.Date()
            // 2. If Now - Last < threshold (2-3 hours, then send)
            // 3.. Activate send
            bool result = await _mailNotifier.NotifySpectrumSavedAsync(null);
        }

        private readonly ApplicationConfig _config;
        private readonly IFileStoreService _storeService;
        private readonly IModelContext _context;
        private readonly ISpectrumReadyNotifier _mailNotifier;
        private readonly ILogger<SpectraNotifyJob> _logger;
    }
}
