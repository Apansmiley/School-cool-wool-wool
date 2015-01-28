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
            string text= "#";
            while (true)
            {

                if (timer.isDone())
                {
                    
                    Console.Write(text);
                    timer.reset(1000);
                }
            }

          
        }
    }    
}
