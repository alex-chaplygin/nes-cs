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
	/// флаг Pulse1
	/// </summary>
	static bool enablePulse1;

	/// <summary>
	/// флаг Pulse2
	/// </summary>
	static bool enablePulse2;

	/// <summary>
	/// флаг Triangle
	/// </summary>
	static bool enableTriangle;

	/// <summary>
	/// флаг Noise
	/// </summary>
	static bool enableNoise;

	/// <summary>
	/// флаг DMC
	/// </summary>
	static bool enableDMC;
	
	/// <summary>
        /// Импульсный канал
        /// </summary>
        struct Pulse
        {
            /// <summary>
            /// режим
            /// </summary>
            public int duty;

            /// <summary>
            /// окутывать цыкл
            /// </summary>
            public int envelopeLoop;

            /// <summary>
            /// постоянная громкость
            /// </summary>
            public int constantVolume;

            /// <summary>
            /// громкость
            /// </summary>
            public int volume;

            /// <summary>
            /// отключённый
            /// </summary>
            public int enabled;

            /// <summary>
            /// период
            /// </summary>
            public int period;

            /// <summary>
            /// отрицать
            /// </summary>
            public int negate;

            /// <summary>
            /// сдвиг
            /// </summary>
            public int shift;

            /// <summary>
            /// низкий таймер
            /// </summary>
            public int timerLow;

            /// <summary>
            /// 
            /// </summary>
            public int lengthCounterLoad;

            /// <summary>
            /// высокий таймер
            /// </summary>
            public int timerHigh;
        }

	/// <summary>
        /// Треугольный канал создаёт квантовую треугольную волну 
        /// </summary>
        struct Triangle
        {
            /// <summary>
            /// остановщик счетика длинны 
            /// </summary>
            public int lengthCounterHalt;

            /// <summary>
            /// линейная встречная нагрузка
            /// </summary>
            public int linearCounterLoad;

            /// <summary>
            /// таймер низких
            /// </summary>
            public int timerLow;

            /// <summary>
            /// длина встречной нагрузки
            /// </summary>
            public int lengthCounterLoad;

            /// <summary>
            /// таймер выскокий 
            /// </summary>
            public int timerHigh;
        }
	
	/// <summary>
        /// Функция записи в APU 
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
	    new Register( 0x4010, DMC.FlagsandRate),
            new Register( 0x4011, DMC.Directload ),
            new Register( 0x4012, DMC.Sampleaddress ),
            new Register( 0x4013, DMC.Samplelength ),
	    new Register( 0x4015, StatusWrite ),
	    new Register( 0x400C, Noise.SetParametrs),
	    new Register( 0x400E, Noise.ModePeriod),
	    new Register( 0x400F, Noise.LengthCounterLoad)
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

	/// <summary>
        /// Функция установки флагов каналов
        /// </summary>
        /// <param name="val"></param>
        public static void StatusWrite(byte val)
        {
            enablePulse1 = (val & 0x01) != 0;
            enablePulse2 = (val & 0x02) != 0;
            enableTriangle = (val & 0x04) != 0;
            enableNoise = (val & 0x08) != 0;
            enableDMC = (val & 0x10) != 0;
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
