using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PYS.Lib
{
    public class PYSSample
    {

    }

    public class ThreadTest
    {
        public void RunMe()
        {
            Console.WriteLine("Runme Called.");
        }

        public static void Start()
        {
            ThreadTest threadTest = new ThreadTest();
            Thread thread = new Thread(threadTest.RunMe);
            thread.Start();
        }
    }
}
