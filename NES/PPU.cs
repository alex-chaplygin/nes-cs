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
	const int PPU_MEM_SIZE = 0x4000;
        public const int PATTERN_TABLE_0 = 0x0;
        public const int PATTERN_TABLE_1 = 0x1000;

	/// <summary>
        /// Память видеопроцессора
        /// </summary>
        public static byte[] ppu_memory = new byte[PPU_MEM_SIZE];

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

	        /// <summary>
        /// Записать таблицу шаблонов 0
        /// </summary>
        /// <param name="data">Данные шаблонов</param>
        public static void WritePattern0(byte[] data)
        {
            data.CopyTo(ppu_memory, PATTERN_TABLE_0);
        }

        /// <summary>
        /// Записать таблицу шаблонов 1
        /// </summary>
        /// <param name="data">Данные шаблонов</param>
        public static void WritePattern1 (byte[] data)
        {
            data.CopyTo(ppu_memory, PATTERN_TABLE_1);
        }
    }
}
