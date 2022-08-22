using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using Wissance.MossbauerLab.Watcher.Web.Config;
using UserCredentials = SimpleImpersonation.UserCredentials;

namespace Wissance.MossbauerLab.Watcher.Web.Services.Store
{
    /// <summary>
    ///    Class that allows to access shared folder files and dirs Anonymously (we don't use credentials here because we attempt to use it in Linux)
    /// </summary>
    public class WindowsShareStoreService : IFileStoreService
    {
        public WindowsShareStoreService(SpectraStoreConfig config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<WindowsShareStoreService>();
        }

        public async Task<IList<string>> GetChildrenAsync(string shareName, string parent = RootFolder)
        {
            try
            {
                // todo (UMV): think about what would happened if we run app from Linux
                using (SafeAccessTokenHandle userHandle = GetAccessToken())
                {
                    string folder = $@"\\{_config.Address}\{shareName}";
                    if (!string.Equals(parent, RootFolder))
                    {
                        folder = $@"\\{_config.Address}\{shareName}\{parent}";
                    }

                    IList<string> children = await WindowsIdentity.RunImpersonatedAsync(userHandle, async () =>
                    {
                        List<string> entries = Directory.GetDirectories(folder).ToList();
                        entries.AddRange(Directory.GetFiles(folder));
                        return entries;
                    });

                    _logger.LogDebug($"Shared folder \"{folder}\" on server {_config.Address} contains {children.Count} items");
                    return children;
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during getting children items of shared folder: {e.Message}");
                return null;
            }

        }

        public async Task<FileInfo> GetFileInfoAsync(string fileName)
        {
            try
            {
                using (SafeAccessTokenHandle userHandle = GetAccessToken())
                {
                    FileInfo currFile = new FileInfo(fileName);
                    return currFile;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during getting fileInfo of file: \"{fileName}\", error: {e.Message}");
                return null;
            }
        }

        public async Task<IList<FileInfo>> GetAllDirectoryFilesInfoAsync(string directory)
        {
            IList<FileInfo> filesData = new List<FileInfo>();
            try
            {
                // todo (UMV): think about what would happened if we run app from Linux
                using (SafeAccessTokenHandle userHandle = GetAccessToken())
                {

                    IList<string> children = await WindowsIdentity.RunImpersonatedAsync(userHandle, async () =>
                    {
                        return Directory.GetFiles(directory).ToList();
                    });

                    filesData = children.Select(c => new FileInfo(c)).OrderBy(c => c.LastWriteTime).ToList();

                    _logger.LogDebug($"Directory \"{directory}\" on server {_config.Address} contains {children.Count} items");
                }
                
                return filesData;

            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during getting children items of shared folder: {e.Message}");
                return null;
            }
        }

        public async Task<byte[]> ReadAsync(string fileName)
        {
            try
            {
                using (SafeAccessTokenHandle userHandle = WindowsIdentity.GetAnonymous().AccessToken)
                {
                    return await File.ReadAllBytesAsync(fileName);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during reading file: {fileName}, error: {e.Message}");
                return null;
            }
        }

        private SafeAccessTokenHandle GetAccessToken()
        { 
            SafeAccessTokenHandle userHandle = WindowsIdentity.GetAnonymous().AccessToken;
            return userHandle;
        }

        private const string RootFolder = ".";

        private readonly SpectraStoreConfig _config;
        private readonly ILogger<WindowsShareStoreService> _logger;
    }
}
