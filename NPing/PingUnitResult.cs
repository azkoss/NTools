using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace MD.NTools.NPing {
    public class PingUnitResult {

        #region Stat Properties

        public IPAddress Address { get; set; }

        public string Ip {
            get {
                return this.Address == null ? null : this.Address.ToString();
            }
        }

        public int SentCount { get; set; }

        public int ReceivedCount { get; set; }

        public int LostCount {
            get {
                return this.SentCount - ReceivedCount;
            }
        }

        public float LostPercent { get; set; }

        public long? MaxRoundtripTime { get; set; }

        public long? MinRoundtripTime { get; set; }

        public double? AverageRoundtripTime { get; set; }

        public PingTypes PingType { get; set; }

        #endregion

        #region Public Methods

        public static PingUnitResult GetResult(IPAddress address, IEnumerable<PingResult> results, PingTypes pingType) {
            PingUnitResult result = new PingUnitResult() {
                Address = address,
                PingType = pingType,
            };

            if (results == null || !results.Any()) {
                result.LostPercent = 1;
                return result;
            }

            var successes = results.Where(m => m != null && m.Status == IPStatus.Success);

            result.SentCount = results.Count();
            result.ReceivedCount = successes.Count();
            result.LostPercent = (float)(result.SentCount - result.ReceivedCount) / (float)result.SentCount;

            if (successes != null && successes.Any()) {
                var ascResults = successes.OrderBy(m => m.RoundtripTime);
                var minItem = ascResults.FirstOrDefault();
                var maxItem = ascResults.LastOrDefault();

                result.MinRoundtripTime = minItem.RoundtripTime;
                result.MaxRoundtripTime = maxItem.RoundtripTime;
                result.AverageRoundtripTime = successes.Average(m => m.RoundtripTime);
            }

            return result;
        }

        #endregion

    }
}
