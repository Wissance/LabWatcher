using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Web.Config;

namespace Wissance.MossbauerLab.Watcher.Web.Store
{
    public class NetworkFileSystemStoreService : IFileStoreService
    {
        public NetworkFileSystemStoreService(SpectraStoreConfig config, ILoggerFactory loggerFactory)
        {

        }

        public Task<IList<string>> GetChildrenAsync(string shareName, string parent)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }

        private readonly SpectraStoreConfig _config;
        private readonly ILogger<Smb1Service> _logger;
    }
}
