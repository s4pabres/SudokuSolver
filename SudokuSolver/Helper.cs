using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Helper
    {
        public static readonly Dictionary<string, string[]> Neighbours = new Dictionary<string, string[]>()
        {
            {"BOX_A1", new[] { "BOX_D1", "BOX_A3" }},
            {"BOX_D1", new[] { "BOX_A1", "BOX_G1", "BOX_D4" }},
            {"BOX_G1", new[] { "BOX_D1", "BOX_G4" }},
            {"BOX_A4", new[] { "BOX_A1", "BOX_D4", "BOX_A7" }},
            {"BOX_D4", new[] { "BOX_D1", "BOX_A4", "BOX_G4", "BOX_D7" }},
            {"BOX_G4", new[] { "BOX_G1", "BOX_D4", "BOX_G7" }},
            {"BOX_A7", new[] { "BOX_G1", "BOX_D4" }},
            {"BOX_D7", new[] { "BOX_D4", "BOX_A7", "BOX_G7" }},
            {"BOX_G7", new[] { "BOX_G4", "BOX_D7" }},
        };


        public static IPAddress[] GetAllLocalIPv4(NetworkInterfaceType _type)
        {
            List<IPAddress> ipAddrList = new List<IPAddress>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrList.Add(ip.Address);
                        }
                    }
                }
            }
            return ipAddrList.ToArray();
        }

        public static (ushort x, ushort y) GetLocals(string Name,string coords)
        {
            var x = (ushort) (coords[0] - Name[4]);
            var y = (ushort) (coords[1] - Name[5]);
            return (x, y);
        }

    }
}
