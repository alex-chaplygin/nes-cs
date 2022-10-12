using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NES
{
    public class Cartridge
    {
        public static int prg_count;
        public static int chr_count;
        public static bool mirroring;
        public static bool trainer;
        public static bool prg_ram;
        public static bool ignore_mirroring;
        public static int mapper;
        static byte[] prg_mem;
        static byte[] chr_mem;
        static byte[] trainer_mem;

        public static byte[] GetPrgBank(int n)
        {
            return prg_mem.Skip<byte>(n * Memory.PRG_LENTGH).
		Take(Memory.PRG_LENTGH).ToArray();
        }

        public static bool ReadFile(string fileName)
        {
	    byte[] Header = { 0x4E, 0x45, 0x53, 0x1A };
            byte[] header;
            using (var stream = File.Open(fileName, FileMode.Open))
	    {
		using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
		{
		    header = reader.ReadBytes(16);
		    for (int i = 0; i < 4; i++)
			if (header[i] != Header[i])
			    return false;
                    prg_count = header[4];
                    prg_mem = new byte[prg_count * Memory.PRG_LENTGH];
                    chr_count = header[5];
                    chr_mem = new byte[chr_count * Memory.CHR_LENTGH];
                    mirroring = (header[6] & 0x01) != 0;
                    prg_ram = (header[6] & 0x02) != 0;
                    trainer = (header[6] & 0x04) != 0;
                    ignore_mirroring = (header[6] & 0x08) != 0;
		    // trainer ???
                    prg_mem = reader.ReadBytes(prg_count * Memory.PRG_LENTGH);
		    // chr_mem ???
                }                                            
            }
            return true;
        }
    }
}
