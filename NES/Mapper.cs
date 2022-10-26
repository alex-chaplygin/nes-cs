using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    /// <summary>
    /// Класс для переключения банков памяти 
    /// </summary>
    public class Mapper
    {
        /// <summary>
        /// Инициализация Mapper
        /// </summary>
        public static void Init() // Крюков Никита
        {
           Memory.WriteROM1(Cartridge.GetPrgBank(0));
           Memory.WriteROM2(Cartridge.GetPrgBank(Cartridge.prg_count - 1));
        }

        public static void Write(ushort adr, byte value)
        {

        }
    }
}
