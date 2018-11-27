using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Message
    {
        public string Msg { get; private set; }
        public TcpClient Client { get; private set; }

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
    }
}
