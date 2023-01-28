using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.APU
{
    public class Sequencer
    {
        int[] b;
        int counter;

        public Sequencer(int[] b)
        {
            this.b = b;
            this.counter = 0;
        }

        public int Clock()
        {
            if (counter == b.Length - 1)
            {
                counter = 0;
                return b[b.Length - 1];
            }

            counter++;
            return b[counter - 1];
        }

    }
}
