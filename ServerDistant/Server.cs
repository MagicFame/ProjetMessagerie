using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using Communication;
using System.Xml.Serialization;

namespace ServerDistant
{
    class Server
    {
        private int port;
        public Server(int port)
        {
            this.port = port;
        }
        public void start()
        {
            TcpListener l = new TcpListener(new IPAddress(new byte[] { 127, 0, 0, 1 }), port);
            l.Start();

            while (true)
            {
                TcpClient comm = l.AcceptTcpClient();
                Console.WriteLine("Connection established @" + comm);
                new Thread(new Receiver(comm).doOperation).Start();
            }
        }
    }
    class Receiver
    {
        private TcpClient comm;

        public Receiver(TcpClient s)
        {
            comm = s;
        }
        public void doOperation()
        {
            new Guests(comm);
        }
        

    }
}