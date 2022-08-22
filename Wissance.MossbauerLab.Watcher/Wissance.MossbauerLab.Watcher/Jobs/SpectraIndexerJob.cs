using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Data.Entities;
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
                    DateTime? first = null;
                    DateTime? last = null;
                    bool nonEmpty = true;

                    foreach (string child in children)
                    {
                        // 0. Get short name without path
                        string shortName = Path.GetFileName(child);
                        Sm2201SpectrumNameData nameData = null;
                        // 1. Extract info from directory or file
                        if (child.EndsWith(".spc"))
                        {
                            // we working with single file
                            // 2. get from spectrum additional info
                            FileInfo info = await _storeService.GetFileInfoAsync(child);
                            // 2.1 channel number i.e. 1n220311.spc, 1 means - 1 channel (in future analyze letter to understand what spectrum type is f - a-Fe, n - SNP)
                            nameData = Sm2201SpectrumNameParser.Parse(shortName);
                            // 2.2 get date of measure end (by last modified date)
                            last = info.LastWriteTime;
                        }
                        else
                        {
                            // we working with a set of files ...
                            nameData = Sm2201SpectrumNameParser.Parse(shortName);
                            // todo: list files in directory, if we have any create db record
                            IList<FileInfo> files = await _storeService.GetAllDirectoryFilesInfoAsync(child);
                            if (files.Any())
                            {
                                first = files.First().LastWriteTime;
                                last = files.Last().LastWriteTime;
                            }
                            else
                            {
                                nonEmpty = false;
                            }

                        }
                        SpectrumEntity spectrum = _context.Spectra.FirstOrDefault(s => string.Equals(s.Name.ToLower(), shortName.ToLower()));
                        if (spectrum == null)
                        {
                            // creating new one
                            if (nonEmpty)
                                _context.Spectra.Add(new SpectrumEntity(shortName, string.Format(SpectrumDescriptionTemplate, nameData.OneLetterSpectrumType, nameData.Channel),
                                                                        child, nameData.MeasureStart, first, last));
                        }
                        else
                        {
                            // updating existing one
                            // update ere only last ...
                            if (spectrum.Last != last)
                                spectrum.Last = last;
                            // todo(UMV): think maybe i should update something else ...

                        }

                    }
                    int result = await _context.SaveChangesAsync();
                    if (result < 0)
                    {
                        _logger.LogError("An error occurred during indexed spectra data save");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during spectra indexation job: {e.Message}");
            }

            _logger.LogInformation("*********** Spectra indexation job finished ***********");
        }

        private const string SpectrumDescriptionTemplate = "Спектр типа: {0} измерен на канале {1} спектрометра";

        private readonly string _spectraShare;
        private readonly IFileStoreService _storeService;
        private readonly IModelContext _context;
        private readonly ILogger<SpectraIndexerJob> _logger;
    }
}
