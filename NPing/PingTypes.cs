using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.NTools.NPing {
    [Serializable]
    public enum PingTypes {
        None,
        Icmp,
        Udp,
        IcmpOrUdp,
    }
}
