namespace Wissance.MossbauerLab.Watcher.Web.Data
{
    public enum ControllingItemType
    {
        SharedFolder,
        SpectrometerControllingComputer,
        NetworkFileStorage,  // NAS
        Database
    }
    
    public class ControllingItem
    {
        public ControllingItem()
        {
        }

        public ControllingItem(ControllingItemType itemType, string itemStr, bool state,  string stateStr)
        {
            ItemType = itemType;
            ItemStr = itemStr;
            State = state;
            StateStr = stateStr;
        }

        public ControllingItemType ItemType { get; set; }
        public string ItemStr { get; set; }
        public bool State { get; set; }
        public string StateStr { get; set; }
    }
}