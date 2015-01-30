using System;
using System.Collections.Generic;
using System.Linq;
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
            int cmd = int.Parse(Console.ReadLine());
            if (cmd == 1)
            {
                CServer server = new CServer();
                server.Start();
            }
            else
            {
                CClient client = new CClient();
                client.start();
            }

            Console.ReadKey();
        }
    }    
}
