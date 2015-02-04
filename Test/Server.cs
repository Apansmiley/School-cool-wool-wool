using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Test
{
    class CServer
    {
        TcpListener server;
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
            Console.WriteLine("Server has started on: " + LocalIp +  "{0}Waiting for a connection...", Environment.NewLine);
        } 

        public void Clientconnected ()
        {
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connection accepted.");
            NetworkStream stream = client.GetStream();

            var childTcpclientThread = new Thread(() =>
            {
                // while (!stream.DataAvailable) ;
                Byte[] bytes = new Byte[client.Available];

                stream.Read(bytes, 0, bytes.Length);
                string line = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(line);
                    
            });
                

                                
            Console.ForegroundColor = ConsoleColor.Blue;
            string message = Console.ReadLine();
            byte[] byteBuffer = Encoding.ASCII.GetBytes(message);
            stream.Write(byteBuffer, 0, byteBuffer.Length);
               
            childTcpclientThread.Start();
        }
        }

         
    }
}
