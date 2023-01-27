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
	class Sweep
	{
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
		/// Величина изменения
		/// </summary>
		int amount;

		/// <summary>
		/// Число бит сдвига периода
		/// </summary>
		int shift;

		/// <summary>
		/// Инициализируем объект Sweep по входному значению типа byte
		/// </summary>
		public void Set(byte value)
		{
			/// <summary>
			/// 7й бит
			/// </summary>
			enable = (value & (1 << 7)) != 0;

			/// <summary>
			/// 6-4й бит
			/// </summary>
			period = (value >> 4) & 0x07;
			divider = new Divider(period + 1);
			/// <summary>
			/// 3й бит
			/// </summary>
			negate = (value & (1 << 3)) != 0;

			/// <summary>
			/// 2-0й бит
			/// </summary>
			shift = value & 0x07;
			amount = period >> shift;
		}

		/// <summary>
		/// Обновляем период делителя
		/// </summary>
		public void Clock()
		{
			if (enable)
			{
				if (negate)
				{
					period -= shift;
				}
				else
				{
					period += shift;
				}
			}
		}
	}
}
