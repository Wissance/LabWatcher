using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Web.Store;
using Wissance.MossbauerLab.Watcher.Web.Utils;

namespace Wissance.MossbauerLab.Watcher.Web.Jobs
{
    public class SpectraIndexerJob : IJob
    {
        public SpectraIndexerJob(IFileStoreService storeService, ModelContext context, ILoggerFactory loggerFactory/*, string spectraShare*/)
        {
            _storeService = storeService;
            _context = context;
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
                    /*byte[] content = await _storeService.ReadAsync(children[0]);
                    if (content != null)
                    {

                    }*/
                    foreach (string child in children)
                    {
                        // 0. Get short name without path
                        string shortName = Path.GetFileName(child);
                        // 1. Extract info from directory or file
                        if (child.EndsWith(".spc"))
                        {
                            // we working with single file
                            // 2. get from spectrum additional info
                            // 2.1 channel number i.e. 1n220311.spc, 1 means - 1 channel
                            // 2.2 analyze letter to understand what spectrum type is f - a-Fe, n - SNP
                            // 2.3 get date of measure start
                            // 2.4 get date of measure end (by last modified date)
                        }
                        else
                        {
                            // we working with a set of files ...
                            Sm2201SpectrumNameData nameData = Sm2201SpectrumNameParser.Parse(shortName);
                            // todo: list files in directory, if we have any create db record
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during spectra indexation job: {e.Message}");
            }

            _logger.LogInformation("*********** Spectra indexation job finished ***********");
        }

        private readonly string _spectraShare;
        private readonly IFileStoreService _storeService;
        private readonly IModelContext _context;
        private readonly ILogger<SpectraIndexerJob> _logger;
    }
}
