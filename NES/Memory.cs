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
        public const int PRG_LENTGH = 0x4000;
        public const int CHR_LENTGH = 0x2000;
        public const int TRN_LENGTH = 0x200;

	static byte[] memory = new byte[MEM_SIZE];

	// Алексеев Андрей
	public static byte Read(ushort adr)
	{
	    return 0;
	}

	// Киселев Николай
	public static void Write(ushort adr, byte val)
	{
	}

	// Геворкян Арнольд
	public static void WriteROM1(byte[] bank)
	{
	}
    }
}
