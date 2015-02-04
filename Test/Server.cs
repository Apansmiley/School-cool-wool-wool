using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class CServer
    {
        List<TcpClient> clientList;
        TcpListener server;
        Thread ClientConnection;

        Thread ClientMessage;
        TcpClient client;

        public void Start()
        {
            clientList = new List<TcpClient>();

            IPHostEntry host;
            string LocalIp = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    LocalIp = ip.ToString();
                    break;
                }

            }
            server = new TcpListener(IPAddress.Parse(LocalIp), 80);

            server.Start();
            Console.WriteLine("Server has started on: " + LocalIp + "{0}Waiting for a connection...", Environment.NewLine);
            ClientConnection = new Thread(new ThreadStart(Clientconnected));
            ClientConnection.Start();
            ClientMessage = new Thread(new ThreadStart(Clientmessage));
            ClientMessage.Start();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                string message = "Server: " + Console.ReadLine();
                ReturnMessagetoClients(message, null);
            }

        }

        public void Clientconnected()
        {
            while (true)
            {
                client = server.AcceptTcpClient();
                Console.WriteLine("Connection accepted.");
                clientList.Add(client);
                var threading = new Thread(() => Getmessage(client));
                threading.Start();
            }
        }

        public void Clientmessage()
        {

            while (true)
            {

            }
        }
        public void Getmessage(TcpClient newClient)
        {
            while (true)
            {
                NetworkStream stream = newClient.GetStream();
                if (stream != null)
                {
                    while (!stream.DataAvailable) ;
                    Byte[] bytes = new Byte[newClient.Available];

                    stream.Read(bytes, 0, bytes.Length);
                    string line = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(line);
                    ReturnMessagetoClients("Client: "+ line, newClient);
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
            }
        }
        public void ReturnMessagetoClients(string line, TcpClient exclude)
        {
            

            byte[] byteBuffer = Encoding.ASCII.GetBytes(line);

            foreach (TcpClient c in clientList)
            {
                if (c != null && c != exclude)
                    c.GetStream().Write(byteBuffer, 0, byteBuffer.Length);
                Console.ForegroundColor = ConsoleColor.Red;
            }
        }
    }


}

