using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MD.NTools.NPing {
    public static class PingHelper {

        public static PingResult Send(IPAddress address, int port, PingTypes pingType, int timeout = 5000) {
            if (pingType == PingTypes.Icmp) {
                return SendIcmp(address, timeout);
            } else if (pingType == PingTypes.Udp) {
                return SendUdp(address, port, timeout);
            }
            return null;
        }

        public static Task<PingResult> SendAsync(IPAddress address, int port, PingTypes pingType, int timeout = 5000) {
            if (pingType == PingTypes.Icmp) {
                return SendIcmpAsync(address, timeout);
            } else if (pingType == PingTypes.Udp) {
                return SendUdpAsync(address, port, timeout);
            }
            return null;
        }

        public static PingResult SendIcmp(string hostNameOrAddress, int timeout = 5000) {
            IPAddress address = null;
            if (!IPAddress.TryParse(hostNameOrAddress, out address)) {
                return null;
            }
            return SendIcmp(address, timeout);
        }

        public static PingResult SendIcmp(IPAddress address, int timeout = 5000) {
            Ping ping = new Ping();
            var reply = ping.Send(address, timeout);
            var result = PingResult.FromPingReply(reply);
            if (result.Address == null || result.Address.Equals(IPAddress.Any)) {
                result.Address = address;
            }
            return result;
        }

        public static Task<PingResult> SendIcmpAsync(string hostNameOrAddress, int timeout = 5000) {
            IPAddress address = null;
            if (!IPAddress.TryParse(hostNameOrAddress, out address)) {
                return null;
            }
            return SendIcmpAsync(address, timeout);
        }

        public static Task<PingResult> SendIcmpAsync(IPAddress address, int timeout = 5000) {
            var tcs = new TaskCompletionSource<PingResult>();
            Ping ping = new Ping();
            ping.PingCompleted += (obj, sender) => {
                var ad = sender.UserState as IPAddress;
                var result = PingResult.FromPingReply(sender.Reply);
                if (result.Address == null || result.Address.Equals(IPAddress.Any)) {
                    result.Address = ad;
                }
                tcs.SetResult(result);
            };
            ping.SendAsync(address, timeout, address);
            return tcs.Task;
        }

        public static PingResult SendUdp(string hostNameOrAddress, int port, int timeout = 5000) {
            IPAddress address = null;
            if (!IPAddress.TryParse(hostNameOrAddress, out address)) {
                return null;
            }
            return SendUdp(address, port, timeout);
        }

        public static PingResult SendUdp(IPAddress address, int port, int timeout = 5000) {
            UdpClient client = new UdpClient();
            client.Client.ReceiveTimeout = timeout;

            IPEndPoint remoteEndPoint = new IPEndPoint(address, port);

            byte[] data = BitConverter.GetBytes(DateTime.Now.Ticks);

            client.Send(data, data.Length, remoteEndPoint);
            PingResult result = new PingResult() {
                Address = address,
                Status = IPStatus.TimedOut,
            };
            try {
                var response = client.Receive(ref remoteEndPoint);
                if (response != null) {
                    var ticks = BitConverter.ToInt64(response, 0);
                    var roundtripTime = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;

                    result = new PingResult() {
                        Address = address,
                        Buffer = response,
                        RoundtripTime = roundtripTime,
                        Status = IPStatus.Success,
                    };
                    return result;
                }
            } catch (Exception ex) {

            }
            return result;
        }

        public static Task<PingResult> SendUdpAsync(string hostNameOrAddress, int port, int timeout = 5000) {
            IPAddress address = null;
            if (!IPAddress.TryParse(hostNameOrAddress, out address)) {
                return null;
            }
            return SendUdpAsync(address, port, timeout);
        }

        public static Task<PingResult> SendUdpAsync(IPAddress address, int port, int timeout = 5000) {
            return Task.Factory.StartNew(() => {
                return SendUdp(address, port, timeout);
            });
        }

    }
}
