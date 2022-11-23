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
        /// Ширина экрана в точках
        /// </summary>
        public const int WIDTH = 256;

        /// <summary>
        /// Высота экрана в точках
        /// </summary>
        public const int HEIGHT = 240;

        /// <summary>
        /// Память видеопроцессора
        /// </summary>
        public static byte[] memory = new byte[PPU_MEM_SIZE];

        /// <summary>
        /// Палитра 64 цвета
        /// </summary>
        public static byte[] palette = new byte[] {
            84, 84, 84, 0, 30, 116, 8, 16, 144, 48, 0, 136, 68, 0, 100, 92, 0, 48, 84, 4, 0, 60, 24, 0, 32, 42, 0, 8, 58, 0, 0, 64, 0, 0, 60, 0, 0, 50, 60, 0, 0, 0,
            152, 150, 152, 8, 76, 19, 48, 50, 236, 92, 30, 228, 136, 20, 176, 160, 20, 100, 152, 34, 32, 120, 60, 0, 84, 90, 0, 40, 114, 0, 8, 124, 0, 0, 118, 40, 0, 102, 120, 0, 0, 0,
            236, 238, 236, 76, 154, 236, 120, 124, 236, 176, 98, 236, 228, 84, 236, 236, 88, 180, 236, 106, 100, 212, 136, 32, 160, 170, 0, 116, 196, 0, 76, 208, 32, 56, 204, 108, 56, 180, 204, 60, 60, 60,
            236, 238, 236, 168, 204, 236, 188, 188, 236, 212, 178, 236, 236, 174, 236, 236, 174, 212, 236, 180, 176, 228, 196, 144, 204, 210, 120, 180, 222, 120, 168, 226, 144, 152, 226, 180, 160, 214, 228, 160, 162, 160
        };

        public static byte[] screen = new byte[WIDTH * HEIGHT * 3];

        /// <summary>
        /// Номер экрана
        /// </summary>
        public static int nametable;

        /// <summary>
        /// Если переменная = 0, то к адресу добавляется 1, если переменная = 1, то к адресу добавляется 32
        /// </summary>
        public static int increment;

        /// <summary>
        /// Номер таблицы шаблонов для спрайтa 8x8  0: $0000; 1: $1000. Для спрайтa 8x16 игнорируется
        /// </summary>
        public static int sprite_table;

        /// <summary>
        /// Номер таблицы для фона; 0: $0000; 1: $1000
        /// </summary>
        public static int background_table;

        /// <summary>
        /// Размер спрайта. Если 0, то 8x8, если 1, то 8x16
        /// </summary>
        public static int sprite_size;

        /// <summary>
        /// Если 0, то прерывание НЕ генерируется, если 1, то генерируется
        /// </summary>
        public static bool generate_nmi;

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

	/// <summary>
        /// Записать в регистр управления
        /// </summary>
        /// <param name="val">значение</param>
        public static void ControllerWrite(byte val)
        {
            nametable = val & 3;
            increment = (val >> 2) & 1;
            sprite_table = (val >> 3) & 1;            
            background_table = (val >> 4) & 1;
            sprite_size = (val >> 5) & 1;            
            generate_nmi = ((val >> 7) & 1) > 0;
        }

        /// <summary>
        /// Вычислить адрес, где хранятся данные строчки тайла
        /// </summary>
        /// <param name="tile">номер тайла 0-255</param>
        /// <param name="tile_y">строчка тайла 0-7</param>
        /// <param name="table">номер таблицы тайлов 0-1</param>
        /// <returns></returns>
        ushort GetTileAdr(byte tile, int tile_y, int table)
        {
            
            return 0;
        }
    }
}
