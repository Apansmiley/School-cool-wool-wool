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
      //  TcpClient client;

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
            //ClientMessage = new Thread(new ThreadStart(Clientmessage));
            //ClientMessage.Start();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                string message = "Server: " + Console.ReadLine();
                sendMessagetoClients(message, null);

            }

        }

        public void Clientconnected()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connection accepted.");
                clientList.Add(client);
                var threading = new Thread(() => checkIncomingMessage(client));
                threading.Start();
            }
        }

        //TODO: fixa så att den kan kolla om klienten är connected eller inte.
        public void checkClientConnection(TcpClient tcp)
        {
            if (tcp.Client.Poll(0, SelectMode.SelectWrite))
            {
                byte[] buff = new byte[1];
                if (!tcp.Connected)
                    Console.WriteLine("Disconnected...");

                //byte[] buff = new byte[1];
                //if(tcp.Client.Receive(buff, SocketFlags.Peek) == 0)
                //{
                //    Console.WriteLine("Client disconnected");
                //}
            }
                
        }

        //public void Clientmessage()
        //{

        //    while (true)
        //    {

        //    }
        //}
        public void checkIncomingMessage(TcpClient newClient)
        {
            while (true)
            {
                NetworkStream stream = newClient.GetStream();
                if (stream != null)
                {
                    while (!stream.DataAvailable) ;
                    Byte[] bytes = new Byte[newClient.Available];

                    stream.Read(bytes, 0, bytes.Length);
                    string line = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(line);
                    sendMessagetoClients("Client: "+ line, newClient);
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
            }
        }
        public void sendMessagetoClients(string line, TcpClient exclude)
        {
            //Kollar om alla klienter är connectade.
            foreach (TcpClient c in clientList)
            {
                checkClientConnection(c);
            }

            byte[] byteBuffer = Encoding.Unicode.GetBytes(line);

            foreach (TcpClient c in clientList)
            {
                if (c != exclude)
                {
                    c.GetStream().Write(byteBuffer, 0, byteBuffer.Length);
                } Console.ForegroundColor = ConsoleColor.Red;
            }
        }
    }


}

