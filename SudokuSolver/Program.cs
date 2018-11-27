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

        private static ConcurrentDictionary<string, TcpClient> NeighbourClients = new ConcurrentDictionary<string, TcpClient>();
        private static ConcurrentDictionary<TcpClient,Thread> allClients = new ConcurrentDictionary<TcpClient, Thread>();

        private static ConcurrentQueue<Message> MessageQueue = new ConcurrentQueue<Message>();
        

        static void waitForConnections()
        {
            TcpListener listener = new TcpListener(IP,Port);
            listener.Start();

            while (true)
            {
                var c = listener.AcceptTcpClient();
                var t = new Thread(new ParameterizedThreadStart(readClientMessage));
                t.Start(c);
                allClients[c] = t;
            }
        }

        static void readClientMessage(Object obj)
        {
            TcpClient c = (TcpClient) obj;
            while (true)
            {
                if (c.GetStream().CanRead)
                {
                    byte[] bytes = new byte[c.ReceiveBufferSize];
                    c.GetStream().Read(bytes, 0, c.ReceiveBufferSize);
                    MessageQueue.Enqueue(new Message(Encoding.UTF8.GetString(bytes),c));
                }
            }
        }

        static void Main(string[] args)
        {
            string Name = args[0];
            
            

            Console.WriteLine("URI: tcp://"+IP.ToString()+":"+Port);
            Console.WriteLine("Name: "+Name);
            Console.WriteLine("Cells:");
            for (var j = 0; j < Cells.GetLength(0); j++)
            {
                for (var i = 0; i < Cells.GetLength(1); i++)
                {
                    Console.Write(Cells[i,j]+" ");
                }
                Console.WriteLine();
            }

            try
            {
                (ushort a, ushort b) = Helper.GetLocals(Name, "D5");
                Console.WriteLine(a + "," + b);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e);
            }
    
            Thread myThread = new Thread(new ThreadStart(waitForConnections));
            myThread.Start();

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
            
            Message.SendMessage(Manager, Name + "," + IP.ToString() + "," +
                                        Port);

            while (true)
            {
                Message msg;
                if (!MessageQueue.TryDequeue(out msg))
                    continue;

                if (msg.Equals("FEIERABEND"))
                {
                    //Do Feierabend-Stuff
                    continue;
                }


                Match m1 = Regex.Match(msg.Msg, "^(BOX_[A-I][1-9]),([0-2]),([0-2]):([1-9])$");
                Match m2 = Regex.Match(msg.Msg, "^(BOX_[A-I][1-9]),([A-I][1-9]):([1-9])$");
                if (m1.Success || m2.Success)
                {
                    string name;
                    ushort x, y;
                    ushort value;

                    if (m1.Success)
                    {
                        name = m1.Groups[1].Value;
                        x = UInt16.Parse(m1.Groups[2].Value);
                        y = UInt16.Parse(m1.Groups[3].Value);
                        value = UInt16.Parse(m1.Groups[4].Value);
                    } else {
                        name = m2.Groups[1].Value;
                        (x, y) = Helper.GetLocals(Name, m2.Groups[2].Value);
                        value = UInt16.Parse(m2.Groups[3].Value);
                    }

                    if (x < 2 && y < 2)
                    {
                        // Do Sodoku-Stuff
                    }
                    else
                    {
                        //Wrong/Unnecessary Rows or Colums send
                    }
                }

            }
        }

    }
}
