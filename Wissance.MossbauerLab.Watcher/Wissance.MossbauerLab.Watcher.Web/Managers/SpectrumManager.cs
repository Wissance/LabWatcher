using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
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
            _config = appConfig;
            _logger = loggerFactory.CreateLogger<SpectrumManager>();
        }

        public async Task<OperationResultDto<SpectrumSamplesInfoDto>> GetSpectrumSamplesAsync(int spectrumId)
        {
            try
            {
                SpectrumEntity spectrum = await _context.Spectra.FirstOrDefaultAsync(s => s.Id == spectrumId);
                if (spectrum == null)
                {
                    string msg = $"Spectrum with id: {spectrumId} was not found";
                    _logger.LogDebug(msg);
                    return new OperationResultDto<SpectrumSamplesInfoDto>(false, (int)HttpStatusCode.NotFound, $"Spectrum with id: {spectrumId} was not found", null);
                }

                string[] samples = await GetSamples(spectrum);
                return new OperationResultDto<SpectrumSamplesInfoDto>(true, (int)HttpStatusCode.OK, null, SpectrumSampleFactory.Create(spectrum, samples));
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during getting spectrum {spectrumId} samples, error: {e.Message}";
                _logger.LogError(msg);
                return new OperationResultDto<SpectrumSamplesInfoDto>(true, (int)HttpStatusCode.InternalServerError, msg, null);
            }
        }

        public async Task<OperationResultDto<byte[]>> GetSpectrumSampleFileAsync(int spectrumId, string sampleNumber)
        {
            try
            {
                SpectrumEntity spectrum = await _context.Spectra.FirstOrDefaultAsync(s => s.Id == spectrumId);
                if (spectrum == null)
                {
                    string msg = $"Spectrum with id: {spectrumId} was not found";
                    _logger.LogDebug(msg);
                    return new OperationResultDto<byte[]>(false, (int)HttpStatusCode.NotFound, msg, null);
                }

                string[] samples = await GetSamples(spectrum);
                if (samples == null || samples.Length == 0)
                {
                    _logger.LogWarning("There are no samples, it looks suspicious maybe shared folder or FTP are inaccessible? ");
                    return new OperationResultDto<byte[]>(false, (int)HttpStatusCode.OK, $"Samples are empty", null);
                }

                string sampleFile = samples.FirstOrDefault(s => s.ToLower().Contains(sampleNumber.ToLower()));
                if (sampleFile == null)
                {
                    string msg = $"There are no sample {sampleNumber} for spectrum with id: {spectrumId}";
                    _logger.LogDebug(msg);
                    return new OperationResultDto<byte[]>(false, (int)HttpStatusCode.NotFound, msg, null);
                }

                byte[] spectrumSampleContent = await ReadSpectrumFile(spectrum, sampleFile);
                return new OperationResultDto<byte[]>(true, (int)HttpStatusCode.OK, null, spectrumSampleContent);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during getting spectrum {spectrumId} sample \"{sampleNumber}\", error: {e.Message}";
                _logger.LogError(msg);
                return new OperationResultDto<byte[]>(true, (int)HttpStatusCode.InternalServerError, msg, null);
            }
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
                using AsyncFtpClient ftp = new AsyncFtpClient(_config.FtpArchSettings.FtpSettings.Host, _config.FtpArchSettings.FtpSettings.Username,
                    _config.FtpArchSettings.FtpSettings.Password, _config.FtpArchSettings.FtpSettings.Port);
                await ftp.SetWorkingDirectory(spectrum.Location);
                string[] samples = await ftp.GetNameListing();
                await ftp.SetWorkingDirectory("/");
                await ftp.Disconnect();
                return samples;
            }
        }

        private async Task<byte[]> ReadSpectrumFile(SpectrumEntity spectrum, string sampleFile)
        {
            if (!spectrum.IsArchived)
            {
                string fullPath = Path.Combine(spectrum.Location, sampleFile);
                byte[] spectrumSampleContent = await _storeService.ReadAsync(fullPath);
                return spectrumSampleContent;
            }
            else
            {
                // we should get from FTP, temporarily we can't do (FTP was not configured properly and we still in DEBUG)
                using AsyncFtpClient ftp = new AsyncFtpClient(_config.FtpArchSettings.FtpSettings.Host, _config.FtpArchSettings.FtpSettings.Username,
                    _config.FtpArchSettings.FtpSettings.Password, _config.FtpArchSettings.FtpSettings.Port);
                await ftp.SetWorkingDirectory(spectrum.Location);
                byte[] spectrumSampleContent = await ftp.DownloadBytes(sampleFile, new CancellationToken());
                return null;
            }
        }

        private readonly ILogger<SpectrumManager> _logger;
        private readonly IModelContext _context;
        private readonly IFileStoreService _storeService;
        private readonly ApplicationConfig _config;
    }
}
