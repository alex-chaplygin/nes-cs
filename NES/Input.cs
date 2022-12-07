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
	static byte keys = 0;
	
        public static byte Read(ushort adr)
        {
            return 0;
        }
	
        public static void Write(ushort adr, byte value)
        {

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
