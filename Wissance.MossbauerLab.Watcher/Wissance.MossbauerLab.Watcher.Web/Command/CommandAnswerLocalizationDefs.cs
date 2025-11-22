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

        public const string UnknownError = "Произошла ошибка, проверьте состояние системы: " + CheckStateCmd + " и свяжитесь с администратором систем";

        public const string SpectrumIdNotProvidedError = "Для выполнения этой команды необходимо предоставить целочисленный идентификатор спектра";
        public const string SpectrumIdCantBeExtractedError = "Значение переданное в качестве идентификатора спектра не является целочисленным";
        public const string SpectrometryLabState = "Состояние контролируемых в лаборатории объектов:";
        public const string SpectrumWasNotFound = "Спектр с таким идентификатором не найден в базе данных";
        public const string UserOperationIsNotPermitted = "Для выполнения команды необходимо быть членом группы мессбауэровской спектроскопии, обратитесь к администратору группы";
        
        public static readonly IDictionary<string, string> KeyboardCaptions = new Dictionary<string, string>()
        {
            {ListSpectraCmd, "Вывести список спектров"},
            {GetSpectrumInfoCmd, "Вывесте детальную информацию по спектру"},
            {GetSpectrumFilesCmd, "Получить файлы спектра"},
            {CheckStateCmd, "Проверить состояние спектрометра"}
        };
    }
}