using System;
namespace NES.APU
{
    public class Envelope
    {
        /// <summary>
        /// Флаг начала
        /// </summary>
        static bool start_flag;

        /// <summary>
        /// Делитель
        /// </summary>
        Divider divider;

        /// <summary>
        /// Cчётчик угасания
        /// </summary>
        LengthCounter decay;

        /// <summary>
        /// Уровень громкости
        /// </summary>
        public int volume;

        /// <summary>
        /// Флаг постоянной громкости
        /// </summary>
        public bool const_volume;       
    }
}
