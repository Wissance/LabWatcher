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
using Wissance.MossbauerLab.Watcher.Services.Store;
using Wissance.MossbauerLab.Watcher.Web.Config;

namespace Wissance.MossbauerLab.Watcher.Web.Services.Jobs
{
    public class FTPArchiveFilesJob : IJob
    {
        

        public FTPArchiveFilesJob(IFileStoreService storeService, ModelContext context, ILoggerFactory loggerFactory, ApplicationConfig config)
        {
            _storeService= storeService;
            _context = context;
            _logger = loggerFactory.CreateLogger<FTPArchiveFilesJob>();
            _config = config;

        }
        public async Task Execute(IJobExecutionContext context)
        {
            //TODO: refactor
         
            string relativeDir = GetRelativePathWinShare();
            var fileInfos = (await _storeService.GetAllDirectoryFilesInfoAsync(relativeDir))
                .Where(x => x.LastWriteTimeUtc > DateTime.UtcNow.AddDays(_config.FTPSettings.ArchiveWhenFileIsOlderThanInDays));
            var fileNamesToTransfer = fileInfos
                .Select(x => x.FullName);
            var bytesToTransfer = new List<byte[]>();
            foreach (var file in fileNamesToTransfer)
            {
                bytesToTransfer.Add(await _storeService.ReadAsync(file));
            }
            var dict = fileNamesToTransfer.Zip(bytesToTransfer).ToDictionary(x => x.First, x => x.Second);
            using AsyncFtpClient ftp = new AsyncFtpClient(_config.FTPSettings.Host, _config.FTPSettings.UserCredentials.User, _config.FTPSettings.UserCredentials.Password);
            await ftp.AutoConnect();
            var dirPath = @$"{_config.FTPSettings.ServerFolderPath}\ArchivedSpectra_{DateTime.UtcNow}";
            await ftp.CreateDirectory(dirPath);
            await ftp.SetWorkingDirectory(dirPath);
            foreach (var item in dict)
            {
                await ftp.UploadBytes(item.Value, item.Key);
            }
            await ftp.Disconnect();

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
        private readonly ILogger<FTPArchiveFilesJob> _logger;
        private readonly ApplicationConfig _config;
    }
}
