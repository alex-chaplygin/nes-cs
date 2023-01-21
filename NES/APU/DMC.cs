﻿using System;
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
        /// Метод для 0x4010
        /// </summary>
        public static void FlagsandRate(byte val)
        {

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
