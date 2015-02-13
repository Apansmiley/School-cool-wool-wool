using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace Test
{
    class CServer
    {
        public class SClient
        {
            public TcpClient tcp;
            public string nickname;
            bool closed;
            public void setClosed(bool c) { closed = c; }
            public bool isClosed() { return closed; }

            public bool gameReady = false;
        }
        List<SClient> clientList;
        TcpListener server;
        Thread ClientConnection;


        public void Start()
        {
            clientList = new List<SClient>();
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
                string nickname = "";
                try
                {
                    while (nickname.Length == 0)
                    {
                        Byte[] bytes = new Byte[client.Available];
                        if (client.GetStream().Read(bytes, 0, bytes.Length) > 0)
                            nickname = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
                    }

                    Console.WriteLine("Client " + nickname + " connected.");
                    SClient newClient = new SClient();
                    newClient.tcp = client;
                    newClient.nickname = nickname;
                    clientList.Add(newClient);
                    var threading = new Thread(() => checkIncomingMessage(newClient));
                    threading.Start();
                    sendMessagetoClients(nickname + " joined the club.", null);
                }
                catch
                {
                    Console.WriteLine("Failed to get nickname");
                }
            }
        }

        public void gameThread()
        {
            
            
        }

        public void checkIncomingMessage(SClient client)
        {
            TcpClient newClient = client.tcp;
            try
            {
                while (!client.isClosed())
                {
                    NetworkStream stream = newClient.GetStream();
                    if (stream != null)
                    {
                        while (!stream.DataAvailable) ;
                        Byte[] bytes = new Byte[newClient.Available];
                        stream.Read(bytes, 0, bytes.Length);
                        string line = Encoding.Unicode.GetString(bytes, 0, bytes.Length);

                        if (line.Length > 0)
                        {
                            if (line[0] == '#')
                            {
                                Console.WriteLine(line);
                                if (line == "#ready")
                                {
                                    sendMessagetoClients(client.nickname + " is ready for hängagubbe.", null);
                                    Console.WriteLine(client.nickname + " is ready for hängagubbe.");
                                    client.gameReady = true;
                                }
                            }
                            else
                            {
                                line = client.nickname;
                                line += ": ";
                                line += Encoding.Unicode.GetString(bytes, 0, bytes.Length);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(line);
                                sendMessagetoClients(line, null);
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                        }

                    }
                }
            }
            catch
            {
                Console.WriteLine("Failed to read message from client");
            }
        }
        public void sendMessagetoClients(string line, TcpClient exclude)
        {
            string newLine = line;
            int pos = 0;
            foreach (SClient c in clientList)
            {
                if (c.tcp == exclude)
                {
                    newLine = c.nickname;
                    newLine += ": ";
                    newLine += line;
                }
                pos++;
            }
            byte[] byteBuffer = Encoding.Unicode.GetBytes(newLine);
            List<SClient> toRemove = new List<SClient>();
            foreach (SClient c in clientList)
            {
                try
                {
                    if (c.tcp != exclude)
                    {
                        c.tcp.GetStream().Write(byteBuffer, 0, byteBuffer.Length);
                        c.tcp.GetStream().Flush();
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Client " + c.nickname + " disconnected...");
                    toRemove.Add(c);
                }
            }
            //Removes disconnected clients.
            foreach (SClient c in toRemove)
            {
                c.setClosed(true);
                c.tcp.Close();
                clientList.Remove(c);
            }
        }
    }
}