using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace NES
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"BattleCity.nes";
            if (!Cartridge.ReadFile(fileName))
            {
                Console.WriteLine("File not found");
            }
	}

	public static void DumpMem(byte[] a)
	{
	    int i = 0;
	    while (i < a.Length)
	    { 
		Console.Write ($"{i:X04}:\t");
		for (int j = 0; j < 16 && i < a.Length; j++, i++)
		    Console.Write($"{a[i]:X02} "); 
		Console.WriteLine();
	    }
	}
    }
}






