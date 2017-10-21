using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MD.NTools.NPing {
    public class PingTask {

        #region Properties

        public List<PingResult> Caches { get; private set; }

        public PingArgs Args { get; set; }

        #endregion

        #region Public Methods

        public PingUnitResult Start() {
            this.Caches = new List<PingResult>();

            for (int i = 0; i < this.Args.Count; i++) {
                long start = DateTime.Now.Ticks;
                var item = PingHelper.Send(this.Args.Address, this.Args.Port, this.Args.PingType, this.Args.Timeout);
                this.Caches.Add(item);
                long spent = (DateTime.Now.Ticks - start) / 10000;
                if (item == null || item.Status != System.Net.NetworkInformation.IPStatus.Success) {
                    if (item == null) {
                        Console.WriteLine("Timeout.");
                    } else {
                        Console.WriteLine(item.Status);
                    }
                } else {
                    Console.WriteLine("From {0} {1}: {2}ms", item.Address, this.Args.PingType, item.RoundtripTime);
                }
                int sleepTimeout = this.Args.Interval - (int)spent;
                if (sleepTimeout > 0) {
                    Thread.Sleep(sleepTimeout);
                }
            }
            var ur = PingUnitResult.GetResult(this.Args.Address, this.Caches, this.Args.PingType);
            return ur;
        }

        public static PingTask FromCommandLines(string[] args) {
            PingTask task = new PingTask();
            task.Args = PingArgs.FromCommandLines(args);
            return task;
        }

        #endregion
    }
}
