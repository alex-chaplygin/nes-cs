using System;

namespace NES
{
	class Divider
	{
		private int period;
		private int counter;
	    private static readonly int[] table = {428, 380, 340, 320, 286, 254, 226, 214, 190, 160, 142, 128, 106, 84, 72, 54};

		public Divider(int code)
		{
		    period = table[code];
		    counter = period;
		}

		public bool Clock()
		{
			if (this.counter == 0)
			{
				this.counter = this.period;
				return true;
			}
			else
			{
				this.counter--;
				return false;
			}
		}

		public void Reset()
		{
			counter = period;
		}

		public int Get()
		{
		    return period;
		}

		public void Set(int val)
		{
		    period = table[val];
		    counter = period;
		}
	}
}
