using System.Collections.Generic;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public static class CommandDefs
    {
        public const string StartCmd = "/start";
        public const string HelpCmd = "/help";
        public const string ListSpectraCmd = "/list-spectra";
        public const string GetSpectrumInfoCmd = "/get-spectrum-info";
        public const string GetSpectrumFilesCmd = "/get-spectrum-files ";
        public const string CheckStateCmd = "/check-state";

        public static IDictionary<string, string> KeyboardCaptions = new Dictionary<string, string>()
        {
            {ListSpectraCmd, "Вывести список спектров"},
            {GetSpectrumInfoCmd, "Вывесте детальную информацию по спектру"},
            {GetSpectrumFilesCmd, "Получить файлы спектра"},
            {CheckStateCmd, "Проверить состояние спектрометра"}
        };
    }
}