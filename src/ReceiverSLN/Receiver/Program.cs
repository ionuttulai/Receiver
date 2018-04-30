using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Receiver
{
    class Program
    {
        private static long _seed;

        static void Main(string[] args)
        {
            //obtain current state from sender on initial handshake;
            _seed = 1024;

            //TestProcessor2(500);
            //Test(500);
            TestParallel(1000);

            //this should be a long running process -> keep the main thread running for async test
            Thread.Sleep(5000);
        }


        public static void TestProcessor2(int n)
        {
            var arr = new long[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = _seed + i + 1;
            }

            var rnd = new Random();

            using (var processor = new Processor2(_seed))
            {
                //receive packages in random order
                foreach (var sequenceReceived in arr.OrderBy(x => rnd.Next()))
                {
                    Debug.WriteLine($"Received {sequenceReceived}");
                    processor.HandlePackage(new Object(), sequenceReceived);
                }
            }
        }

        public static void Test(int n)
        {
            var arr = new long[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = _seed + i + 1;
            }

            var rnd = new Random();

            using (var processor = new Processor(_seed))
            {
                //receive packages in random order
                foreach (var sequenceReceived in arr.OrderBy(x => rnd.Next()))
                {
                    Debug.WriteLine($"Received {sequenceReceived}");
                    processor.HandlePackage(new Object(), sequenceReceived);
                }
            }
        }


        public static void TestParallel(int n)
        {
            var arr = new long[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = _seed + i + 1;
            }

            var rnd = new Random();

            using (var processor = new Processor(_seed))
            {
                //receive packages in random order in parallel
                Parallel.ForEach(arr.OrderBy(x => rnd.Next()), new ParallelOptions() {MaxDegreeOfParallelism = 5},
                    sequenceReceived =>
                    {
                        Debug.WriteLine($"Received {sequenceReceived}");
                        processor.HandlePackage(new Object(), sequenceReceived);
                    });
            }
        }

    }
}
