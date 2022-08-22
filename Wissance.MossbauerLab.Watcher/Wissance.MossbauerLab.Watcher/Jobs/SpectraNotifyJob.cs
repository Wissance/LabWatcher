using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Web.Store;

namespace Wissance.MossbauerLab.Watcher.Web.Jobs
{
    public class SpectraNotifyJob : IJob
    {
        public SpectraNotifyJob(IFileStoreService storeService, ModelContext context, ILoggerFactory loggerFactory)
        {
            _storeService = storeService;
            _context = context;
            // todo: umv: pass!
            _spectraShare = "Autosaves";
            _logger = loggerFactory.CreateLogger<SpectraNotifyJob>();
        }

        public Task Execute(IJobExecutionContext context)
        {
            // 1. Get spectra that we updated today last.Date() == Now.Date()
            // 2. If Now - Last < threshold (2-3 hours, then send)
            // 3.. Activate send
            throw new NotImplementedException();
        }

        private readonly string _spectraShare;
        private readonly IFileStoreService _storeService;
        private readonly IModelContext _context;
        private readonly ILogger<SpectraNotifyJob> _logger;
    }
}
