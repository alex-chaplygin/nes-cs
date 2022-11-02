using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    
    /// <summary>
    ///   Графический процессор
    /// </summary>
    public class PPU
    {	
	/// <summary>
	///   Регистр адреса
	/// </summary>
	public static ushort address;

	/// <summary>
	///   Первая запись в адрес
	/// </summary>
        static bool isFirst = true;
	
	/// <summary>
	///   Запись в регистр PPU
	/// </summary>
        public static void Write(ushort adr, byte value)
        {

        }

	/// <summary>
	///   Чтение из регистра PPU
	/// </summary>
        public static byte Read(ushort adr)
        {
            return 0;
        }

	/// <summary>
	///   Записать адрес, сначала старший байт, затем младший
	/// </summary>
	public static void SetAddress(byte val)
        {
            if (isFirst)
            {
                address = (ushort)(val << 8 | (address & 0xFF));
                isFirst = false;
            }
            else
            {
                address = (ushort)(address & 0xFF00);
                address = (ushort)(address + val);
                isFirst = true;
            }
        }	
    }
}
