using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    public class Memory
    {
	const int MEM_SIZE = 0x10000;
        public const int PRG_SIZE = 0x4000;
        public const int CHR_SIZE = 0x2000;
        public const int RAM_SIZE = 0x800;
        public const int TRN_SIZE = 0x200;

	public static byte[] memory = new byte[MEM_SIZE];

	// Алексеев Андрей
	public static byte Read(ushort adr)
	{
	    while (adr >= RAM_SIZE)
                adr -= RAM_SIZE;
            return memory[adr];
	}

	// Киселев Николай
	public static void Write(ushort adr, byte val)
	{
	    memory[adr] = val;
	}

	// Геворкян Арнольд
	public static void WriteROM1(byte[] bank)
	{
	}
    }
}
