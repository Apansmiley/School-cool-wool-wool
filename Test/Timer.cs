using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Test
{
    class CTimer
    {
        Stopwatch watch;
        long stopTime;
        public CTimer(long ms)
        {
            watch = new Stopwatch();
            watch.Start();
            reset(ms);
        }
        public void reset(long ms)
        {
            stopTime = watch.ElapsedMilliseconds + ms;
        }
        public bool isDone()
        {
            if (stopTime <= watch.ElapsedMilliseconds)
                return true;
            return false;
        }
    }
}
