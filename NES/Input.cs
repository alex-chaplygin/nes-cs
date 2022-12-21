using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    public enum Key 
    {
        A,
        B,
        Select,
        Start,
        Up,
        Down,
        Left,
        Right
    }
    
    internal class Input
    {
	const ushort CONTROLER1 = 0x4016;
	
	static byte keys = 0;
	static byte read_keys;
	
        public static byte Read(ushort adr)
        {
	    if (adr == CONTROLER1)
            {
                int i = read_keys & 1;
                read_keys >>= 1;
                return (byte)i;
            }
            return 0;
        }
	
        public static void Write(ushort adr, byte value)
        {
	    if(adr == CONTROLER1 && (value & 1) == 1)
		read_keys = keys;
        }

	public static void KeyDown(Key key)
        {
            keys = (byte)(keys | (byte)(1 << (int)key));
        }
	
        public static void KeyUp(Key key)
        {
            byte temp = (byte)(1 << (int)key);
            keys = (byte)(keys & ~temp);
        }
    }
}
