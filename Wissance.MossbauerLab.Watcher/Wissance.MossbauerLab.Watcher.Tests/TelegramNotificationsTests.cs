using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Text;
using System.Collections.Generic;

using Wissance.MossbauerLab.Watcher.Common.Data;
using Wissance.MossbauerLab.Watcher.Services.Notification;
using Microsoft.QualityTools.Testing.Fakes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Common;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Services.Tests
{
    [TestClass]
    public class TelegramNotificationsTests
    {
        [TestInitialize]
        public void Init()
        {
            _bin = Convert.FromBase64String("NjI1MzUyNzMxNjpBQUYzWGZuSnE2azlTMnFldTc2bmd6SHhEU29id3BMcm50SQ==");
            _key = Encoding.UTF8.GetString(_bin);
            _templates = new Dictionary<SpectrometerEvent, MessageTemplate>();
            _templates[SpectrometerEvent.SpectrumSaved] = new MessageTemplate(true, @"Templates\testTelegramMessageTemplate.txt", @"Templates\testEmptySpectraTemplate.txt");
            _tgRequisites = new TelegramSendRequisites(_key, -1001520411610, null);
            _telegramNotifier = new TelegramNotifier(_tgRequisites, new TemplateManager(_templates), new LoggerFactory());
        }
        
        [TestMethod]
        public async Task SendNotificationTest()
        {
          

            SpectrumReadyData spectra = new SpectrumReadyData
            {
                Spectrum = new byte[] { 4, 5, 6 },
                Name = "Test spectra",
                Channel = 1,
                Updated = DateTime.Now,
                RawInfo = new System.IO.FileInfo("textFileForFileInfo.txt")
            };
           
           
            bool result = await _telegramNotifier.NotifySpectrumSavedAsync(new List<SpectrumReadyData> { spectra, spectra });
            Assert.IsTrue(result);

        }
        [TestMethod]
        public async Task NotifyAboutTroubles()
        {
            bool result = await _telegramNotifier.NotifySpectrumSavedAsync(new List<SpectrumReadyData>());
            Assert.IsTrue(result);
        }

        private byte[] _bin;
        private string _key;
        private TelegramSendRequisites _tgRequisites;
        private TelegramNotifier _telegramNotifier;
        private IDictionary<SpectrometerEvent, MessageTemplate> _templates;
    }
}
