using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SudokuSolver
{
    class Program
    {
        private const ushort Port = 8080;
        private static readonly IPAddress IP = Helper.GetAllLocalIPv4(NetworkInterfaceType.Ethernet)[0];

        private static ConcurrentDictionary<TcpClient, Thread> allClients = new ConcurrentDictionary<TcpClient, Thread>();


        static void WaitForConnections()
        {
            TcpListener listener = new TcpListener(IP, Port);
            listener.Start();

            while (true)
            {
                var c = listener.AcceptTcpClient();
                var t = new Thread(new ParameterizedThreadStart(Message.ReadClientMessage));
                t.Start(c);
                allClients[c] = t;
            }
        }

        static void Main(string[] args)
        {
            // INIT
            Box box = new Box(args[0], args[1]);

            Console.WriteLine("URI: tcp://" + IP.ToString() + ":" + Port);
            Console.WriteLine("Name: " + box.Name);
            Console.WriteLine("Cells:");
            Console.WriteLine(box);
            // 

            // Create Thread to wait for any connections...don't care who
            Thread myThread = new Thread(new ThreadStart(WaitForConnections));
            myThread.Start();
            //

            // Connect to Manager and tell who we are
            TcpClient Manager = new TcpClient();
            try
            {
                Manager.Connect(args[2].Split(':')[1].Substring(2), Int32.Parse(args[2].Split(':')[2]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                Environment.Exit(0);
            }
            Message.SendMessage(Manager, box.Name + "," + IP.ToString() + "," +
                                        Port);
            //

            // Ask Manager for Neighbour-adresses and connect to them
            foreach (var neighbour in Helper.Neighbours[box.Name])
            {
                // Send Message to ask Manager for for Address of neigbour
                Message.SendMessage(Manager, neighbour);
                //

                // Wait until Answer gets recieved
                while (true)
                {
                    if (Manager.GetStream().CanRead)
                    {
                        // Recieve Message
                        byte[] bytes = new byte[Manager.ReceiveBufferSize];
                        Manager.GetStream().Read(bytes, 0, Manager.ReceiveBufferSize);
                        string msg = Encoding.ASCII.GetString(bytes);
                        // 

                        // Match Message if it is valid
                        Match m = Regex.Match(msg, "^([^,]+),([0-9]+)$");
                        if (m.Success)
                        {
                            // Connect To Client
                            Helper.NeighbourClients[neighbour] = new TcpClient();
                            try
                            {
                                Helper.NeighbourClients[neighbour]
                                    .Connect(m.Groups[1].Value, UInt16.Parse(m.Groups[2].Value));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                Console.ReadKey();
                                Environment.Exit(0);
                            }
                            //

                            // Create Recieving-Thread 
                            var t = new Thread(new ParameterizedThreadStart(Message.ReadClientMessage));
                            t.Start(Helper.NeighbourClients[neighbour]);
                            //
                        }
                        else
                        {
                            Console.WriteLine("Expected BoxQuery, got: " + msg);
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                }
            }

            // Check if any Values can already be determined
            box.DetermineValues();
            // 

            // Process Messages until the end of the world
            while (true)
            {
                Message msg;
                if (!Message.MessageQueue.TryDequeue(out msg))
                    continue;

                msg.process(box);
            }
            // 
        }

    }
}
