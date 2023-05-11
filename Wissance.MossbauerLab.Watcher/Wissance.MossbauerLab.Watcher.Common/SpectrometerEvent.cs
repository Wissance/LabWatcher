using System;
using System.Collections.Generic;
using System.Text;

namespace Wissance.MossbauerLab.Watcher.Common
{
    public enum SpectrometerEvent
    {
        SpectrumSaved = 1,
        ConnectionToSm2201Lost = 2,
        NetworkLost = 3
    }
}
