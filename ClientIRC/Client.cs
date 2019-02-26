using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Communication;

namespace ClientIRC
{
    class Client
    {
        private string hostname;
        private int port;
        private TcpClient comm = null;
        public Client(string host, int port)
        {
            this.hostname = host;
            this.port = port;
        }
        public void start()
        {
            Console.Clear();
            comm = new TcpClient(hostname, port);
            Console.WriteLine("Connection établie avec le serveur hôte : " + this.hostname + ":" + this.port);
            new Guest(comm);
        }
        
    }
}
