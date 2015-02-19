using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DatabasTest;

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
        List<string> wordList;
        TcpListener server;
        Thread ClientConnection;
        Thread gameThread;
        private bool gameRun = true;
        public string lastGuess = "";
        private string currentWord = "";
        private int currentWordPos = 0;
        private int guessTries = 11;
        private bool gameEnd = false;

        public void Start()
        {
            CSQL sql = new CSQL();
            wordList = new List<string>();

            if(!sql.start())
            {
                Console.WriteLine("Reverting to secondary list...");
                wordList.Add("gubbe");
                wordList.Add("dator");
                wordList.Add("nätverk");
            }

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
            gameThread = new Thread(new ThreadStart(gameListener));
            gameThread.Start();
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
                    break;
                }
            }
        }

        private void resetGame()
        {
            gameRun = true;
            lastGuess = "";
            currentWord = "";
            currentWordPos = 0;
            guessTries = 8;
            gameEnd = false;

            Random random = new Random();
            int r = random.Next(0, wordList.Count);
            currentWord = wordList[r];
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Current word is: " + currentWord);
            Console.ForegroundColor = ConsoleColor.Blue;
        }
        public void gameListener()
        {
            //Resetar för att nollställa allt och randomiza ord.
            resetGame();
            CTimer resetTimer = new CTimer(5000);
            while (gameRun)
            {
                if(gameEnd)
                {
                    if (resetTimer.isDone())
                    {
                        resetGame();
                    }
                    else if (lastGuess != "" && !resetTimer.isDone())
                    {
                        sendMessagetoClients("Server: Next game in " + resetTimer.timeLeft() / 1000 + " seconds.", null);
                        lastGuess = "";
                    }
                }
                else
                {
                    if (lastGuess != "")
                    {
                        if (lastGuess.Length == 1)
                        {
                            if (currentWord[currentWordPos] == lastGuess[0])
                            {
                                sendMessagetoClients("#%RIGHT%#", null);
                                currentWordPos++;
                                if (currentWordPos >= currentWord.Length)
                                    currentWordPos = currentWord.Length - 1;
                                Console.WriteLine("Right guess");
                            }
                            else
                            {
                                sendMessagetoClients("#%WRONG%#", null);
                                guessTries--;
                                Console.WriteLine("Wrong guess");
                            }
                        }
                        else if (lastGuess.Length > 1)
                        {
                            if (currentWord == lastGuess)
                            {
                                sendMessagetoClients("#%SUCCESS%#", null);
                                Console.WriteLine("Players win");
                                gameEnd = true;
                                resetTimer.reset(5000);
                                Console.WriteLine("Resetting game...");
                            }
                            else
                            {
                                //for (int i = 0; i < lastGuess.Length; i++)
                                //{
                                //    sendMessagetoClients("#%WRONG%#", null);
                                //    guessTries--;
                                //    Console.WriteLine("Wrong guess");
                                //}
                                sendMessagetoClients("#%WRONG%#", null);
                                guessTries--;
                            }
                        }
                        if (guessTries <= 0)
                        {
                            sendMessagetoClients("#%DEATH%#", null);
                            Console.WriteLine("Players failed");
                            gameEnd = true;
                            resetTimer.reset(5000);
                            Console.WriteLine("Resetting game...");
                        }

                        lastGuess = "";
                    }
                }
            }
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
                                //Removes the # from the line.
                                //line = line.Substring(1);
                                //Console.WriteLine(line);
                                string rest;
                                if (wordInString(line, "#%READY%#", out rest))
                                {
                                    sendMessagetoClients(client.nickname + " is ready for hängagubbe.", null);
                                    Console.WriteLine(client.nickname + " is ready for hängagubbe.");
                                    client.gameReady = true;
                                }
                                else if (wordInString(line, "#%GUESS%#", out rest) && lastGuess.Length == 0)
                                {
                                    if(rest.Length > 0)
                                    {
                                        Console.WriteLine("Guess:" + rest);
                                        rest = rest.ToLower();
                                        lastGuess = rest;
                                    }
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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Failed to read message from client");
                Console.ForegroundColor = ConsoleColor.Blue;
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
        public bool wordInString(string line, string word, out string rest)
        {
            rest = "";
            bool foundWord = false;
            int startRemovePos = 0;

            if (word.Length > 0 && line.Length > 0)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if(line[i] == word[0])
                    {
                        if (i + word.Length <= line.Length)
                        {
                            for (int w = 0; w < word.Length; w++)
                            {
                                foundWord = true;
                                if (line[w + i] != word[w])
                                {
                                    foundWord = false;
                                    break;
                                }
                                else
                                    startRemovePos = i + w + 1;
                            }
                        }
                    }
                    if (foundWord)
                        break;
                }
            }

            if (foundWord)
            {
                rest = line.Substring(startRemovePos, line.Length - startRemovePos);
                rest = rest.Trim();
            }
            return foundWord;
        }
    }
}