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
        int volume;

        /// <summary>
        /// Флаг постоянной громкости
        /// </summary>
        bool const_volume;

	/// <summary>
        /// Конструктор
        /// </summary>
        public Envelope (int volume, bool const_volume)
        {
            this.volume = volume;
            this.const_volume = const_volume;
        }
    }
}
