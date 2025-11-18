using System.Collections.Generic;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public static class CommandAnswerLocalizationDefs
    {
        public static string GetArchivedCaption(bool isArchived)
        {
            return isArchived ? "архивный" : "активный";
        }

        public const string StartCmd = "/start";
        public const string HelpCmd = "/help";
        public const string ListSpectraCmd = "/listSpectra";
        public const string GetSpectrumInfoCmd = "/getSpectrumInfo";
        public const string GetSpectrumFilesCmd = "/getSpectrumFiles ";
        public const string CheckStateCmd = "/checkState";

        public const string UnknownError = "Произошла ошибка, свяжитесь с администратором систем";

        public static readonly IDictionary<string, string> KeyboardCaptions = new Dictionary<string, string>()
        {
            {ListSpectraCmd, "Вывести список спектров"},
            {GetSpectrumInfoCmd, "Вывесте детальную информацию по спектру"},
            {GetSpectrumFilesCmd, "Получить файлы спектра"},
            {CheckStateCmd, "Проверить состояние спектрометра"}
        };
    }
}