using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Wissance.MossbauerLab.Watcher.Web.Store;

namespace Wissance.MossbauerLab.Watcher.Web.Jobs
{
    public class SpectraIndexerJob : IJob
    {
        public SpectraIndexerJob(IFileStoreService storeService, ILoggerFactory loggerFactory/*, string spectraShare*/)
        {
            _storeService = storeService;
            _spectraShare = "Autosaves";
            _logger = loggerFactory.CreateLogger<SpectraIndexerJob>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("*********** Spectra indexation job started ***********");
            try
            {
                IList<string> children = await _storeService.GetChildrenAsync(_spectraShare, ".");
                if (children != null && children.Any())
                {
                    // todoL umv: save to database 
                    byte[] content = await _storeService.ReadAsync(children[0]);
                    if (content != null)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during spectra indexation job: {e.Message}");
            }

            _logger.LogInformation("*********** Spectra indexation job finished ***********");
        }

        private readonly IFileStoreService _storeService;
        private readonly string _spectraShare;
        private readonly ILogger<SpectraIndexerJob> _logger;
    }
}
