using System;
using System.Collections.Generic;
using System.Text;

namespace Wissance.MossbauerLab.Watcher.Common.Data.Notification
{
    public class TemplateManager
    {
        public TemplateManager()
        {
            Templates = new Dictionary<SpectrometerEvent, MessageTemplate>();
        }

        public TemplateManager(IDictionary<SpectrometerEvent, MessageTemplate> templates)
        {
            Templates = templates;
        }

        public IDictionary<SpectrometerEvent, MessageTemplate> Templates { get; set; }
    }
}
