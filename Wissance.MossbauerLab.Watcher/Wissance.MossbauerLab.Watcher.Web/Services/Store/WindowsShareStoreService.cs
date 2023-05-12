﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Services.Store;
using Wissance.MossbauerLab.Watcher.Web.Config;

//using UserCredentials = SimpleImpersonation.UserCredentials;

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
            string folder = shareName;
            if (!string.IsNullOrEmpty(_config.Address))
                folder = Path.Combine($@"\\{_config.Address}", shareName);

            try
            {

                if (!string.Equals(parent, RootFolder))
                {
                    if (!string.IsNullOrEmpty(_config.Address))
                        folder = Path.Combine($@"\\{_config.Address}", shareName, parent);
                    else
                    {
                        folder = Path.Combine(shareName, parent);
                    }
                }

                List<string> children = Directory.GetDirectories(folder).ToList();
                children.AddRange(Directory.GetFiles(folder));
                _logger.LogDebug($"Shared folder \"{folder}\" on server {_config.Address} contains {children.Count} items");
                return children;

            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during getting children items of shared folder: {e.Message} using path {folder}");
                return null;
            }

        }

        public async Task<FileInfo> GetFileInfoAsync(string fileName)
        {
            try
            {
                FileInfo currFile = new FileInfo(fileName);
                return currFile;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"An error occurred during getting fileInfo of file: \"{fileName}\", error: {e.Message}");
                return null;
            }
        }

        public async Task<IList<FileInfo>> GetAllDirectoryFilesInfoAsync(string directory)
        {
            IList<FileInfo> filesData = new List<FileInfo>();
            try
            {
                IList<string> children = Directory.GetFiles(directory).ToList();

                filesData = children.Select(c => new FileInfo(c)).OrderBy(c => c.LastWriteTime).ToList();

                _logger.LogDebug($"Directory \"{directory}\" on server {_config.Address} contains {children.Count} items");
                return filesData;

            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during getting all shared folder files: {e.Message}");
                return null;
            }
        }

        public async Task<bool> RemoveDirectoryRecursiveAsync(string directory)
        {
            try
            {
                Directory.Delete(directory, true);
                return true;

            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during removing folder files: {e.Message}");
                return false;
            }
        }

        public async Task<Tuple<FileInfo, byte[]>> GetLastChangedFileAsync(string directory)
        {
            try
            {
                IList<FileInfo> children = await GetAllDirectoryFilesInfoAsync(directory);
                FileInfo info = children.Last();
                byte[] spectrum = await ReadAsync(info.FullName);
                return new Tuple<FileInfo, byte[]>(info, spectrum);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during getting last written file in shared folder: {e.Message}");
                return null;
            }
        }

        public async Task<byte[]> ReadAsync(string fileName)
        {
            try
            {
                return await File.ReadAllBytesAsync(fileName);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during reading file: {fileName}, error: {e.Message}");
                return null;
            }
        }


        private const string RootFolder = ".";

        private readonly SpectraStoreConfig _config;
        private readonly ILogger<WindowsShareStoreService> _logger;
    }
}
