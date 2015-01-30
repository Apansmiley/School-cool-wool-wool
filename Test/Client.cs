using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Test
{
    class CClient
    {
        public void start()
        {
            String server = "94.254.65.11"; // Server name or IP address  


            // Use port argument if supplied, otherwise default to 7  
            int servPort = 80;

            TcpClient client = null;
            NetworkStream ns = null;

            try
            {
                // Create socket that is connected to server on specified port  
                client = new TcpClient(server, servPort);

                Console.WriteLine("Connected to server......");

                ns = client.GetStream();

                // Send the encoded string to the server  
               // ns.Write(byteBuffer, 0, byteBuffer.Length);

              //  Console.WriteLine("Sent {0} bytes to server...", byteBuffer.Length);

                // Receive the same string back from the server  
                while (true)
                {

                    string line = Console.ReadLine();
                    byte[] byteBuffer = Encoding.ASCII.GetBytes(line);
                    ns.Write(byteBuffer, 0, byteBuffer.Length);

                    byteBuffer = new Byte[client.Available];

                    ns.Read(byteBuffer, 0, byteBuffer.Length);
                    line = Encoding.ASCII.GetString(byteBuffer, 0, byteBuffer.Length);
                    Console.WriteLine(line);
                }
               

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                ns.Close();
                client.Close();
            } 
        }
    }
}
