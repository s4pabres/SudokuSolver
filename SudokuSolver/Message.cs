using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Message
    {
        public string Msg { get; private set; }
        public TcpClient Client { get; private set; }
        public static ConcurrentQueue<Message> MessageQueue { get; private set; } = new ConcurrentQueue<Message>();

        public Message(string msg, TcpClient client)
        {
            Msg = msg;
            Client = client;
        }

        public static void SendMessage(TcpClient Client, string message)
        {
            byte[] bytes =
                Encoding.ASCII.GetBytes(message);
            Client.GetStream().Write(bytes, 0, bytes.Length);
        }

        public static void ReadClientMessage(Object obj)
        {
            TcpClient c = (TcpClient)obj;
            while (true)
            {
                if (c.GetStream().CanRead)
                {
                    byte[] bytes = new byte[c.ReceiveBufferSize];
                    c.GetStream().Read(bytes, 0, c.ReceiveBufferSize);
                    MessageQueue.Enqueue(new Message(Encoding.ASCII.GetString(bytes), c));
                }
            }
        }

        public void process(Box box)
        {
            if (Msg.Equals("FEIERABEND"))
            {
                foreach (var neighbourClient in Helper.NeighbourClients.Values)
                {
                    SendMessage(neighbourClient,"FEIERABEND");
                }
                Console.WriteLine("FEIERABEND");
                Environment.Exit(0);
            }

            Match m1 = Regex.Match(Msg, "^(BOX_[A-I][1-9]),([0-2]),([0-2]):([1-9])$");
            Match m2 = Regex.Match(Msg, "^(BOX_[A-I][1-9]),([A-I][1-9]):([1-9])$");
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
                }
                else
                {
                    name = m2.Groups[1].Value;
                    (x, y) = Helper.GetLocals(box.Name, m2.Groups[2].Value);
                    value = UInt16.Parse(m2.Groups[3].Value);
                }

                if (x <= 2 && y <= 2)
                {
                    box.RemoveValue(x, y, value);
                }
                else
                {
                    Console.WriteLine("Row/Columm not inside Box");
                }
            }
        }
    }
}
