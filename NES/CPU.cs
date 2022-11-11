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
        /// Дополнительный цикл работы команды
        /// </summary>
        static int add_cycle;

        /// <summary>
        /// Пересечение страниц в результате индексации с помощью X, Y
        /// </summary>
        static bool cross;
	
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
            new Instruction(ORA, XIndirect, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(ORA, Zero, 3),
            new Instruction(ASL, Zero, 5),
            new Instruction(NUL, Implied, 0),
	};

	/// <summary>
	///   Преобразовать два байта в слово
	/// </summary>
        public static ushort ToWord(byte low, byte high) 
        {
            return (ushort)((high << 8) + low);
        }

	/// <summary>
	///   Сохранить слово в стеке
	/// </summary>
        public static void PushWord(ushort val)
        {
            Push((byte)(val & 0x00FF));
            Push((byte)(val >> 8));
        }

	/// <summary>
	///   Сохранить байт в стеке
	/// </summary>
        public static void Push(byte val)
        {
            Memory.Write((ushort)(0x100 + SP--), val);
        }

	/// <summary>
	///   Извлечь байт из стека
	/// </summary>
        public static byte Pop()
        {
            return Memory.Read((ushort)(0x100 + ++SP));
        }

	/// <summary>
	///   Извлечь слово из стека
	/// </summary>
        public static ushort PopWord()
	{
	    byte h = Pop();
	    byte l = Pop();
            return ToWord(l, h);
        }

	/// <summary>
	///   Соединить флаги в байт
	/// </summary>
        static byte AssembleFlags()
        {
            return (byte)(Convert.ToByte(carry_flag) | Convert.ToByte(zero_flag)<<1 |
			  Convert.ToByte(interrupt_flag) << 2 | Convert.ToByte(decimal_flag) << 3 |
			  Convert.ToByte(break_flag) << 4 | 1 << 5 |
			  Convert.ToByte(overflow_flag) << 6 | Convert.ToByte(negative_flag) << 7);
        }

        /// <summary>
        /// Загрузить очередной байт из памяти
        /// </summary>
        /// <returns>значение байта</returns>
        static byte Fetch()
        {
            return Memory.Read(PC++);
        }

	//Малышев Максим
        /// <summary>
        /// Проверяет пересечение страницы
        /// </summary>
        /// <param name="adr">Начальный адрес</param>
        /// <param name="offset">Смещение которое прибавляется к адресу</param>
        /// <returns></returns>
        static bool IsCross(int adr, int offset)
        {
            int adr1 = adr + offset;
            return (adr >> 8) != (adr1 >> 8);
        }

	/// <summary>
        /// Установить значение флагов нуля и знака
        /// </summary>
        /// <param name="val">Результат операции</param>
        static void SetZeroNeg(byte val)
        {
            CPU.zero_flag = val == 0;
            CPU.negative_flag = (val & 0x80) > 0;
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

        /// <summary>
        ///  Абсолютная адресация
        /// </summary>
        public static byte Absolute(ref ushort adr)
        {
            adr = ToWord(Fetch(), Fetch());
            return Memory.Read(adr);
        }

        /// <summary>
        ///  Абсолютная адресация со смещением X
        /// </summary>
        public static byte AbsoluteX(ref ushort adr)
        {
            adr = (ushort)(ToWord(Fetch(), Fetch()));
	    cross = IsCross(adr, X);
	    adr += X;
            return Memory.Read(adr);
        }

	// Киселев Николай
        /// <summary>
        ///  Абсолютная адресация со смещением Y
        /// </summary>
        public static byte AbsoluteY(ref ushort adr)
        {
            adr = (ushort)(ToWord(Fetch(), Fetch()));
	    cross = IsCross(adr, Y);
	    adr += Y;
            return Memory.Read(adr);
        }
	
        /// <summary>
        /// Режим адресации абсолютный по нулевой странице
        /// </summary>
        /// <param name="adr"></param>
        /// <returns></returns>
        static byte Zero(ref ushort adr)
        {
            adr = Fetch();
            return Memory.Read(adr);
        }

        /// <summary>
        /// Режим адресации косвенный со смещением X
        /// </summary>
        /// <param name="adr"></param>
        /// <returns></returns>
        public static byte XIndirect(ref ushort adr)
        {
            adr = Fetch();
            adr = Memory.ReadWord((ushort)((adr + X) & 0xFF));
            return Memory.Read(adr);
        }
	
        /// <summary>
        /// Несуществующая команда
        /// </summary>
	static void NUL(byte val, ushort adr)
        {
            throw new Exception($"Вызвана несуществующая команда: 
	    {Memory.Read((ushort)(PC - 1)):X}");
        }

	// Носорев Николай
        /// <summary>
        /// Программное прерывание
        /// </summary>
        static void BRK(byte val, ushort adr) 
        {
	    break_flag = true;
	    Interrupt(Interruption.IRQ);
        }

	// Малышев Максим
        /// <summary>
        /// Побитовое "И" аккумулятора с операндом
        /// </summary>
        /// <param name="val">Операнд</param>
        /// <param name="adr"></param>
        static void AND(byte val, ushort adr)
        {
            A &= val;
	    SetZeroNeg(A);
	    add_cycle = Convert.ToInt32(cross);
        }

	// Коваль Никита
        static void ASL(byte val, ushort adr)
        {
            
        }

	/// <summary>
        /// Переход, если флаг переноса сброшен, указатель команд устанавливается на новый адрес
        /// Добавляет 1 цикл если переход успешный и еще 1 цикл если при переходе была пересечена граница
        /// </summary>
        /// <param name="val">Смещение</param>
        /// <param name="adr">с</param>       
        static void BCC(byte val, ushort adr)
        {
            if (!carry_flag)
            {
                PC += (char)val;
                add_cycle++;
                if (IsCross(PC, (char)val))
                    add_cycle++;
            }
        }

        /// <summary>
        /// Переход, если флаш переноса установлен, указатель команд устанавливается на новый адрес
        /// </summary>
        /// <param name="val">Смещение</param>
        /// <param name="adr"></param>        
        static void BCS(byte val, ushort adr)
        {
            if (carry_flag)
            {
                PC += (char)val;
                add_cycle++;
                if (IsCross(PC, (char)val))
                    add_cycle++;
            }
            
        }

        /// <summary>
        /// Переход если флаг нуля установлен
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void BEQ(byte val, ushort adr)
        {
            if (zero_flag)
            {
                PC += (char)val;
                add_cycle++;
                if (IsCross(PC, (char)val))
                    add_cycle++;
            }
        }

        /// <summary>
        /// Переход если результат отрицательный
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void BMI(byte val, ushort adr)
        {
            if (negative_flag)
            {
                PC += (char)val;
                add_cycle++;
                if (IsCross(PC, (char)val))
                    add_cycle++;
            }
        }

        /// <summary>
        /// Переход если флаг нуля не установлен
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void BNE(byte val, ushort adr)
        {
            if (!zero_flag)
            {
                PC += (char)val;
                add_cycle++;
                if (IsCross(PC, (char)val))
                    add_cycle++;
            }
        }

        /// <summary>
        /// Переход если флаг положительный
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void BPL(byte val, ushort adr)
        {
            if (!negative_flag)
            {
                PC += (char)val;
                add_cycle++;
                if (IsCross(PC, (char)val))
                    add_cycle++;
            }
        }

        /// <summary>
        /// Переход если не переполнение
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void BVC(byte val, ushort adr)
        {
            if (!overflow_flag)
            {
                PC += (char)val;
                add_cycle++;
                if (IsCross(PC, (char)val))
                    add_cycle++;
            }
        }

        /// <summary>
        /// Переход если переполнение
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void BVS(byte val, ushort adr)
        {
            if (overflow_flag)
            {
                PC += (char)val;
                add_cycle++;
                if (IsCross(PC, (char)val))
                    add_cycle++;
            }
        }
	
        /// <summary>
        /// Сравнение аккумулятора с операндом
        /// </summary>
	static void CMP(byte val, ushort adr)
        {   byte result = (byte)(A - val);
            carry_flag = result >= 0;
            SetZeroNeg(result);
            add_cycle = cross ? 1 : 0;
        }

        /// <summary>
        /// Сравнение X с операндом
        /// </summary>
        static void CPX(byte val, ushort adr)
        {
            byte result = (byte)(X - val);
            carry_flag = result >= 0;
            SetZeroNeg(result);
            add_cycle = cross ? 1 : 0;
        }

        /// <summary>
        /// Сравнение Y с операндом
        /// </summary>
        static void CPY(byte val, ushort adr)
        {
            byte result = (byte)(Y - val);
            carry_flag = result >= 0;
            SetZeroNeg(result);
            add_cycle = cross ? 1 : 0;
        }

	/// <summary>
        /// Очищает флаг переноса
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void CLC(byte val, ushort adr)
        {
            carry_flag = false;
        }

        /// <summary>
        /// Очищает флаг десятичного режима
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void CLВ(byte val, ushort adr)
        {
            decimal_flag = false;
        }

        /// <summary>
        /// Очищает флаг запрета прерываний
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void CLI(byte val, ushort adr)
        {
            interrupt_flag = false;
        }

        /// <summary>
        /// Очищает флаг переполнения
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void CLV(byte val, ushort adr)
        {
            overflow_flag = false;
        }
	
        /// <summary>
        /// Уменьшить ячейку памяти
        /// </summary>
        static void DEC(byte val, ushort adr)
        {
            byte res = (byte)(val - 1);
            Memory.Write(adr, res);
            SetZeroNeg(res);
        }
	
        /// <summary>
        /// Уменьшить X
        /// </summary>
        static void DEX(byte val, ushort adr)
        {
	    X--;
            SetZeroNeg(X);
        }

        /// <summary>
        /// Уменьшить Y
        /// </summary>
        static void DEY(byte val, ushort adr)
        {
	    Y--;
            SetZeroNeg(Y);
        }
	
        /// <summary>
        /// Увеличить ячейку памяти
        /// </summary>
        static void INC(byte val, ushort adr)
        {
            byte res = (byte)(val + 1);
            Memory.Write(adr, res);
            SetZeroNeg(res);
        }

        /// <summary>
        /// Увеличить X
        /// </summary>
        static void INX(byte val, ushort adr)
        {
	    X++;
            SetZeroNeg(X);
        }

        /// <summary>
        /// Увеличить Y
        /// </summary>
        static void INY(byte val, ushort adr)
        {
	    Y++;
            SetZeroNeg(Y);
        }
	
        /// <summary>
        /// Безусловный переход
        /// </summary>
        static void JMP(byte val, ushort adr)
        {
            PC = Memory.ReadWord(adr);
        }
	
        /// <summary>
        /// Вызов подпрограммы
        /// </summary>
        static void JSR(byte val, ushort adr)
        {
            PushWord(PC);
            PC = Memory.ReadWord(adr);
        }
	
        /// <summary>
        /// Побитовое ИЛИ аккумулятора и операнда
        /// </summary>
        static void ORA(byte val, ushort adr)
        {
            A |= val;
	    SetZeroNeg(A);
	    add_cycle = Convert.ToInt32(cross);
        }

	// Малышев Максим
        /// <summary>
        /// Исключающее ИЛИ аккумулятора и операнда
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void EOR(byte val, ushort adr)
        {
            A ^= val;
            SetZeroNeg(A);
            add_cycle = Convert.ToInt32(cross);
        }

	/// <summary>
        /// Установить флаг переноса
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void SEC(byte val, ushort adr)
        {
            carry_flag = true;
        }

        /// <summary>
        /// Установить флаг десятичного режима
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void SED(byte val, ushort adr)
        {
            decimal_flag = true;
        }

        /// <summary>
        /// Установить флаг запрета прерываний
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void SEI(byte val, ushort adr)
        {
            interrupt_flag = true;
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
	    add_cycle = 0;
            cross = false;
            byte command = Fetch();
	    byte operandVal = table[command].adrMode(ref operandAddr);
            table[command].op(operandVal,operandAddr);
	    return table[command].cycles + add_cycle;
        }
    }
}
