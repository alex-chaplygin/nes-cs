﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.APU
{
    class DMC
    {
        /// <summary>
        /// Флаг IRQ
        /// </summary>
        static bool irq_flag;

        /// <summary>
        /// Флаг цикла
        /// </summary>
        static bool loop_flag;

        /// <summary>
        /// Индекс частоты
        /// </summary>
        static int rate_index;

        /// <summary>
        /// выходной уровень (громкость канала)
        /// </summary>

        static Byte sirect_load;

        /// <summary>
        /// адрес отсчётов
        /// </summary>
        static ushort sample_address;

        /// <summary>
        /// длина отсчётов
        /// </summary>
        static int sample_length;

        /// <summary>
        /// прерывание вкл
        /// </summary>
        static bool interrupt_enabled;

        /// <summary>
        /// зацикливание вкл
        /// </summary>
        static bool loop_enabled;

        /// <summary>
        /// частота
        /// </summary>
        static  int frequency;

        /// <summary>
        /// таблица частот NTSC
        /// </summary>
        static readonly int[] NTSC_FrequencyTable = {428, 380, 340, 320, 286, 254, 226, 214, 190, 160, 142, 128, 106, 84, 72, 54};

        /// <summary>
        /// Метод установки флагов и частоты
        /// </summary>
        public static void FlagsandRate(byte val)
        {
            interrupt_enabled = (val & (1 << 7)) != 0; 
            loop_enabled = (val & (1 << 6)) != 0; 
            int frequencyCode = val & 0xF;
            frequency = NTSC_FrequencyTable[frequencyCode];
        }

        /// <summary>
        /// Метод для 0x4011
        /// </summary>
        public static void Directload(byte val)
        {

        }

        /// <summary>
        /// Метод для 0x4012
        /// </summary>
        public static void Sampleaddress(byte val)
        {

        }

        /// <summary>
        /// Метод для 0x4013
        /// </summary>
        public static void Samplelength(byte val)
        {

        }
    }
}
