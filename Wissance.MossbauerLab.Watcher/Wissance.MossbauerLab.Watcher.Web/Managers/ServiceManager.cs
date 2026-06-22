using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Services.Notification;
using Wissance.MossbauerLab.Watcher.Services.Store;
using Wissance.MossbauerLab.Watcher.Web.Config;

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
        
        private readonly ILogger<ServiceManager> _logger;
        private readonly IModelContext _context;
        private readonly IFileStoreService _storeService;
        private readonly ISpectrumMeasureEventsNotifier _emailNotifier;
        private readonly ISpectrumMeasureEventsNotifier _tgNotifier;
        private readonly ApplicationConfig _config;
    }
}