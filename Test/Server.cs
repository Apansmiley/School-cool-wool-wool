using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Test
{
    class CServer
    {
        public void Start()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("172.22.212.225"), 80);

            server.Start();
            Console.WriteLine("Server has started on 172.22.212.225:80.{0}Waiting for a connection...", Environment.NewLine);

            TcpClient client = server.AcceptTcpClient();

            Console.WriteLine("A client connected.");

            NetworkStream stream = client.GetStream();

            //enter to an infinite cycle to be able to handle every change in stream
            while (true)
            {
                while (!stream.DataAvailable) ;

                Byte[] bytes = new Byte[client.Available];

                stream.Read(bytes, 0, bytes.Length);
                string line = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(line);
                Console.ForegroundColor = ConsoleColor.White;
                string message = Console.ReadLine();
                byte[] byteBuffer = Encoding.ASCII.GetBytes(message);
                stream.Write(byteBuffer, 0, byteBuffer.Length);
            }
        }
    }
}
