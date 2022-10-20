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
        /// размер оперативной памяти
        /// </summary>
        public const int RAM_SIZE = 0x800;
	/// <summary>
        /// размер трейнера
        /// </summary>
        public const int TRN_SIZE = 0x200;
	/// <summary>
        /// все адресное пространство памяти
        /// </summary>
	public static byte[] memory = new byte[MEM_SIZE];

	// Алексеев Андрей

	/// <summary>
	///   Читать байт из памяти
	/// </summary>
        /// <param name="adr">адрес памяти</param>
        /// <returns>значение из памяти</returns>
	public static byte Read(ushort adr)
	{
	    while (adr >= RAM_SIZE)
                adr -= RAM_SIZE;
            return memory[adr];
	}

	/// <summary>
	///   Читать слово из памяти
	/// </summary>
        /// <param name="adr">адрес памяти</param>
        /// <returns>значение из памяти</returns>
	public static ushort ReadWord(ushort adr)
	{
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
	    memory[adr] = val;
	}

	// Геворкян Арнольд
	/// <summary>
	///   Записать ПЗУ1
	/// </summary>
        /// <param name="bank">банк программы</param>
	public static void WriteROM1(byte[] bank)
	{
	}

	/// <summary>
	///   Записать ПЗУ2
	/// </summary>
        /// <param name="bank">банк программы</param>
	public static void WriteROM2(byte[] bank)
	{
	}
    }
}
