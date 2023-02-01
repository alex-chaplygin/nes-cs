using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.APU
{
    class Noise
    {
         /// <summary>
        /// Флаг периода
        /// </summary>
        static bool mode;
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
        
        static Divider divider;
        
        /// <summary>
        /// 
        /// </summary>
        static Envelope envelope;
        
        static LengthCounter lengthCounter = new LengthCounter(1);
        

        /// <summary>
        /// таблица частот NTSC
        /// </summary>
        static readonly int[] NTSC_FrequencyTable = {428, 380, 340, 320, 286, 254, 226, 214, 190, 160, 142, 128, 106, 84, 72, 54};

        /// <summary>
        /// Установка параметров
        /// </summary>
        public static void SetParametrs(byte val)
        {
            envelope = new Envelope (val & 0xF, (val & 0x10) == 1);
            if ((val & 0x20) == 1) 
                lengthCounter.Halt();            
        }
        
	public static void ModePeriod(byte val)
        {
           divider = new Divider(NTSC_FrequencyTable[val & 0xF]); 
           mode = (val & 0x80) == 1;
        }
        
        public static void LengthCounterLoad(byte val)
        {
            lengthCounter.Set(val >> 3);
        }
    }
}
