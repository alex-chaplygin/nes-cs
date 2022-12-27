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
        /// Функцмя записи в APU 
        /// </summary>
        /// <param name="adr">Адрес</param>
        /// <param name="val">Значение</param>
        delegate void APUWrite(ushort adr, byte val);

        static APUWrite[] apu = new APUWrite[]
        {
        };


        /// <summary>
        /// Записать значения в APU
        /// </summary>
        /// <param name="adr">Адрес</param>
        /// <param name="val">Значнение</param>
        public static void Write(ushort adr, byte val)
        {
            

        }        
    }
}
