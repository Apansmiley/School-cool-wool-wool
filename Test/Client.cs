using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Test
{
    class CClient
    {
        private TcpClient client = null;
        private NetworkStream stream = null;
        private Thread thread2 = null;
       // private bool connectionLost = false;

        public void checkForServerResponse()
        {
            try
            {
                while (true)
                {
                   // Console.WriteLine("Waiting for message from server...");
                    Byte[] byteBuffer = new Byte[100];
                    if (stream.Read(byteBuffer, 0, byteBuffer.Length) > 0)
                    {
                        string line = Encoding.Unicode.GetString(byteBuffer, 0, byteBuffer.Length);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(line);
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void start()
        {
            //String server = "94.254.65.11";
            //String server = "172.22.212.225";
            //String server = "192.168.150.1";
            String server = "172.22.212.132";

            // Use port argument if supplied, otherwise default to 7  
            int servPort = 80;

            try
            {
                Console.WriteLine("Trying to connect to server...");
                // Create socket that is connected to server on specified port  
                client = new TcpClient(server, servPort);

                if (client.Connected)
                {
                    Console.WriteLine("Connected to server......");
                    thread2 = new Thread(new ThreadStart(checkForServerResponse));
                    thread2.Start();
                }
                else
                {
                    Console.WriteLine("Failed to connect to server");
                    Console.ReadKey();
                    return;
                }

                stream = client.GetStream();

                while (true)
                {

                    //Console.WriteLine("Commands: Send, Exit.");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    string message = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.Red;

                    Byte[] byteBuffer = Encoding.Unicode.GetBytes(message);
                    //Send the encoded string to the server  
                    if(client.Connected)
                        stream.Write(byteBuffer, 0, byteBuffer.Length);
                    else
                    {
                        Console.WriteLine("Connection lost...");
                        Console.ReadKey();
                        break;
                    }
                    //Console.WriteLine("Message " + message + " was sent to the server.");

                    if (message == "exit" || message == "Exit")
                    {
                        break;
                    }
                    //Console.ForegroundColor = ConsoleColor.Blue;
                    //string message = Console.ReadLine();
                    //Byte[] byteBuffer = Encoding.ASCII.GetBytes(message);
                    //stream.Write(byteBuffer, 0, byteBuffer.Length);

                    //Byte[] bytes = new Byte[client.Available];
                    //byteBuffer = new Byte[100];

                    //stream.Read(byteBuffer, 0, byteBuffer.Length);
                    //string line = Encoding.ASCII.GetString(byteBuffer, 0, byteBuffer.Length);
                    //Console.ForegroundColor = ConsoleColor.Red;
                    //Console.WriteLine(line);

                    //string line = Console.ReadLine();
                    //byte[] byteBuffer = Encoding.ASCII.GetBytes(line);
                    //ns.Write(byteBuffer, 0, byteBuffer.Length);

                    //byteBuffer = new Byte[client.Available];

                    //ns.Read(byteBuffer, 0, byteBuffer.Length);
                    //line = Encoding.ASCII.GetString(byteBuffer, 0, byteBuffer.Length);
                    //Console.WriteLine(line);
                }
                //stream.Close();
                //client.Close();
                //thread2.Abort();
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            finally
            {
                stream.Close();
                client.Close();
                thread2.Abort();
            } 
        }
    }
}
