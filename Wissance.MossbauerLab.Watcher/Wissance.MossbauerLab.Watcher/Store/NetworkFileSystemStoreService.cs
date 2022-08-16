using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;
using Wissance.MossbauerLab.Watcher.Web.Config;
using UserCredentials = SimpleImpersonation.UserCredentials;

namespace Wissance.MossbauerLab.Watcher.Web.Store
{
    public class NetworkFileSystemStoreService : IFileStoreService
    {
        public NetworkFileSystemStoreService(SpectraStoreConfig config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<NetworkFileSystemStoreService>();
        }

        public async Task<IList<string>> GetChildrenAsync(string shareName, string parent = RootFolder)
        {
            try
            {
                // todo (UMV): think about what would happened if we run app from Linux
                using (SafeAccessTokenHandle userHandle = WindowsIdentity.GetAnonymous().AccessToken)
                {
                    string folder = $@"\\{_config.Address}\{shareName}";
                    if (!string.Equals(parent, RootFolder))
                    {
                        folder = $@"\\{_config.Address}\{shareName}\{parent}";
                    }

                    IList<string> children = await WindowsIdentity.RunImpersonatedAsync(userHandle, async () =>
                    {
                        return System.IO.Directory.GetFiles(folder).ToList();
                    });

                    return children;
                }

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

        private UserCredentials GetCredentials()
        {
            if (_config.UserCredentials == null)
                return new UserCredentials("admin", "123");
            return new UserCredentials(_config.UserCredentials.User, _config.UserCredentials.Password, _config.Domain);
        }

        private const string RootFolder = ".";

        private readonly SpectraStoreConfig _config;
        private readonly ILogger<NetworkFileSystemStoreService> _logger;
    }
}
