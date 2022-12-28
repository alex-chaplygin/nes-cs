﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    /// <summary>
    ///   Класс памяти
    /// </summary>
    public class Memory
    {
        /// <summary>
        /// общий размер памяти
        /// </summary>
	const int MEM_SIZE = 0x10000;

	/// <summary>
        /// размер банка программы
        /// </summary>
        public const int PRG_SIZE = 0x4000;

	/// <summary>
        /// размер банка изображений
        /// </summary>
        public const int CHR_SIZE = 0x2000;

	/// <summary>
	/// 4 килобайтный банк изображения 
	/// </summary>
        public const int CHR_SIZE_4BYTES = 0x1000;
	
	/// <summary>
        /// размер оперативной памяти
        /// </summary>
        public const int RAM_SIZE = 0x800;

	/// <summary>
        /// размер трейнера
        /// </summary>
        public const int TRN_SIZE = 0x200;

	/// <summary>
        /// область ПЗУ 1
        /// </summary>	
	const int ROM1 = 0x8000;

	/// <summary>
        /// область ПЗУ 2
        /// </summary>	
        const int ROM2 = 0xC000;

	/// <summary>
        /// все адресное пространство памяти
        /// </summary>
	public static byte[] memory = new byte[MEM_SIZE];

	/// <summary>
	/// Функция чтения памяти
	/// </summary>
	/// <param name="adr">Адрес памяти</param>
	/// <returns>Значение из памяти</returns>
	delegate byte MemoryRead(ushort adr);

	/// <summary>
	/// Функция записи в память
	/// </summary>
	/// <param name="adr">Адрес памяти</param>
	/// <param name="value">Значение, записываемое в память</param>
	delegate void MemoryWrite(ushort adr, byte value);

	/// <summary>
	/// Регион памяти
	/// </summary>
	struct MemoryRegion
        {
	    /// <summary>
	    /// Верхняя граница
	    /// </summary>
	    public int upper;

	    /// <summary>
	    ///   Функция чтения
	    /// </summary>
	    public MemoryRead read;

	    /// <summary>
	    ///   Функция записи
	    /// </summary>
	    public MemoryWrite write;

	    public MemoryRegion(int u, MemoryRead r, MemoryWrite w)
            {
		upper = u;
		read = r;
		write = w;
            }
        }

	/// <summary>
	///   Таблица регионов памяти
	/// </summary>
	static MemoryRegion[] memoryTable = new MemoryRegion[]
	{
	    new MemoryRegion( 0x2000, ReadRAM, WriteRAM ),
	    new MemoryRegion( 0x4015, PPU.Read, PPU.Write ),
	    new MemoryRegion( 0x4018, Input.Read, Input.Write ),
	    new MemoryRegion( 0x8000, ReadM, WriteM ),
	    new MemoryRegion( 0x10000, ReadM, Mapper.Write ),
	};
	
	/// <summary>
	///   Читать байт из памяти
	/// </summary>
        /// <param name="adr">адрес памяти</param>
        /// <returns>значение из памяти</returns>
	public static byte Read(ushort adr)
	{
	    for (int i = 0; i < memoryTable.Length; i++)
            {
		if (adr < memoryTable[i].upper)
		    return memoryTable[i].read(adr);
            }
	    return 0;
	}

	/// <summary>
	///   Читать слово из памяти
	/// </summary>
        /// <param name="adr">адрес памяти</param>
        /// <returns>значение из памяти</returns>
	public static ushort ReadWord(ushort adr)
	{
	    if ((adr & 0x00FF) == 0x00FF)
		return (ushort)(Read((ushort)(adr & 0xFF00)) << 8);
	    else
		return (ushort)((Read((ushort)(adr + 1)) << 8) | Read(adr));
	}

	// Киселев Николай
	/// <summary>
	///   Записать байт в память
	/// </summary>
        /// <param name="adr">адрес памяти</param>
        /// <param name="val">записываемое значение</param>
	public static void Write(ushort adr, byte val)
	{
	    for (int i = 0; i < memoryTable.Length; i++)
	    {
				if (adr < memoryTable[i].upper)
				{
					memoryTable[i].write(adr, val);
					return;
				}
	    }
	}

	/// <summary>
	///   Простое чтение
	/// </summary>
	static byte ReadM(ushort adr)
	{
	    return memory[adr];
	}

	/// <summary>
	///   Простая запись
	/// </summary>
	static void WriteM(ushort adr, byte value)
        {
	    memory[adr] = value;
        }

	// Алексеев Андрей
	/// <summary>
	///   Чтение из ОЗУ
	/// </summary>
	static byte ReadRAM(ushort adr)
	{
	    adr &= 0x7FF;
	    return memory[adr];
	}

	// Киселев Николай
	/// <summary>
	///   Запись в ОЗУ
	/// </summary>
	static void WriteRAM(ushort adr, byte val)
	{
            adr &= 0x7FF;
            memory[adr] = val;
	}
	
	// Геворкян Арнольд
	/// <summary>
	///   Записать ПЗУ1
	/// </summary>
        /// <param name="bank">банк программы</param>
	public static void WriteROM1(byte[] bank)
	{
	    int count = 0;
            for (int i = ROM1; i < ROM2; i++)
            {
                if (count < bank.Length)
                    memory[i] = bank[count];
                else
                    break;
                count++;
            }
	}

	/// <summary>
	///   Записать ПЗУ2
	/// </summary>
        /// <param name="bank">банк программы</param>
	public static void WriteROM2(byte[] bank)
	{
	    int count = 0;
            for (int i = ROM2; i < memory.Length; i++)
            {
                if (count < bank.Length)
                    memory[i] = bank[count];
                else
                    break;
                count++;
            }
	}
    }
}
