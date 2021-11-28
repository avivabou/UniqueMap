using System;
using System.Collections;
using System.Collections.Generic;

namespace UniqueMap
{
    class Program
    {
        static void Main(string[] args)
        {
            Action[] examples = new Action[] { OrderRepeatExample, OrderNoRepeatExample, NoOrderNotBoundedRepeatsExample, NoOrderBoundedRepeatsExample, NoOrderNoRepeatExample };
            foreach (Action example in examples)
            {
                Console.WriteLine(example.Method.Name);
                Console.WriteLine("###################################################################################");
                example.Invoke();
            }
        }

        static private void OrderRepeatExample()
        {
            OrderRepeatMap<char> map = new OrderRepeatMap<char>(new char[] { 'x', 'c', 'a' });
            List<char[]> lis = new List<char[]>();

            lis.Add(new char[] { 'x' });
            lis.Add(new char[] { 'c' });
            lis.Add(new char[] { 'a' });
            lis.Add(new char[] { 'x', 'x', 'x', 'x', 'x', 'x' });
            lis.Add(new char[] { 'x', 'c' });
            lis.Add(new char[] { 'x', 'a' });
            lis.Add(new char[] { 'c', 'x' });
            lis.Add(new char[] { 'c', 'c' });
            lis.Add(new char[] { 'c', 'a' });
            lis.Add(new char[] { 'a', 'x' });
            lis.Add(new char[] { 'a', 'c' });
            lis.Add(new char[] { 'a', 'a' });

            for (int i = 0; i < lis.Count; i++)
            {
                Console.WriteLine(lis[i]);
                printBitArray(map.GetUniqueID(lis[i]));
                Console.WriteLine(map.GetUniqueMap(map.GetUniqueID(lis[i])));
                Console.WriteLine();
            }
        }

        static private void OrderNoRepeatExample()
        {
            OrderNoRepeatMap<ExampleDomain> map = new OrderNoRepeatMap<ExampleDomain>(new ExampleDomain[] { ExampleDomain.x, ExampleDomain.y, ExampleDomain.z });
            List<ExampleDomain[]> lis = new List<ExampleDomain[]>();
            lis.Add(new ExampleDomain[] { });
            lis.Add(new ExampleDomain[] { ExampleDomain.x });
            lis.Add(new ExampleDomain[] { ExampleDomain.x, ExampleDomain.y });
            lis.Add(new ExampleDomain[] { ExampleDomain.x, ExampleDomain.y, ExampleDomain.z });

            for (int i = 0; i < lis.Count; i++)
            {
                print(lis[i]);
                BitArray ID = map.GetUniqueID(lis[i]);
                printBitArray(ID);
                print(map.GetUniqueMap(ID));
                Console.WriteLine();
            }
        }

        static private void NoOrderNotBoundedRepeatsExample()
        {
            NoOrderRepeatExample();
        }

        static private void NoOrderBoundedRepeatsExample()
        {
            NoOrderRepeatExample(1000);
        }

        static private void NoOrderRepeatExample(uint bound = 0)
        {
            NoOrderRepeatMap<ExampleDomain> map;
            if (bound < 2)
                map = new NoOrderNotBoundedRepeatsMap<ExampleDomain>(new ExampleDomain[] { ExampleDomain.x, ExampleDomain.y, ExampleDomain.z });
            else
                map = new NoOrderBoundedRepeatsMap<ExampleDomain>(new ExampleDomain[] { ExampleDomain.x, ExampleDomain.y, ExampleDomain.z },bound);

            List<Dictionary<ExampleDomain,uint>> lis = new List<Dictionary<ExampleDomain, uint>>();
            lis.Add(new Dictionary<ExampleDomain, uint> { { ExampleDomain.x, 2 }, { ExampleDomain.y, 3 }, { ExampleDomain.z, 1 } });
            lis.Add(new Dictionary<ExampleDomain, uint> { { ExampleDomain.x, 2 }, { ExampleDomain.y, 3 }, { ExampleDomain.z, 5 } });
            lis.Add(new Dictionary<ExampleDomain, uint> { { ExampleDomain.x, 1 }, { ExampleDomain.y, 0 }, { ExampleDomain.z, 3 } });
            lis.Add(new Dictionary<ExampleDomain, uint> { { ExampleDomain.x, 18 }, { ExampleDomain.y, 0 }, { ExampleDomain.z, 3 } });
            lis.Add(new Dictionary<ExampleDomain, uint> { { ExampleDomain.x, 1 }, { ExampleDomain.y, 30 }, { ExampleDomain.z, 3 } });
            lis.Add(new Dictionary<ExampleDomain, uint> { { ExampleDomain.x, 1 }, { ExampleDomain.y, 410 }, { ExampleDomain.z, 3 } });
            lis.Add(new Dictionary<ExampleDomain, uint> { { ExampleDomain.x, 1 }, { ExampleDomain.y, 999 }, { ExampleDomain.z, 37 } });

            for (int i = 0; i < lis.Count; i++)
            {
                printDict(lis[i]);
                BitArray ID = map.GetUniqueID(lis[i]);
                printBitArray(ID);
                printDict(map.GetUniqueCountsDictionary(ID));
                Console.WriteLine();
            }
        }

        static private void NoOrderNoRepeatExample()
        {
            NoOrderNoRepeatMap<ExampleDomain> map = new NoOrderNoRepeatMap<ExampleDomain>(new ExampleDomain[] { ExampleDomain.x, ExampleDomain.y, ExampleDomain.z });
            List<ExampleDomain[]> lis = new List<ExampleDomain[]>();
            lis.Add(new ExampleDomain[] { });
            lis.Add(new ExampleDomain[] { ExampleDomain.x });
            lis.Add(new ExampleDomain[] { ExampleDomain.x, ExampleDomain.y });
            lis.Add(new ExampleDomain[] { ExampleDomain.x, ExampleDomain.y, ExampleDomain.z });

            for (int i = 0; i < lis.Count; i++)
            {
                print(lis[i]);
                BitArray ID = map.GetUniqueID(lis[i]);
                printBitArray(ID);
                print(map.GetUniqueMap(ID));
                Console.WriteLine();
            }
        }

        static private void printDict(Dictionary<ExampleDomain,uint> dict)
        {
            for (int i = 0; i < 3; i++)
                Console.Write(string.Format("[{0}:{1}] ", i, dict[(ExampleDomain)i]));
            Console.WriteLine();
        }

        static private void print<T>(T[] array)
        {
            foreach (T item in array)
                Console.Write(item);
            Console.WriteLine();
        }

        static private void printBitArray(BitArray arr)
        {
            for (int i = 0; i < arr.Length; i++)
                Console.Write(arr[i] ? 1 : 0);
            Console.WriteLine();

        }

        private enum ExampleDomain
        {
            x=0,y=1,z=2
        }
    }
}
