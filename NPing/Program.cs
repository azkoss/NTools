using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD.NTools.NPing {
    class Program {
        static void Main(string[] args) {

            var pingTask = PingTask.FromCommandLines(args);
            if (pingTask.Args == null || !pingTask.Args.IsValid) {
                Console.WriteLine("Command parameter is invalid.");
                return;
            }
            var result = pingTask.Start();
            if (result != null) {
                Console.WriteLine("");
                //Console.WriteLine("finished ping {0} {1}", pingTask.Args.Address, pingTask.Args.PingType);
                Console.WriteLine("sent={0},received={1},lost={2}({3:0%})", result.SentCount, result.ReceivedCount, result.LostCount, result.LostPercent);
                Console.WriteLine("min={0}ms,max={1}ms,avg={2:0.##}ms", result.MinRoundtripTime, result.MaxRoundtripTime, result.AverageRoundtripTime);
            }
        }
    }
}
