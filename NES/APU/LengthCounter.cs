﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.APU
{
    public class LengthCounter
    {
        /// <summary>
        /// значение счетчика
        /// </summary>
        public int value;

        /// <summary>
        /// Таблица кодов
        /// </summary>
        private static readonly int[] table = { 10, 254, 20, 2, 40, 4, 80, 6, 160, 8, 60, 10, 14, 12, 26, 14, 12, 16, 24, 18, 48, 20, 96, 22, 192, 24, 72, 26, 16, 28, 32, 30 };

        /// <summary>
        /// если значение true, то счетчик включен
        /// </summary>
        private bool isActive;

        /// <summary>
        /// устанавливает начальное значение счетчика
        /// </summary>
        /// <param name="value">значение счетчика</param>
        public LengthCounter(int code)
        {
            this.value =table[code];
            this.isActive = true;
        }

        /// <summary>
        ///  если счетчик включен, то уменьшает значения счетчика. возвращает true, когда счетчик достигает 0. меньше нуля счетчик не уменьшается.
        /// </summary>
        /// <returns></returns>
        public bool Clock()
        {
            if (value == 0)
                return true;
            if (isActive && value > 0)
            {
                value--;
                if (value == 0)
                    return true;
            }
            return false;

        }

        /// <summary>
        /// возвращает значение счетчика
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            return this.value;
        }

        /// <summary>
        /// устанавливает значение счетчика
        /// </summary>
        /// <param name="value">значение счетчика</param>
        public void Set(int code)
        {
            this.value = table[code];
        }

        /// <summary>
        /// выключает счетчик
        /// </summary>
        public void Halt()
        {
            this.isActive = false;
        }

        /// <summary>
        /// включает счетчик
        /// </summary>
        public void Continue()
        {
            this.isActive = true;
        }

        /// <summary>
        /// устанавливает счетчик в 0
        /// </summary>
        public void Clear()
        {
            this.value = 0;
        }
    }
}
