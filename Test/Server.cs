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
        TcpListener server;
        Thread ClientConnection;
        NetworkStream stream;
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



        }

        public void Clientconnected()
        {
            while (true)
            {
                client = server.AcceptTcpClient();
                Console.WriteLine("Connection accepted.");
                stream = client.GetStream();
                var threading = new Thread(() => Getmessage(stream));
                threading.Start();
            }
        }

        public void Clientmessage()
        {

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                string message = Console.ReadLine();
                byte[] byteBuffer = Encoding.ASCII.GetBytes(message);
                stream.Write(byteBuffer, 0, byteBuffer.Length);
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

