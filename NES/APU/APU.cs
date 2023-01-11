using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.APU
{
    class APU
    {
	/// <summary>
        /// Канал проигрывания звуковых отсчетов с delta модуляцией
        /// </summary>
        struct DMC
        {
            /// <summary>
            /// Флаг IRQ
            /// </summary>
            public bool irq_flag;

            /// <summary>
            /// Флаг цикла
            /// </summary>
            public bool loop_flag;

            /// <summary>
            /// Индекс частоты
            /// </summary>
            public int rate_index;

            /// <summary>
            /// выходной уровень (громкость канала)
            /// </summary>
            public Byte sirect_load;

            /// <summary>
            /// адрес отсчётов
            /// </summary>
            public ushort sample_address;

            /// <summary>
            /// длина отсчётов
            /// </summary>
            public int sample_length;
        }

	/// <summary>
        /// Функцмя записи в APU 
        /// </summary>
        /// <param name="val">Значение</param>
        delegate void APUWrite(byte val);

	struct Register
        {
            public int adr;

            public APUWrite write;

            public Register(int a, APUWrite w)
            {
                adr = a;
                write = w;
            }
        }

        static Register[] memoryTable = new Register[]
        {
            new Register( 0x4000,  Pulse1Timer ),
            new Register( 0x4001, Pulse1Length ),
            new Register( 0x4002, Pulse1Envelope ),
            new Register( 0x4003, Pulse1Sweep ),
            new Register( 0x4004, Pulse2Timer ),
            new Register( 0x4005, Pulse2Length ),
            new Register( 0x4006, Pulse2Envelope ),
            new Register( 0x4007, Pulse2Sweep ),
        };

        /// <summary>
        /// Записать значения в APU
        /// </summary>
        /// <param name="adr">Адрес</param>
        /// <param name="val">Значнение</param>
        public static void Write(ushort adr, byte value)
        {
	    for (int i = 0; i < memoryTable.Length; i++)
                if (adr == memoryTable[i].adr)
                {
                    memoryTable[i].write(value);
                    return;
                }            
        }

	static void Pulse1Timer(byte val)
        {
            
        }

        static void Pulse1Length(byte val)
        {

        }

	static void Pulse1Envelope(byte val)
        {

        }

	static void Pulse1Sweep(byte val)
        {

        }

	static void Pulse2Timer(byte val)
        {

        }

	static void Pulse2Length(byte val)
        {

        }

	static void Pulse2Envelope(byte val)
        {

        }

	static void Pulse2Sweep(byte val)
        {

        }
    }
}
