using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SMBLibrary;
using SMBLibrary.Client;
using SMBLibrary.SMB1;
using Wissance.MossbauerLab.Watcher.Web.Config;

namespace Wissance.MossbauerLab.Watcher.Web.Store
{
    /// <summary>
    ///    Service that allows to work with Smb V1 (https://github.com/TalAloni/SMBLibrary/blob/master/ClientExamples.md)
    /// </summary>
    public class Smb1Service : IFileStoreService
    {
        public Smb1Service(SpectraStoreConfig config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<Smb1Service>();
        }

        //todo (UMV): add task wrap with timeout ...
        public async Task<IList<string>> GetChildrenAsync(string shareName, string parent)
        {
            try
            {
                IList<string> children = new List<string>();
                SMB1Client client = new SMB1Client();
                bool isConnected = client.Connect(IPAddress.Parse(_config.Address), SMBTransportType.NetBiosOverTCP);
                if (isConnected)
                {
                    bool hasPassword = _config.UserCredentials != null;
                    NTStatus status = client.Login(_config.Domain, hasPassword ? _config.UserCredentials.User : String.Empty,
                                                   hasPassword ? _config.UserCredentials.Password : String.Empty);
                    if (status == NTStatus.STATUS_SUCCESS)
                    {
                        List<string> shares = client.ListShares(out status).Select(s => s.ToLower()).ToList();
                        // todo (UMV) ...
                        if (shares.Contains(shareName.ToLower()))
                        {
                            // find folder, we assume folder on level 1 ...
                            // if we have "." as folder we should return all we got from share ....
                            ISMBFileStore fileStore = client.TreeConnect(shareName, out status);
                            if (status == NTStatus.STATUS_SUCCESS)
                            {
                                object handle;
                                FileStatus fileStatus;
                                status = fileStore.CreateFile(out handle, out fileStatus, "\\", AccessMask.GENERIC_READ, 
                                                              FileAttributes.Directory, ShareAccess.Read | ShareAccess.Write, 
                                                              CreateDisposition.FILE_OPEN, CreateOptions.FILE_DIRECTORY_FILE, null);
                                if (status == NTStatus.STATUS_SUCCESS)
                                {
                                    List<FindInformation> fileList;
                                    status = ((SMB1FileStore)fileStore).QueryDirectory(out fileList, "\\*", FindInformationLevel.SMB_FIND_FILE_DIRECTORY_INFO);
                                    status = fileStore.CloseFile(handle);

                                    foreach (FindInformation info in fileList)
                                    {
                                        
                                    }
                                }
                            }
                            else
                            {
                                _logger.LogWarning(NtStatusIsFailure);
                            }
                        }
                        else
                        {
                            _logger.LogWarning(ShareNotFoundOnServer, shareName);
                        }

                        client.Logoff();
                    }
                    else
                    {
                        _logger.LogWarning(NtStatusIsFailure);
                    }
                    client.Disconnect();
                }
                return children;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }

        private const string ShareNotFoundOnServer = "Share with name {0} was not found on server!";
        private const string NtStatusIsFailure = "Smb status is false";

        private readonly SpectraStoreConfig _config;
        private readonly ILogger<Smb1Service> _logger;
    }
}
