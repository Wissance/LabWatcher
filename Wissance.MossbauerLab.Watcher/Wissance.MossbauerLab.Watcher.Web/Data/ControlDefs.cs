namespace Wissance.MossbauerLab.Watcher.Web.Data
{
    public static class ControlDefs
    {
        public const string SharedFolderItemName = "Общая сетевая папка";
        public const string SpectrometerControllingComputerItemName = "Управляющий компьютер";
        public const string NetworkFileStorageItemName = "Сетевое хранилище";
        public const string DatabaseItemName = "База данных";
        
        public const string SharedFolderStateMessageTemplate = "Сетевая папка общего доступа: \"_{0}_\" - *{1}*";
        public const string SpectrometerControllingComputerStateMessageTemplate = "Компьютер \"_{0}_\" управляющий спектрометром c IP-адресом \"_{1}_\" - *{2}*";
        public const string NetworkFileStorageStateMessageTemplate = "Сетевое хранилище \"_{0}_\" - *{1}*";
        public const string DatabaseStateMessageTemplate = "База данных \"_{0}_\" - *{1}*";

        public const string AccessibleMessage = "ОК";
        public const string NotAccessibleMessage = "Нет доступа";
    }
}