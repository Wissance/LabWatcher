namespace Wissance.MossbauerLab.Watcher.Web.Data
{
    public static class ControlDefs
    {
        public const string SharedFolderItemName = "Общая сетевая папка";
        public const string SharedFolderStateMessageTemplate = "Сетевая папка общего доступа: \"{0}\" - {1}";
        public const string SpectrometerControllingComputerStateMessageTemplate = "Компьютер \"{0}\" управляющий спектрометром c IP-адресом \"{1}\" - {2}";
        public const string NetworkFileStorageStateMessageTemplate = "Сетевое хранилище \"{0}\" - {1}";
        public const string DatabaseStateMessageTemplate = "База данных \"{0}\" - {1}";

        public const string AccessibleMessage = "ОК";
        public const string NotAccessibleMessage = "Нет доступа";
    }
}