﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace MinerPluginToolkitV1
{
    // RENAME MinersApiPortsManager to FreePortsCheckerManager
    public static class MinersApiPortsManager
    {
        const int _reserveTimeSeconds = 5;
        private static Dictionary<int, DateTime> _reservedPortsAtTime = new Dictionary<int, DateTime>();

        private static bool IsPortFree(int port, IPEndPoint[] tcpOrUdpPorts)
        {
            var isTaken = tcpOrUdpPorts.Any(tcp => tcp.Port == port);
            return !isTaken;
        }

        private static bool CanReservePort(int port, DateTime currentTime)
        {
            if (_reservedPortsAtTime.ContainsKey(port))
            {
                var reservedTime = _reservedPortsAtTime[port];
                var secondsDiff = (currentTime - reservedTime).TotalSeconds;
                return secondsDiff > _reserveTimeSeconds;
            }
            return true;
        }

        public static int GetAvaliablePortInRange(int portStart = 4000, int next = 2300)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpIpEndpoints = ipGlobalProperties.GetActiveTcpListeners();
            var udpIpEndpoints = ipGlobalProperties.GetActiveUdpListeners();

            var now = DateTime.UtcNow;

            var port = portStart;
            var newPortEnd = portStart + next;
            for (; port < newPortEnd; ++port)
            {
                var tcpFree = IsPortFree(port, tcpIpEndpoints);
                var udpFree = IsPortFree(port, udpIpEndpoints);
                var canReserve = CanReservePort(port, now);
                if (tcpFree && udpFree && canReserve)
                {
                    // reserve port and return
                    _reservedPortsAtTime[port] = now;
                    return port;
                }
            }
            return -1; // we can't retrive free port
        }

        // TODO add implement get random port
    }
}
