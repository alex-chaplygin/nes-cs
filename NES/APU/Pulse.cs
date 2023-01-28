using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.APU
{
    class Pulse
    {

        /// <summary>
        /// режим
        /// </summary>
        public int duty;

        /// <summary>
        /// отключённый
        /// </summary>
        public int enabled;

        /// <summary>
        /// Делитель
        /// </summary>
        Divider divider;

        /// <summary>
        /// Счётчик длительности
        /// </summary>
        LengthCounter lengthCounter;

        /// <summary>
        /// Модуль изменения периода
        /// </summary>
        Sweep sweep;

        /// <summary>
        /// Генератор огибающей
        /// </summary>
        Envelope envelope;

        public Pulse()
        {
            this.lengthCounter = new LengthCounter(0);
            this.sweep = new Sweep();
            this.envelope = new Envelope();
        }

        public void EnvelopeI(byte val)
        {
            duty = val >> 6;
            if (((val >> 5) & 1) == 1)
                lengthCounter.Halt();
            envelope.const_volume = ((val >> 4) & 1) == 1;
            envelope.volume = val & 0xF;
        }

        public void SweepI(byte val)
        {
            sweep.Set(val);
        }

        public void Timer(byte val)
        {
            int timer_low = divider.Get();
            timer_low = timer_low & 0x700 | val;
            divider.Set(timer_low);
        }

        public void Length(byte val)
        {
            int timer_high = divider.Get();
            timer_high = timer_high & 0xFF | (val << 8);
            divider.Set(timer_high);
            lengthCounter.Set(val >> 3);
        }
    }
}
