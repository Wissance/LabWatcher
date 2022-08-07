using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SMBLibrary;
using SMBLibrary.Client;
using Wissance.MossbauerLab.Watcher.Web.Config;

namespace Wissance.MossbauerLab.Watcher.Web.Smb
{
    /// <summary>
    ///    Service that allows to work with Smb V1 (https://github.com/TalAloni/SMBLibrary/blob/master/ClientExamples.md)
    /// </summary>
    public class Smb1Service : ISmbService
    {
        public Smb1Service(SmbConfig config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<Smb1Service>();
        }

        public async Task<IList<string>> GetChildrenAsync(string parent)
        {
            try
            {
                IList<string> children = new List<string>();
                SMB1Client client = new SMB1Client();
                bool isConnected = client.Connect(IPAddress.Parse(_config.Address), SMBTransportType.DirectTCPTransport);
                if (isConnected)
                {
                    bool hasPassword = _config.UserCredentials != null;
                    NTStatus status = client.Login(_config.Domain, hasPassword ? _config.UserCredentials.User : String.Empty,
                                                   hasPassword ? _config.UserCredentials.Password : String.Empty);
                    if (status == NTStatus.STATUS_SUCCESS)
                    {
                        List<string> shares = client.ListShares(out status);
                        // todo ...
                        client.Logoff();
                    }
                    client.Disconnect();
                }
                return children;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            //client.Connect()
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }

        private readonly SmbConfig _config;
        private readonly ILogger<Smb1Service> _logger;
    }
}
