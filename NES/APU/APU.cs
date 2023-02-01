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
        /// первый импульсный канал
        /// </summary>
        static Pulse pulse1 = new Pulse();

        /// <summary>
        /// второй импульсный канал
        /// </summary>
        static Pulse pulse2 = new Pulse();

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
            new Register( 0x4000, pulse1.EnvelopeI ),
            new Register( 0x4001, pulse1.SweepI ),
            new Register( 0x4002, pulse1.Timer ),
            new Register( 0x4003, pulse1.Length ),
            new Register( 0x4004, pulse2.EnvelopeI ),
            new Register( 0x4005, pulse2.SweepI ),
            new Register( 0x4006, pulse2.Timer ),
            new Register( 0x4007, pulse2.Length ),
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
    }
}
