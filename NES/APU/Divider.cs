using System;

namespace NES
{
    class Divider
    {
	private int period;
	private int counter;

	public Divider(int period)
	{
	    this.period = period;
	    this.counter = period;
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
    }
} 
