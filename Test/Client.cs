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
            String server = "127.0.0.1"; // Server name or IP address  
            string message = "Hello";

            // Convert input String to bytes  
            byte[] byteBuffer = Encoding.ASCII.GetBytes(message);

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
                ns.Write(byteBuffer, 0, byteBuffer.Length);

                Console.WriteLine("Sent {0} bytes to server...", byteBuffer.Length);

                int totalBytesRcvd = 0; // Total bytes received so far  
                int bytesRcvd = 0; // Bytes received in last read  

                // Receive the same string back from the server  
                while (totalBytesRcvd < byteBuffer.Length)
                {
                    if ((bytesRcvd = ns.Read(byteBuffer, totalBytesRcvd,
                    byteBuffer.Length - totalBytesRcvd)) == 0)
                    {
                        Console.WriteLine("Connection closed prematurely.");
                        break;
                    }
                    totalBytesRcvd += bytesRcvd;
                }
                Console.WriteLine("Received {0} bytes from server: {1}", totalBytesRcvd,
                 Encoding.ASCII.GetString(byteBuffer, 0, totalBytesRcvd));

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
