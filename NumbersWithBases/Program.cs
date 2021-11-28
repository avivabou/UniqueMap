using System;
using System.Collections;

namespace NumbersWithBases
{
    class Program
    {
        static void Main(string[] args)
        {
            DigitNode d = new DigitNode(3, 8);
            d.AppendPrev(new DigitNode(1, 8));
            printBitArray(d.ToBinaryBitArray());
            Console.WriteLine(DigitNode.FromBinary(d.ToBinaryBitArray(),4));
            /*Console.WriteLine(d.Rest);
            printBitArray(d.ToBinary());
            Console.WriteLine(Digit.FromBinary(new BitArray(10,true), 10)); // 256 + 64 + 4 + 2 = 326*/
            Console.ReadLine();
        }

        static private void printBitArray(BitArray arr)
        {
            for (int i = 0; i < arr.Length; i++)
                Console.Write(arr[i]? 1: 0);
            Console.WriteLine();
        }
    }
}
