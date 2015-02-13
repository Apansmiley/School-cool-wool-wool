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
        List<ClientId> clientList = new List<ClientId>(); // Listan där clienter och nicknames ska finnas
        TcpListener server;
        Thread ClientConnection;
       public class ClientId // här är variablen för nickname
        {
           public TcpClient client;
           public string nickname;
           public int id;
        }
    

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
          

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                string message = "Server: " + Console.ReadLine();
                sendMessagetoClients(message, null);

            }

        }

        public void Clientconnected()  // när en client connectar
        {
            while (true)
            {
                ClientId newClient = new ClientId();
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connection accepted.");
              string Nickname = "";
              while (Nickname.Length == 0)
              {
                  Byte[] bytes = new Byte[client.Available];

                  client.GetStream().Read(bytes, 0, bytes.Length);
                  Nickname = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
              }
                Console.ForegroundColor = ConsoleColor.Red;
                newClient.nickname = Nickname;
                newClient.id = clientList.Count+1;
                newClient.client = client;
                clientList.Add(newClient);
                Console.WriteLine("Client " + Nickname+ " Connected");
                var threading = new Thread(() => checkIncomingMessage(client));
                threading.Start();
            }
        }

       
        public void checkIncomingMessage(TcpClient newClient)
        {
            try
            {
                while (newClient.Connected)
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
                        sendMessagetoClients(line, newClient);
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                }
            }
            catch (Exception r)
            {

            }
        }
        public void sendMessagetoClients(string line, TcpClient exclude)
        {
            string newline = line;
            if ( exclude != null)
            {
               foreach (ClientId c in clientList) 
               {
                   
                   if (c.client == exclude)
                   {
                       newline = c.nickname;
                       newline += ": ";
                       newline += line;
                       
                   }
               }
            }

            byte[] byteBuffer = Encoding.Unicode.GetBytes(newline);
            
            List<ClientId> toRemove = new List<ClientId>();

            foreach (ClientId c in clientList)
            {
                try
                {
                    if (c.client != exclude)
                    {

                        c.client.GetStream().Write(byteBuffer, 0, byteBuffer.Length);
                        c.client.GetStream().Flush();
                    } 
                }
                catch(Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Client disconnected...");
                    toRemove.Add(c);
                }
            }

           // Console.ForegroundColor = ConsoleColor.Red;

            //Removes disconnected clients.
            foreach (ClientId c in toRemove)
            {
                clientList.Remove(c);
                c.client.Close();
            }
        }
    }


}

