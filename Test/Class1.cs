﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Class1
    {
        string text = "This is class1";
        public void Text()
        {
            Console.WriteLine(text);
        }
    }
    class TimerExample
    {
        static public void Tick(Object stateInfo)
        {
            Console.WriteLine("Tick: {0}", DateTime.Now.ToString("h:mm:ss"));
        }
    }
    class Nobrainer
    {

    }
}
