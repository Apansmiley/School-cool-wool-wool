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
            TimerExample time = new TimerExample(Tick);
            TimerCallback callback = new TimerCallback(Tick);

            Console.WriteLine("Creating timer: {0}\n",
                               DateTime.Now.ToString("h:mm:ss"));

            // create a one second timer tick
            Timer stateTimer = new Timer(callback, null, 0, 1000);

            // loop here forever
            for (; ; ) { }
        }
    }
    class class3
    {

    }
}
