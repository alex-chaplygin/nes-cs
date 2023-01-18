using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.APU
{
    /// <summary>
    /// Класс для периодической измены периода для прямоугольного Pulse канала
    /// </summary>
    class Sweep {
	/// <summary>
	/// Делитель, который отсчитывает такты
	/// </summary>
	Divider divider;

	/// <summary>
	/// Включен/выключен
	/// </summary>
	bool enable;

	/// <summary>
	/// Период
	/// </summary>
	int period;

	/// <summary>
	/// Инвертировать сдвиг
	/// </summary>
	bool negate;

	/// <summary>
	/// Число бит сдвига периода
	/// </summary>
	int shift;

	/// <summary>
	/// Инициализируем объект Sweep
	/// </summary>
	public Sweep(int dividerValue)
	{
	    divider = new Divider(dividerValue);
	    enable = false;
	    period = 0;
	    negate = false;
	    shift = 0;
	}
    }
}
