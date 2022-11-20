using NES;
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
            BRK = 0xFFFE, // программное прерывание
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
        /// Флаг аккумуляторной адресации
        /// </summary>
        static bool is_accum;
	
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
            new Instruction(PHP, Implied, 3),
            new Instruction(ORA, Immediate, 2),
            new Instruction(ASL, Accumulator, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(ORA, Absolute, 4),
            new Instruction(ASL, Absolute, 6),
            new Instruction(NUL, Implied, 0),
            // 0x10
            new Instruction(BPL, Relative, 2),
            new Instruction(ORA, IndirectY, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(ORA, ZeroX, 4),
            new Instruction(ASL, ZeroX, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(CLC, Implied, 2),
            new Instruction(ORA, AbsoluteY, 4),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(ORA, AbsoluteX, 4),
            new Instruction(ASL, AbsoluteX, 7),
            new Instruction(NUL, Implied, 0),
            // 0x20
            new Instruction(JSR, Absolute, 6),
            new Instruction(AND, XIndirect, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(BIT, Zero, 3),
            new Instruction(AND, Zero, 3),
            new Instruction(ROL, Zero, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(PLP, Implied, 4),
            new Instruction(AND, Immediate, 2),
            new Instruction(ROL, Accumulator, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(BIT, Absolute, 4),
            new Instruction(AND, Absolute, 4),
            new Instruction(ROL, Absolute, 6),
            new Instruction(NUL, Implied, 0),
            // 0x30
            new Instruction(BMI, Relative, 2),
            new Instruction(AND, IndirectY, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(AND, ZeroX, 4),
            new Instruction(ROL, ZeroX, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(SEC, Implied, 2),
            new Instruction(AND, AbsoluteY, 4),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(AND, AbsoluteX, 4),
            new Instruction(ROL, AbsoluteX, 7),
            new Instruction(NUL, Implied, 0),
            // 0x40
            new Instruction(RTI, Implied, 6),
            new Instruction(EOR, XIndirect, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(EOR, Zero, 3),
            new Instruction(LSR, Zero, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(PHA, Implied, 3),
            new Instruction(EOR, Immediate, 2),
            new Instruction(LSR, Accumulator, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(JMP, Absolute, 3),
            new Instruction(EOR, Absolute, 4),
            new Instruction(LSR, Absolute, 6),
            new Instruction(NUL, Implied, 0),
            // 0x50
            new Instruction(BVC, Relative, 2),
            new Instruction(EOR, IndirectY, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(EOR, ZeroX, 4),
            new Instruction(LSR, ZeroX, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(CLI, Implied, 2),
            new Instruction(EOR, AbsoluteY, 4),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(EOR, AbsoluteX, 4),
            new Instruction(LSR, AbsoluteX, 7),
            new Instruction(NUL, Implied, 0),
            // 0x60
            new Instruction(RTS, Implied, 6),
            new Instruction(ADC, XIndirect, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(ADC, Zero, 3),
            new Instruction(ROR, Zero, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(PLA, Implied, 4),
            new Instruction(ADC, Immediate, 2),
            new Instruction(ROR, Accumulator, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(JMP, Indirect, 5),
            new Instruction(ADC, Absolute, 4),
            new Instruction(ROR, Absolute, 6),
            new Instruction(NUL, Implied, 0),
            // 0x70
            new Instruction(BVS, Relative, 2),
            new Instruction(ADC, IndirectY, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(ADC, ZeroX, 4),
            new Instruction(ROR, ZeroX, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(SEI, Implied, 2),
            new Instruction(ADC, AbsoluteY, 4),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(ADC, AbsoluteX, 4),
            new Instruction(ROR, AbsoluteX, 7),
            new Instruction(NUL, Implied, 0),
            // 0x80
            new Instruction(NUL, Implied, 0),
            new Instruction(STA, XIndirect, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(STY, Zero, 3),
            new Instruction(STA, Zero, 3),
            new Instruction(STX, Zero, 3),
            new Instruction(NUL, Implied, 0),
            new Instruction(DEY, Implied, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(TXA, Implied, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(STY, Absolute, 4),
            new Instruction(STA, Absolute, 4),
            new Instruction(STX, Absolute, 4),
            new Instruction(NUL, Implied, 0),
            // 0x90
            new Instruction(BCC, Relative, 2),
            new Instruction(STA, IndirectY, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(STY, ZeroX, 4),
            new Instruction(STA, ZeroX, 4),
            new Instruction(STX, ZeroY, 4),
            new Instruction(NUL, Implied, 0),
            new Instruction(TYA, Implied, 2),
            new Instruction(STA, AbsoluteY, 5),
            new Instruction(TXS, Implied, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(STA, AbsoluteX, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            // 0xA0
            new Instruction(LDY, Immediate, 2),
            new Instruction(LDA, XIndirect, 6),
            new Instruction(LDX, Immediate, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(LDY, Zero, 3),
            new Instruction(LDA, Zero, 3),
            new Instruction(LDX, Zero, 3),
            new Instruction(NUL, Implied, 0),
            new Instruction(TAY, Implied, 2),
            new Instruction(LDA, Immediate, 2),
            new Instruction(TAX, Implied, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(LDY, Absolute, 4),
            new Instruction(LDA, Absolute, 4),
            new Instruction(LDX, Absolute, 4),
            new Instruction(NUL, Implied, 0),
            // 0xB0
            new Instruction(BCS, Relative, 2),
            new Instruction(LDA, IndirectY, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(LDY, ZeroX, 4),
            new Instruction(LDA, ZeroX, 4),
            new Instruction(LDX, ZeroY, 4),
            new Instruction(NUL, Implied, 0),
            new Instruction(CLV, Implied, 2),
            new Instruction(LDA, AbsoluteY, 4),
            new Instruction(TSX, Implied, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(LDY, AbsoluteX, 4),
            new Instruction(LDA, AbsoluteX, 4),
            new Instruction(LDX, AbsoluteY, 4),
            new Instruction(NUL, Implied, 0),
            // 0xC0
            new Instruction(CPY, Immediate, 2),
            new Instruction(CMP, XIndirect, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(CPY, Zero, 3),
            new Instruction(CMP, Zero, 3),
            new Instruction(DEC, Zero, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(INY, Implied, 2),
            new Instruction(CMP, Immediate, 2),
            new Instruction(DEX, Implied, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(CPY, Absolute, 4),
            new Instruction(CMP, Absolute, 4),
            new Instruction(DEC, Absolute, 6),
            new Instruction(NUL, Implied, 0),
            // 0xD0
            new Instruction(BNE, Relative, 2),
            new Instruction(CMP, IndirectY, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(CMP, ZeroX, 4),
            new Instruction(DEC, ZeroX, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(CLD, Implied, 2),
            new Instruction(CMP, AbsoluteY, 4),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(CMP, AbsoluteX, 4),
            new Instruction(DEC, AbsoluteX, 7),
            new Instruction(NUL, Implied, 0),
            // 0xE0
            new Instruction(CPX, Immediate, 2),
            new Instruction(SBC, XIndirect, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(CPX, Zero, 3),
            new Instruction(SBC, Zero, 3),
            new Instruction(INC, Zero, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(INX, Implied, 2),
            new Instruction(SBC, Immediate, 2),
            new Instruction(NOP, Implied, 2),
            new Instruction(NUL, Implied, 0),
            new Instruction(CPX, Absolute, 4),
            new Instruction(SBC, Absolute, 4),
            new Instruction(INC, Absolute, 6),
            new Instruction(NUL, Implied, 0),
            // 0xF0
            new Instruction(BEQ, Relative, 2),
            new Instruction(SBC, IndirectY, 5),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(SBC, ZeroX, 4),
            new Instruction(INC, ZeroX, 6),
            new Instruction(NUL, Implied, 0),
            new Instruction(SED, Implied, 2),
            new Instruction(SBC, AbsoluteY, 4),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(NUL, Implied, 0),
            new Instruction(SBC, AbsoluteX, 4),
            new Instruction(INC, AbsoluteX, 7),
            new Instruction(NUL, Implied, 0)
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
            Push((byte)(val >> 8));
            Push((byte)(val & 0x00FF));
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
	    byte l = Pop();
	    byte h = Pop();
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
        ///   Получить флаги из байта.
        /// </summary>
        static void DisassembleFlags(byte val)
        {
            carry_flag = (val & 1) > 0;
            zero_flag = (val & 2) > 0;
            interrupt_flag = (val & 4) > 0;
            decimal_flag = (val & 8) > 0;
            break_flag = (val & 16) > 0;
            overflow_flag = (val & 64) > 0;
            negative_flag = (val & 128) > 0;
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
        /// Абсолютная адресация памяти
        /// </summary>
        /// <param name="adr"></param>
        /// <returns></returns>
        static byte Absolute(ref ushort adr)
        {
            adr = ToWord(Fetch(), Fetch());
            return Memory.Read(adr);
        }

        /// <summary>
        /// Режим адресации - аккумулятор
        /// </summary>
        static byte Accumulator(ref ushort adr)
        {
	    is_accum = true;
            return A;
        }

        /// <summary>
        /// Режим адресации - относительная
        /// </summary>
        static byte Relative(ref ushort adr)
        {
            return Fetch();
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
        /// Адресация следующего значения из памяти
        /// </summary>
        /// <param name="adr"></param>
        /// <returns></returns>
        static byte Immediate(ref ushort adr)
        {
            return Fetch();
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
        /// Режим адресации абсолютный по нулевой странице со смещением X
        /// </summary>
        static byte ZeroX(ref ushort adr)
        {
            adr = (byte)(Fetch() + X);
            return Memory.Read(adr);
        }

        /// <summary>
        /// Режим адресации абсолютный по нулевой странице со смещением Y
        /// </summary>
        static byte ZeroY(ref ushort adr)
        {
            adr = (byte)(Fetch() + Y);
            return Memory.Read(adr);
        }

        /// <summary>
        /// Режим адресации - косвенный
        /// </summary>
        static byte Indirect(ref ushort adr)
        {
            adr = ToWord(Fetch(), Fetch());
            adr = Memory.ReadWord(adr);
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
        /// Режим адресации косвенный со смещением Y
        /// </summary>
        static byte IndirectY(ref ushort adr)
        {
            adr = Fetch();
            adr = Memory.ReadWord(adr);
            cross = IsCross(adr, Y);
            adr = (ushort)(adr + Y);
            return Memory.Read(adr);
        }

        /// <summary>
        /// Несуществующая команда
        /// </summary>
        static void NUL(byte val, ushort adr)
        {
            throw new Exception($"Вызвана несуществующая команда: {Memory.Read((ushort)(PC - 1)):X}");
        }

        /// <summary>
	/// сложить аккумулятор с флагом переноса и с операндом и записать в аккумулятор
        /// флаг переполнения устанавливается когда изменился знак в результате сложения у акк был знак  а в результате получился знак другой знак это 7 бит
        /// </summary>
        public static void ADC(byte val, ushort adr)
        {
            ushort result = (ushort)(A + val + Convert.ToByte(carry_flag));
            overflow_flag = (A & 0x80) != (result & 0x80);
            carry_flag = (result > 0xFF);
            A = (byte)result;
            SetZeroNeg(A);
        }

	/// <summary>
        /// Вычесть память из аккумулятора с отрицательным переносом.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        /// <returns></returns>
        public static void SBC(byte val, ushort adr)
        {
            short result = (short)(A - val - Convert.ToByte(!carry_flag));
            overflow_flag = (A & 0x80) != (result & 0x80);
            carry_flag = (result < 0x00);
            A = (byte)result;
            SetZeroNeg(A);
        }

	// Носорев Николай
        /// <summary>
        /// Программное прерывание
        /// </summary>
        static void BRK(byte val, ushort adr) 
        {
            break_flag = true;
	    Interrupt(Interruption.BRK);
        }


        static void BIT(byte val, ushort adr)
        {

        }

	/// <summary>
        /// Циклический сдвиг влево
        /// </summary
        static void ROL(byte val, ushort adr)
        {
            int b = (val << 1) + Convert.ToByte(carry_flag);
            carry_flag = ((b >> 8) == 1);                      
            SetZeroNeg((byte)b);
            if (!is_accum) 
                Memory.Write(adr, (byte)b);
            else 
                A = (byte)b;
        }

	/// <summary>
        /// Циклический сдвиг вправо
        /// </summary
        static void ROR(byte val, ushort adr)
        {
            int b = (byte)((val >> 1) + (Convert.ToByte(carry_flag) << 7));
            carry_flag = ((val & 1) == 1);
            SetZeroNeg((byte)b);
            if (!is_accum)
		Memory.Write(adr, (byte)b);
            else
		A = (byte)b;
        }
	
        /// <summary>
        /// Логический сдвиг вправо
        /// </summary
        static void LSR(byte val, ushort adr)
        {
            int b = (byte)(Memory.Read(adr) >> 1);
            carry_flag = (val & 1) == 1;
            SetZeroNeg((byte)b);
            if (!is_accum)
		Memory.Write(adr, (byte)b);
            else
		A = (byte)b;
        }

        /// <summary>
        /// Сохранить аккумулятор
        /// </summary>
        static void PHA(byte val, ushort adr)
        {
	    Push(A);
        }

        /// <summary>
        /// Восстановить аккумулятор
        /// </summary>
        static void PLA(byte val, ushort adr)
        {
	    A = Pop();
	    SetZeroNeg(A);
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
	/// <summary>
        /// сдвигает все биты содержимого аккумулятора или памяти на один бит влево
        /// </summary>
        /// <param name="val">значение из памяти</param>
        /// <param name="adr">адрес</param>
        static void ASL(byte val, ushort adr)
        {
            carry_flag = (val >> 7) == 1;
            A = (byte)(val << 1);
	    SetZeroNeg(A);
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
        static void CLD(byte val, ushort adr)
        {
            decimal_flag = false;
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
            PC = adr;
        }
	
        /// <summary>
        /// Вызов подпрограммы
        /// </summary>
        static void JSR(byte val, ushort adr)
        {
            PushWord(PC);
            PC = adr;
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
        /// Выгружает регистр аккумулятора из памяти
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void LDA(byte val, ushort adr)
        {
            A = Memory.Read(adr);
            add_cycle = Convert.ToInt32(cross);
            SetZeroNeg(A);
        }

        /// <summary>
        /// Выгружает индесный регистр X из памяти
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void LDX(byte val, ushort adr)
        {
            X = Memory.Read(adr);
            add_cycle = Convert.ToInt32(cross);
            SetZeroNeg(X);
        }

        /// <summary>
        /// Выгружает индесный регистр Y из памяти
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void LDY(byte val, ushort adr)
        {
            Y = Memory.Read(adr);
            add_cycle = Convert.ToInt32(cross);
            SetZeroNeg(Y);
        }        

        /// <summary>
        /// Пустая операция
        /// </summary>
        static void NOP(byte val, ushort adr)
        {
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
        /// Возврат из подпрограммы
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void RTS(byte val, ushort adr)
        {
            PC = PopWord();
        }

	/// <summary>
        /// Сохраняет регистр аккумулятора в память
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void STA(byte val, ushort adr)
        {
            Memory.Write(adr, A);
        }

        /// <summary>
        /// Сохраняет индексный регистр X в память
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void STX(byte val, ushort adr)
        {
            Memory.Write(adr, X);
        }

        /// <summary>
        /// Сохраняет индексный регистр Y в память
        /// </summary>
        /// <param name="val"></param>
        /// <param name="adr"></param>
        static void STY(byte val, ushort adr)
        {
            Memory.Write(adr, Y);
        }

	//Подушкин Иван
        /// <summary>
        /// Соединить флаги в байт и записать в стек
        /// </summary>
        static void PHP(byte val, ushort adr)
        {
	    break_flag = true;
            Push(AssembleFlags());
        }

	/// <summary>
        ///   Перенос A в X
        /// </summary>
        public static void TAX(byte val, ushort adr)
        {
            X = A;
            SetZeroNeg(X);
        }

        /// <summary>
        ///   Перенос A в Y
        /// </summary>
        public static void TAY(byte val, ushort adr)
        {
            Y = A;
            SetZeroNeg(Y);
        }

        /// <summary>
        ///   Перенос SP в X
        /// </summary>
        public static void TSX(byte val, ushort adr)
        {
            X = SP;
            SetZeroNeg(X);
        }

        /// <summary>
        ///   Перенос X в A
        /// </summary>
        public static void TXA(byte val, ushort adr)
        {
            A = X;
            SetZeroNeg(A);
        }

        /// <summary>
        ///   Перенос Y в A
        /// </summary>
        public static void TYA(byte val, ushort adr)
        {
            A = Y;
            SetZeroNeg(A);
        }

        /// <summary>
        ///   Перенос X в SP
        /// </summary>
        public static void TXS(byte val, ushort adr)
        {
            SP = X;
        }

        /// <summary>
        ///   Восстановить флаги.
        /// </summary>
        public static void PLP(byte val, ushort adr)
        {
            DisassembleFlags(Pop());
        }

        /// <summary>
        ///   Возврат из прерывания.
        /// </summary>
        public static void RTI(byte val, ushort adr)
        {
            PLP(val, adr);
            PC = PopWord();
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
	    is_accum = false;
            byte command = Fetch();
	    byte operandVal = table[command].adrMode(ref operandAddr);
            table[command].op(operandVal,operandAddr);
	    return table[command].cycles + add_cycle;
        }
    }
}
