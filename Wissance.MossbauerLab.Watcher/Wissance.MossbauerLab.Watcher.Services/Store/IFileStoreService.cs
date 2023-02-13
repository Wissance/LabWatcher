using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Services.Store
{
    public interface IFileStoreService
    {
        Task<IList<string>> GetChildrenAsync(string shareName, string parent);
        Task<FileInfo> GetFileInfoAsync(string fileName);
        //Task<FileInfo> GetLastWrittenFileInfoAsync(string directory);
        Task<IList<FileInfo>> GetAllDirectoryFilesInfoAsync(string directory);
        Task<Tuple<FileInfo, byte[]>> GetLastChangedFileAsync(string directory);
        Task<byte[]> ReadAsync(string fileName);
    }
}
