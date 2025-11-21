using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                ControllingItem sharedFolderState = await CheckSharedFolder();
                state.StateItems.Add(sharedFolderState);
                return state;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during checking state of the measuring device, {e.Message}");
                return null;
            }
        }

        private async Task<ControllingItem> CheckSharedFolder()
        {
            IList<string> items = await _storeService.GetChildrenAsync(_config.Sm2201SpectraStoreSettings.Folder, ".");
            // if there are no items it means that shared folder is not mounted
            bool isAccessible = items.Any();
                return new ControllingItem(ControllingItemType.SharedFolder,
                    ControlDefs.SharedFolderItemName, isAccessible, string.Format(ControlDefs.SharedFolderStateMessageTemplate,
                        _config.Sm2201SpectraStoreSettings.Folder, 
                        isAccessible ? ControlDefs.AccessibleMessage : ControlDefs.NotAccessibleMessage));
        }
        

        private readonly ModelContext _context;
        private readonly IFileStoreService _storeService;
        private readonly ApplicationConfig _config;
        private readonly ILogger<StateCheckerService> _logger;
    }
}