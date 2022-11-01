﻿using NES;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    /// <summary>
    ///   Прерывание
    /// </summary>
    public enum Interruption
    {
            IRQ = 0xFFFE, // ввод-вывод
            NMI = 0xFFFA, // немаскируемое - видео
            RESET = 0xFFFC, // сброс в начальное состояние
    }

    /// <summary>
    /// Класс центрального процессора
    /// </summary>
    public static class CPU
    {
        /// <summary>
        /// Регистр аккумулятора
        /// </summary>
        public static byte A;

        /// <summary>
        /// Индексный регистр X
        /// Не являясь аккумулятором, они сами имеют ограниченные режимы адресации при загрузке и сохранении
        /// </summary>
        public static byte X;

        /// <summary>
        /// Индексный регистр Y
        /// </summary>
        public static byte Y;

        /// <summary>
        /// Указатель команд
        /// </summary>
	public static ushort PC;

        /// <summary>
        /// Указатель стека
        /// </summary>
        public static byte SP = 0xFF;

        /// <summary>
        /// Флаг переноса
        /// </summary>
        public static bool carry_flag;

        /// <summary>
        /// Флаг нуля
        /// </summary>
        public static bool zero_flag;

        /// <summary>
        /// Флаг запрета прерываний
        /// </summary>
        public static bool interrupt_flag;

        /// <summary>
        /// Флаг десятичного режима
        /// </summary>
        public static bool decimal_flag;

        /// <summary>
        /// Флаг программного прерывания
        /// </summary>
        public static bool break_flag;

        /// <summary>
        /// Флаг переполнения
        /// </summary>
        public static bool overflow_flag;

        /// <summary>
        /// Флаг отрицательного результата
        /// </summary>
        public static bool negative_flag;
        
        /// <summary>
        /// Функция команды 
        /// </summary>
        /// <param name="val">значение аргумента</param>
        /// <param name="adr">адрес аргумента</param>
        delegate void Operation(byte val, ushort adr);

        /// <summary>
        /// Функция режима адресации
        /// </summary>
        /// <param name="adr">возвращает адрес операнда</param>
        /// <returns>значение операнда</returns>
        delegate byte Address(ref ushort adr);
        
        /// <summary>
        /// Строка таблицы команд
        /// </summary>
        struct Instruction
        {
            /// <summary>
            /// Функция команды
            /// </summary>
            public Operation op;

            /// <summary>
            /// Функция режима адресации
            /// </summary>
            public Address adrMode;

            /// <summary>
            /// Число циклов
            /// </summary>
            public int cycles;

            public  Instruction(Operation op, Address adrMode, int cycles) 
            {
                this.op = op;
                this.adrMode = adrMode;
                this.cycles = cycles; 
            }
        }

        /// <summary>
        /// Таблица команд процессора
        /// </summary>
        static Instruction[] table = new Instruction[]
        {
            new Instruction(BRK, Implied, 7),
        };

	/// <summary>
	///   Преобразовать два байта в слово
	/// </summary>
        public static ushort ToWord(byte high, byte low) 
        {
            return (ushort)((high << 8) +low);
        }

	/// <summary>
	///   Сохранить слово в стеке
	/// </summary>
        public static void PushWord(ushort val)
        {
            CPU.Push((byte)(val & 0x00FF));
            CPU.Push((byte)(val >> 8));
        }

	/// <summary>
	///   Сохранить байт в стеке
	/// </summary>
        public static void Push(byte val)
        {
            Memory.Write((ushort)(0x100+ CPU.SP--),val);
        }

	/// <summary>
	///   Извлечь байт из стека
	/// </summary>
        public static byte Pop()
        {
            return Memory.Read((ushort)(0x100+ ++CPU.SP));
        }

	/// <summary>
	///   Извлечь слово из стека
	/// </summary>
        public static ushort PopWord()
	{
            return ToWord(CPU.Pop(), CPU.Pop());
        }

	/// <summary>
	///   Соединить флаги в байт
	/// </summary>
        static byte AssembleFlags()
        {
            return (byte)(Convert.ToByte(carry_flag) | Convert.ToByte(zero_flag)<<1 | Convert.ToByte(interrupt_flag) << 2 |
                Convert.ToByte(decimal_flag) << 3 | Convert.ToByte(break_flag) << 4 | Convert.ToByte(overflow_flag) << 6 | Convert.ToByte(negative_flag) << 7);
        }

        /// <summary>
        /// Загрузить очередной байт из памяти
        /// </summary>
        /// <returns>значение байта</returns>
        static byte Fetch()
        {
            return Memory.Read(PC++);
        }

        /// <summary>
        /// Режим адресации без операндов
        /// </summary>
        /// <param name="adr"></param>
        /// <returns></returns>
        static byte Implied(ref ushort adr)
        {
            return 0;
        }

	// Носорев Николай
        static void BRK(byte val, ushort adr) 
        {
	    Interrupt(Interruption.IRQ);
        }

	// Коваль Никита
        static void ASL(byte val, ushort adr)
        {
            
        }

	/// <summary>
	///   Сигнал прерывания
	/// </summary>
        public static void Interrupt(Interruption interruption)
        {
            if (!interrupt_flag || interruption != Interruption.IRQ)
            {
                Push(AssembleFlags());
                PushWord(PC);
                PC = Memory.ReadWord((ushort)interruption);
                if (interruption == Interruption.RESET)
                {
                    interrupt_flag = true;
                    SP = 0xFF;                   
                }              
            }      
        }

	/// <summary>
        ///   Шаг процессора
        /// </summary>
       public static int Step()
        {
            ushort operandAddr = 0;
            byte command = Fetch();
	    byte operandVal = table[command].adrMode(ref operandAddr);
            table[command].op(operandVal,operandAddr);
	    return table[command].cycles;
        }
    }
}
