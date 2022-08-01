using System;
using System.Collections.Generic;
using System.Text;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.MossbauerLab.Watcher.Data.Entities
{
    public class SpectrumEntity : IModelIdentifiable<int>
    {
        public int Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime? First { get; set; }
        public DateTime? Last { get; set; }
    }
}
