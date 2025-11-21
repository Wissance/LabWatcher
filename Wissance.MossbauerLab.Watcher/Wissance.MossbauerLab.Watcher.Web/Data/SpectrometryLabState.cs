using System.Collections.Generic;

namespace Wissance.MossbauerLab.Watcher.Web.Data
{
    public class SpectrometryLabState
    {
        public SpectrometryLabState()
        {
            StateItems = new List<ControllingItem>();
        }

        public SpectrometryLabState(IList<ControllingItem> stateItems)
        {
            StateItems = stateItems;
        }

        public IList<ControllingItem> StateItems { get; set; }
    }
}