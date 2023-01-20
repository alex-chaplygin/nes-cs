using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.APU
{
    internal class LengthCounter
    {
        private int value;
        private bool isActive;

        public LengthCounter(int value)
        {
            this.value = value;
            this.isActive = true;
        }

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

        public int GetValue()
        {
            return this.value;
        }

        public void Set(int value)
        {
            this.value = value;
        }

        public void Halt()
        {
            this.isActive = false;
        }

        public void Continue()
        {
            this.isActive = true;
        }

        public void Clear()
        {
            this.value = 0;
        }
    }
}
