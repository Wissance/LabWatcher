using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Services.Store;
using Wissance.MossbauerLab.Watcher.Web.Command;
using Wissance.MossbauerLab.Watcher.Web.Config;
using Wissance.MossbauerLab.Watcher.Web.Data;

namespace Wissance.MossbauerLab.Watcher.Web.Services.State
{
    public class StateCheckerService
    {
        public StateCheckerService(ModelContext context, IFileStoreService storeService, ApplicationConfig config, 
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _storeService = storeService;
            _config = config;
            _logger = loggerFactory.CreateLogger<StateCheckerService>();
        }

        public async Task<SpectrometryLabState> CheckStateAsync()
        {
            try
            {
                SpectrometryLabState state = new SpectrometryLabState();
                // 1. Shared Folder check state
                ControllingItem sharedFolderState = await CheckSharedFolderAsync();
                state.StateItems.Add(sharedFolderState);
                // 2. Spectrometer Computer check state
                ControllingItem spectrometerComputerState = await CheckMeasureControllingMachineAsync();
                state.StateItems.Add(spectrometerComputerState);
                // 3. Network file storage check state
                ControllingItem networkFileStorageState = await CheckNetworkFileStorageAsync();
                state.StateItems.Add(networkFileStorageState);
                // 4. Database state
                ControllingItem databaseState = await CheckDatabaseAsync();
                state.StateItems.Add(databaseState);
                return state;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during checking state of the measuring device, {e.Message}");
                return null;
            }
        }

        private async Task<ControllingItem> CheckSharedFolderAsync()
        {
            IList<string> items = await _storeService.GetChildrenAsync(_config.Sm2201SpectraStoreSettings.Folder, ".");
            // if there are no items it means that shared folder is not mounted
            bool isAccessible = items.Any();
                return new ControllingItem(ControllingItemType.SharedFolder,
                    ControlDefs.SharedFolderItemName, isAccessible, string.Format(ControlDefs.SharedFolderStateMessageTemplate,
                        _config.Sm2201SpectraStoreSettings.Folder, 
                        isAccessible ? ControlDefs.AccessibleMessage : ControlDefs.NotAccessibleMessage));
        }

        private async Task<ControllingItem> CheckMeasureControllingMachineAsync()
        {
            bool success = false;
            using (Ping pingChecker = new Ping())
            {
                PingReply response = await pingChecker.SendPingAsync(_config.Sm2201SpectraStoreSettings.Address, 
                    TimeSpan.FromSeconds(2).Milliseconds);
                success = response.Status == IPStatus.Success;
            }

            return new ControllingItem(ControllingItemType.SpectrometerControllingComputer,
                ControlDefs.SpectrometerControllingComputerItemName, success,
                string.Format(ControlDefs.SpectrometerControllingComputerStateMessageTemplate,
                    _config.Sm2201SpectraStoreSettings.Domain,
                    _config.Sm2201SpectraStoreSettings.Address,
                    success ? ControlDefs.AccessibleMessage : ControlDefs.NotAccessibleMessage));
        }

        private async Task<ControllingItem> CheckNetworkFileStorageAsync()
        {
            bool success = false;
            using (Ping pingChecker = new Ping())
            {
                PingReply response = await pingChecker.SendPingAsync(_config.FtpArchSettings.FtpSettings.Host, 
                    TimeSpan.FromSeconds(2).Milliseconds);
                success = response.Status == IPStatus.Success;
            }
            
            return new ControllingItem(ControllingItemType.NetworkFileStorage,
                ControlDefs.NetworkFileStorageItemName, success,
                string.Format(ControlDefs.NetworkFileStorageStateMessageTemplate,
                    _config.FtpArchSettings.FtpSettings.Host,
                    success ? ControlDefs.AccessibleMessage : ControlDefs.NotAccessibleMessage));
        }
        
        private async Task<ControllingItem> CheckDatabaseAsync()
        {
            bool dbState = await _context.Spectra.AnyAsync();
            return new ControllingItem(ControllingItemType.Database, ControlDefs.DatabaseItemName, dbState,
                string.Format(ControlDefs.DatabaseStateMessageTemplate, _config.ConnStr, 
                    dbState ? ControlDefs.AccessibleMessage : ControlDefs.NotAccessibleMessage));
        }

        private readonly ModelContext _context;
        private readonly IFileStoreService _storeService;
        private readonly ApplicationConfig _config;
        private readonly ILogger<StateCheckerService> _logger;
    }
}