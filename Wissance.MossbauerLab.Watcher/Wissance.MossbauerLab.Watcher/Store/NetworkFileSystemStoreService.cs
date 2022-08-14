using System;
using System.Collections.Generic;
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

        public async Task<IList<string>> GetChildrenAsync(string shareName, string parent)
        {
            try
            {
                UserCredentials credentials = GetCredentials();
                using (SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.Interactive))
                {
                    IList<string> children = await WindowsIdentity.RunImpersonatedAsync(userHandle, async () =>
                    {
                        return System.IO.Directory.GetFiles($@"\\{_config.Address}\{shareName}").ToList();
                    });
                    // todo: umv: implement filtering
                    return children;
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during getting children items of shared folder: {e.Message}");
                return null;
            }

        }

        public Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }

        private UserCredentials GetCredentials()
        {
            if (_config.UserCredentials == null)
                return null;
            return new UserCredentials(_config.UserCredentials.User, _config.UserCredentials.Password, _config.Domain);
        }

        private readonly SpectraStoreConfig _config;
        private readonly ILogger<NetworkFileSystemStoreService> _logger;
    }
}
