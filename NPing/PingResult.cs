using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace MD.NTools.NPing {

    public class PingResult {

        #region Properties

        public IPAddress Address { get; set; }

        public byte[] Buffer { get; set; }

        public PingOptions Options { get; set; }

        public long RoundtripTime { get; set; }

        public IPStatus Status { get; set; }

        #endregion

        #region Public Methods

        public static PingResult FromPingReply(PingReply reply) {
            if (reply == null) {
                return new PingResult() {
                    Status = IPStatus.Unknown,
                };
            }

            PingResult result = new PingResult() {
                Address = reply.Address,
                Buffer = reply.Buffer,
                Options = reply.Options,
                RoundtripTime = reply.RoundtripTime,
                Status = reply.Status,
            };

            return result;
        }

        #endregion

    }
}
