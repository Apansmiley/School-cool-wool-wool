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
            CTimer timer = new CTimer(1000);

            while (true)
            {
                if (timer.isDone())
                {
                    Console.WriteLine("Still going");
                    timer.reset(1000);
                }
            }

            DateTime Today = DateTime.Today;
            Console.WriteLine("Hello World!");
            Console.WriteLine(Today.Date);
            Console.ReadKey();
        }
    }
    class class3
    {

    }
}
