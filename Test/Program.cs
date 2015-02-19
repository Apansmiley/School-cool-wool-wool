using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Test;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose if you wish to start the server or client.");
            Console.WriteLine("To choose server type 1 or if you wish to start client type 2");
            int cmd = 0;
            if (int.TryParse(Console.ReadLine(), out cmd))
            {
                switch (cmd)
                {
                    case 1:            
                    CServer server = new CServer();
                    server.Start();
                    break;
            
                    case 2:
                    CClient client = new CClient();
                    client.start();
                    break;
                    case 3:
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                    
                    
                    break;
                    default:
                    Console.WriteLine("You didn't choose a correct option... Good night");
                    break;
                }
            }
        }
    }    
}
