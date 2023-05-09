using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.MossabuerLab.Watcher.Dto;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Data.Entities;
using Wissance.MossbauerLab.Watcher.Services.Store;
using Wissance.MossbauerLab.Watcher.Web.Config;
using Wissance.MossbauerLab.Watcher.Web.Factories;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Managers;

namespace Wissance.MossbauerLab.Watcher.Web.Managers
{
    public class SpectrumManager: EfModelManager<SpectrumEntity, SpectrumInfoDto, int>
    {
        public SpectrumManager(ModelContext dbContext, ILoggerFactory loggerFactory, IFileStoreService storeService, ApplicationConfig appConfig) 
            : base(dbContext, null, SpectrumFactory.Create, loggerFactory)
        {
            _context = dbContext;
            _storeService = storeService;
            _appConfig = appConfig;
        }

        public async Task<OperationResultDto<SpectrumSamplesInfoDto>> GetSpectrumSamplesAsync(int spectrumId)
        {
            try
            {
                SpectrumEntity spectrum = await _context.Spectra.FirstOrDefaultAsync(s => s.Id == spectrumId);
                if (spectrum == null)
                    return new OperationResultDto<SpectrumSamplesInfoDto>(false, (int)HttpStatusCode.NotFound, $"Spectrum with id: {spectrumId} was not found", null);
                string[] samples = await GetSamples(spectrum);
                return new OperationResultDto<SpectrumSamplesInfoDto>(true, (int)HttpStatusCode.OK, null, SpectrumSampleFactory.Create(spectrum, samples));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<OperationResultDto<byte[]>> GetSpectrumSampleFileAsync(int spectrum, string sampleNumber)
        {
            return null;
        }

        private async Task<string[]> GetSamples(SpectrumEntity spectrum)
        {
            if (!spectrum.IsArchived)
            {
                IList<FileInfo> samples = await _storeService.GetAllDirectoryFilesInfoAsync(spectrum.Location);
                return samples.Select(s => s.Name).ToArray();
            }
            else
            {
                // we should get from FTP, temporarily we can't do (FTP was not configured properly and we still in DEBUG)
                return null;
            }
        }

        private readonly IModelContext _context;
        private readonly IFileStoreService _storeService;
        private readonly ApplicationConfig _appConfig;
    }
}
