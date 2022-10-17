using System;
using System.Threading;

namespace OS_Problem_02
{
    class Thread_safe_buffer
    {
        static int[] TSBuffer = new int[10];
        static int Front = 0;
        static int Back = 0;
        static int Count = 0;
        static object _Lock = new object();
        static Semaphore s = new Semaphore(0, 2);
        static bool isEnd = false;

        static void EnQueue(int eq)
        {
            TSBuffer[Back] = eq;
            Back++;
            Back %= 10;
            Count += 1;
        }

        static int DeQueue()
        {
            if (Count == 0)
                return -1;
            int x = 0;
            x = TSBuffer[Front];
            Front++;
            Front %= 10;
            Count -= 1;
            return x;
        }

        static void th01()
        {
            int i;

            for (i = 1; i < 51; i++)
            {
                lock (_Lock) {
                    while (Count >= 10){
                        Monitor.Wait(_Lock);
                    }
                    EnQueue(i);
                    Thread.Sleep(5);
                    if (Count != 0) {
                        // s.Dispose();
                        // Console.Write("s={0}, ", s);
                        try {
                            s.Release();
                        }
                        catch {

                        }
                    }
                }
            }
            while (Count > 0){
                try {
                    s.Release();
                }
                catch {

                }
            }
            isEnd = true;
        }

        static void th011()
        {
            int i;

            for (i = 100; i < 151; i++)
            {
                lock (_Lock) {
                    while (Count >= 10){
                        Monitor.Wait(_Lock);
                    }
                    EnQueue(i);
                    Thread.Sleep(5);
                    if (Count != 0) {
                        // s.Dispose();
                        // Console.Write("s={0}, ", s);
                        try {
                            s.Release();
                        }
                        catch {

                        }
                    }
                }
            }
            while (Count > 0){
                try {
                    s.Release();
                }
                catch {

                }
            }
        }

        static void th02(object t)
        {
            int i;
            int j;

            for (i=0; i < 60; i++)
            {
                if (!isEnd)
                    s.WaitOne();
                lock (_Lock) {
                    j = DeQueue();
                    if (j == -1)
                        return;
                    Console.WriteLine("j={0}, thread:{1}", j, t);
                    Thread.Sleep(100);
                    Monitor.Pulse(_Lock);
                }
            }
        }
        static void Main(string[] args)
        {
            Thread t1 = new Thread(th01);
            Thread t11 = new Thread(th011);
            Thread t2 = new Thread(()=>th02(1));
            Thread t21 = new Thread(()=>th02(2));
            Thread t22 = new Thread(()=>th02(3));

            t1.Start();
            t11.Start();
            t2.Start();
            t21.Start();
            t22.Start();
        }
    }
}
