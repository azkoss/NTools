using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MD.NTools.NPing {

    public class PingArgs {

        #region Properties

        public IPAddress Address { get; set; }

        private PingTypes _pingType = PingTypes.Icmp;
        public PingTypes PingType {
            get {
                return _pingType;
            }
            set {
                _pingType = value;
            }
        }

        public int Port { get; set; }

        private int _count = 4;
        public int Count {
            get {
                return _count;
            }
            set {
                _count = value;
            }
        }

        private int _timeout = 2000;
        public int Timeout {
            get {
                return _timeout;
            }
            set {
                _timeout = value;
            }
        }

        private int _interval = 1000;
        public int Interval {
            get {
                return _interval;
            }
            set {
                _interval = value;
            }
        }

        public bool IsValid {
            get {
                return this.Address != null && this.Address != IPAddress.Any && (this.PingType == PingTypes.Icmp || this.Port > 0);
            }
        }

        #endregion

        #region Public Methods

        public static PingArgs FromCommandLines(string[] args) {
            List<string> rawArgs = new List<string>();
            Dictionary<string, string> options = new Dictionary<string, string>();
            int i = 0;
            while (i < args.Length) {
                string name = args[i];
                string value = i + 1 < args.Length ? args[i + 1] : null;

                if (name.StartsWith("-")) {
                    string key = name.Substring(1);
                    if (value.StartsWith("-")) {
                        string key2 = value.Substring(1);
                        options[key] = null;
                    } else {
                        options[key] = value;
                    }
                } else {
                    rawArgs.Add(args[i]);
                }
                i++;
            }

            int? count = GetInt(options, "c");
            int? timeout = GetInt(options, "t");
            int? interval = GetInt(options, "i");
            string p = options.ContainsKey("p") ? options["p"] : null;

            var result = new PingArgs();
            result.Count = count ?? result.Count;
            result.Timeout = timeout ?? result.Timeout;
            result.Interval = interval ?? result.Interval;
            result.PingType = p == "u" ? PingTypes.Udp : result.PingType;

            var line = rawArgs.FirstOrDefault();
            if (rawArgs.Any()) {
                var items = line.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                IPAddress ad;
                if (!string.IsNullOrWhiteSpace(items[0]) && IPAddress.TryParse(items[0], out ad)) {
                    result.Address = ad;
                }
                if (items.Length > 1) {
                    int port = 0;
                    if (int.TryParse(items[1], out port)) {
                        result.Port = port;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Private Methods

        private static int? GetInt(Dictionary<string, string> options, string key) {
            int? value = null;
            if (options.ContainsKey(key) && options[key] != null) {
                string cValue = options[key].Trim();
                int tValue = 0;
                if (int.TryParse(cValue, out tValue)) {
                    value = tValue;
                }
            }
            return value;
        }

        #endregion

    }
}
