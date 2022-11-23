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
        public static byte[] memory = new byte[PPU_MEM_SIZE];

	/// <summary>
	///   Регистр адреса
	/// </summary>
	public static ushort address;

	/// <summary>
        /// Регистр позиции прокрутки ППУ
        /// </summary>
        public static ushort scroll;

	/// <summary>
	///   Первая запись в адрес
	/// </summary>
        static bool isFirst = true;

	public static int increment;
	
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
	/// Увеличить адрес, если increment=0, то на 1; если increment=1, то на 32
	/// </summary>
	/// <param name="adr"></param>
	static void IncreaseAddress ()
	{
	    if (increment == 0)
		address++;
	    else if (increment == 1)
		address += 32;
	}
	
	/// <summary>
	/// Записать в память байт по адресу
	/// </summary>
	/// <param name="bt"></param>
	public static void WriteData (byte bt)
	{
	    memory[address] = bt;
	    IncreaseAddress();
	} 

	/// <summary>
	/// Прочитать из памяти байт по адресу
	/// </summary>
	/// <param name="bt"></param>
	static byte ReadData ()
	{
	    return 0;
	} 
	
	/// <summary>
        /// Записать таблицу шаблонов 0
        /// </summary>
        /// <param name="data">Данные шаблонов</param>
        public static void WritePattern0(byte[] data)
        {
            data.CopyTo(memory, PATTERN_TABLE_0);
        }

        /// <summary>
        /// Записать таблицу шаблонов 1
        /// </summary>
        /// <param name="data">Данные шаблонов</param>
        public static void WritePattern1 (byte[] data)
        {
            data.CopyTo(memory, PATTERN_TABLE_1);
        }

	// Акименко Максим 
        /// <summary>
        /// первый раз старший байт регистра прокрутки второй младший байт регистра прокрутки
        /// </summary>
        /// <param name="val">старший или младший байт</param>
        static void SetScroll(byte val)
        {
            if (isFirst)
            {
                scroll = (ushort)(scroll & 0xFF);
                scroll = (ushort)(scroll | val<<8);
                isFirst = false;
            }
            else
            {
                scroll = (ushort)(scroll & 0xFF00);
                scroll = (ushort)(scroll | val);
                isFirst = true;
            }
        }
    }
}
