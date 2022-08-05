using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Smb
{
    /// <summary>
    ///    Service that allows to work with Smb V1 (https://github.com/TalAloni/SMBLibrary/blob/master/ClientExamples.md)
    /// </summary>
    public class Smb1Service : ISmbService
    {
        public Task<IList<string>> GetChildrenAsync(string parent)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }
    }
}
