using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Text;
using System.Collections.Generic;

using Wissance.MossbauerLab.Watcher.Common.Data;
using Wissance.MossbauerLab.Watcher.Services.Notification;
using Microsoft.QualityTools.Testing.Fakes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Services.Tests
{
    [TestClass]
    public class TelegramNotificationsTests
    {
        [TestMethod]
        public async Task SendNotificationTest()
        {
            byte[] bin = Convert.FromBase64String("NjI1MzUyNzMxNjpBQUYzWGZuSnE2azlTMnFldTc2bmd6SHhEU29id3BMcm50SQ==");
            string key = Encoding.UTF8.GetString(bin);

            TelegramSendRequisites tgRequisites = new TelegramSendRequisites(-1001520411610, key, "Templates\\testTelegramMessageTemplate.txt", TODO);

            SpectrumReadyData spectra = new SpectrumReadyData
            {
                Spectrum = new byte[] { 4, 5, 6 },
                Name = "Test spectra",
                Channel = 1,
                Updated = DateTime.Now,
                RawInfo = new System.IO.FileInfo("textFileForFileInfo.txt")
            };
           
            TelegramNotifier telegramNotifier = new TelegramNotifier(tgRequisites, new LoggerFactory());
            bool result = await telegramNotifier.NotifySpectrumSavedAsync(new List<SpectrumReadyData> { spectra, spectra });
            Assert.IsTrue(result);

        }
    }
}
