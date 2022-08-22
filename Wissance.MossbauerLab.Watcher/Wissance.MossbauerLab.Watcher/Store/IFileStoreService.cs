using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Store
{
    public interface IFileStoreService
    {
        Task<IList<string>> GetChildrenAsync(string shareName, string parent);
        Task<FileInfo> GetFileInfoAsync(string fileName);
        Task<IList<FileInfo>> GetAllDirectoryFilesInfoAsync(string directory);
        Task<byte[]> ReadAsync(string fileName);
    }
}
