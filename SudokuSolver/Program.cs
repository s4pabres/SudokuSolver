using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Program
    {
        private const ushort Port = 8080;

      

        static void Main(string[] args)
        {
            string Name = args[0];
            ushort[,] Cells = new ushort[3, 3];
            foreach (var cell in args[1].Split(','))
            {
                Cells[(ushort)(cell[0] - '0'), (ushort)(cell[1] - '0')] = (ushort)(cell[3] - '0');
            }

            TcpClient Manager = new TcpClient();
            Manager.Connect(args[2].Split(':')[1].Substring(2), Int32.Parse(args[2].Split(':')[2]));

            Helper.SendMessage(Manager, Name + "," + Helper.GetAllLocalIPv4(NetworkInterfaceType.Ethernet)[0] + "," +
                                        Port);

            Dictionary<string, TcpClient> Neighbours = new Dictionary<string, TcpClient>();
            


            Console.ReadKey();
        }

    }
}
