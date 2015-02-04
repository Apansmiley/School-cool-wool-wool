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
        List<TcpClient> ClientList;
        TcpListener server;
        Thread ClientConnection;
        NetworkStream stream2;
        Thread ClientMessage;
        TcpClient client;
        Thread ClientRead;
        public void Start()
        {
     
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
            //ClientRead = new Thread(new ThreadStart(Getmessage));
           
           //ClientRead.Start(stream);
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                string message = Console.ReadLine();
                byte[] byteBuffer = Encoding.ASCII.GetBytes(message);
                foreach (TcpClient c in ClientList)
                {

                    c.GetStream().Write(byteBuffer, 0, byteBuffer.Length);
                }
            }

        } 

        public void Clientconnected()
          {
            while (true)
            {
                client = server.AcceptTcpClient();
                Console.WriteLine("Connection accepted.");
                ClientList.Add(client);
                var threading = new Thread(() => Getmessage(client.GetStream()));
                threading.Start();
            }
        }

        public void Clientmessage()
        {

            while (true)
            {
               
            }
        }
        public void Getmessage(NetworkStream stream)
        {
            while (true)
            {
                if (stream != null)
                {
                    while (!stream.DataAvailable) ;
                    Byte[] bytes = new Byte[client.Available];

                    stream.Read(bytes, 0, bytes.Length);
                    string line = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(line);
                }
            }
          }
        }


}

