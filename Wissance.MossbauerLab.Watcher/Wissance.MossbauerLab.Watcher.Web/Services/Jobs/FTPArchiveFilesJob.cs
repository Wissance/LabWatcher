using FluentFTP;

using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Validations;

using Quartz;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Data.Entities;
using Wissance.MossbauerLab.Watcher.Services.Store;
using Wissance.MossbauerLab.Watcher.Web.Config;

namespace Wissance.MossbauerLab.Watcher.Web.Services.Jobs
{
    public class FtpArchiveFilesJob : IJob
    {
        

        public FtpArchiveFilesJob(IFileStoreService storeService, ModelContext context, ILoggerFactory loggerFactory, ApplicationConfig config)
        {
            _storeService= storeService;
            _context = context;
            _logger = loggerFactory.CreateLogger<FtpArchiveFilesJob>();
            _config = config;

        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("*********** FTP archiving job started ***********");
            try
            {
                // 1. Retrieve spectra that should be transferred to ftp and made archived
                // TODO:UMV: add IsArchived = False
                IList<SpectrumEntity> archSpectra = _context.Spectra.Where(s => s.Last != null && s.Last.Value < DateTime.Now.AddDays(-1 * _config.FtpArchSettings.TransferThreshold)).ToList();
                // 2. Process every spectra and do the same
                foreach (SpectrumEntity spectrum in archSpectra)
                {
                    // 3. Get all files in spectra directory from Shared Directory/SMB &&
                    //    Transfer every file in appropriate directory using FtpClient
                    _logger.LogDebug($"Started to archive spectrum: {spectrum.Name}");
                    int result = await TransferSpectrumFiles(spectrum.Name);
                    // 4. Set spectrum.IsArchived = true
                    if (result > 0)
                    {
                        // spectrum.IsArchived = true;
                        _logger.LogDebug($"Successfully transferred {result} samples of spectrum: \" {spectrum.Name}\" from shared folder to FTP");
                    }
                    else
                    {
                        _logger.LogDebug($"Spectrum transfer \"{spectrum.Name}\" from shared folder to FTP failed");
                    }
                    
                }
                // 6. Save Context
                int saveResult = await _context.SaveChangesAsync();


                // Нужно, вероятно достать из БД, а далее последовательно обрабатывать каждый спектр из БД
               /* string relativeDir = GetRelativePathWinShare();
                IList<FileInfo> archSpectraFolders = (await _storeService.GetAllDirectoryFilesInfoAsync(relativeDir))
                    .Where(x => x.LastWriteTimeUtc > DateTime.UtcNow.AddDays(_config.FtpArchSettings.TransferThreshold)).ToList();
                IList<string> foldersNames = archSpectraFolders.Select(x => x.FullName).ToList();
                List<byte[]> bytesToTransfer = new List<byte[]>();
                foreach (string folder in foldersNames)
                {
                    // это не правильно! ReadAsync считывает отдельный файл, мы же должны копировать всю папку
                    bytesToTransfer.Add(await _storeService.ReadAsync(folder));
                }

                var filesContent = foldersNames.Zip(bytesToTransfer).ToDictionary(x => x.First, x => x.Second);
                using AsyncFtpClient ftp = new AsyncFtpClient(_config.FtpArchSettings.FtpSettings.Host,
                    _config.FtpArchSettings.FtpSettings.Username, _config.FtpArchSettings.FtpSettings.Password, _config.FtpArchSettings.FtpSettings.Port);
                await ftp.AutoConnect();
                // неправильное имя!!!!!
                var dirPath = @$"{_config.FtpArchSettings.FtpArchRootDir}\ArchivedSpectra_{DateTime.UtcNow}";
                bool ftpDirCreationResult = await ftp.CreateDirectory(dirPath);
                await ftp.SetWorkingDirectory(dirPath);
                foreach (KeyValuePair<string, byte[]> item in filesContent)
                {
                    await ftp.UploadBytes(item.Value, item.Key);
                }

                await ftp.Disconnect();*/

            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during FTP archiving job: {e.Message}");
            }
            _logger.LogInformation("*********** FTP archiving job finished ***********");
        }

        private async Task<int> TransferSpectrumFiles(string spectrumName)
        {
            try
            {
                // todo (UMV): define roper PATH ....
                string relativeDir = GetRelativePathWinShare();
                IList<string> children = await _storeService.GetChildrenAsync(_config.Sm2201SpectraStoreSettings.Folder, spectrumName);
                string dirPath = Path.Combine(@$"{_config.FtpArchSettings.FtpArchRootDir}", spectrumName);
                using AsyncFtpClient ftp = new AsyncFtpClient(_config.FtpArchSettings.FtpSettings.Host, _config.FtpArchSettings.FtpSettings.Username, 
                                                              _config.FtpArchSettings.FtpSettings.Password, _config.FtpArchSettings.FtpSettings.Port);
                bool ftpDirCreationResult = await ftp.CreateDirectory(dirPath);
                if (!ftpDirCreationResult)
                {
                    _logger.LogDebug($"FTP directory \"{dirPath}\" creation result is false, can't copy files");
                    return -1;
                }

                await ftp.SetWorkingDirectory(dirPath);
                foreach (string child in children)
                {
                    // Get File content
                    // await _storeService.ReadAsync(folder);
                    // Transfer to FTP
                }
                // going to ROOT dir 
                await ftp.SetWorkingDirectory(".");

                await ftp.Disconnect();
                return children.Count;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during archive spectrum: \"{spectrumName}\", error: {e.Message}");
                return -1;
            }
            
        }

        private string GetRelativePathWinShare()
        {
            string relativeDir;
            if (!string.IsNullOrEmpty(_config.Sm2201SpectraStoreSettings.Address))
            {
                relativeDir = $@"\\{_config.Sm2201SpectraStoreSettings.Address}\{_config.Sm2201SpectraStoreSettings.Folder}";
            }
            else
            {
                relativeDir = _config.Sm2201SpectraStoreSettings.Folder;
            }

            return relativeDir;
        }

        private readonly IFileStoreService _storeService;
        private readonly ModelContext _context;
        private readonly ILogger<FtpArchiveFilesJob> _logger;
        private readonly ApplicationConfig _config;
    }
}
