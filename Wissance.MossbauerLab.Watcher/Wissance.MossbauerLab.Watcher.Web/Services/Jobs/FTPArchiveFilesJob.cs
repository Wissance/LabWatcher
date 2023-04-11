using FluentFTP;

using Microsoft.Extensions.Logging;

using Quartz;

using System;
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
            //using AsyncFtpClient ftp = new AsyncFtpClient(_config.FTPSettings.Host, _config.FTPSettings.UserCredentials.User, _config.FTPSettings.UserCredentials.Password);
            //await ftp.AutoConnect();
            string relativeDir = GetRelativePathWinShare();
            var fileNamesToTransfer = (await _storeService.GetAllDirectoryFilesInfoAsync(relativeDir))
                .Where(x => x.LastWriteTimeUtc > DateTime.UtcNow.AddDays(_config.FTPSettings.ArchiveWhenFileIsOlderThanInDays))
                .Select(x => x.FullName);
            
          
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
