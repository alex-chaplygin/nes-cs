using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.Mappers
{
    /// <summary>
    /// Маппер MMC3 позволяет переключать по 8К PRG-ROM банка (ПЗУ программы, 32К памяти) из диапазона 0x8000 - 0xFFFF (диапазон делится на 4 части) и
    /// может переключать CHR банка (всего в CHR 8К памяти) по 2К и 1К из того же диапазона (диапазон делится на 4 и 8 частей соответственно)
    /// Банк берется из класса картриджа и записывается либо в основную память (Memory) либо в видеопамять (PPU)
    /// </summary>
    class MMC3
    {

        /// <summary>
        /// Функция переключения
        /// </summary>
        /// <param name="val"></param>
        delegate void SwitchOp(byte val);
        struct SwitchReg
        {
            /// <summary>
            /// Верхняя граница
            /// </summary>
            public int up;

            /// <summary>
            /// Нижняя граница
            /// </summary>
            public int down;

            /// <summary>
            /// Функция переключения
            /// </summary>
            public SwitchOp bank;

            public SwitchReg(int p, int u, SwitchOp r)
            {
                up = p;
                down = u;
                bank = r;
            }
        }


        static SwitchReg[] regsEven = new SwitchReg[]
        {
            new SwitchReg (0x8001, 0x9FFF, Bank_select ),
            new SwitchReg(0xA000, 0xBFFE, Mirroring_  ),
            new SwitchReg (0xC000, 0xDFFF, IRQ_latch ),
            new SwitchReg (0xE000, 0xFFFE, IRQ_disable ),
        };

        static SwitchReg[] regsOdd = new SwitchReg[]
        {
            new SwitchReg (0x8001, 0x9FFF, Bank_data ),
            new SwitchReg (0xA001, 0xBFFF, PRG_RAM_protect ),
            new SwitchReg (0xC001, 0xDFFE, IRQ_reload ),
            new SwitchReg (0xE001, 0xFFFF, IRQ_enable )
        };

        public static void Write(ushort adr, byte val)
        {
            if (adr % 1 == 0)
            {
                for (int i = 0; i < regsEven.Length; i++)
                    if (regsEven[i].up <= adr && adr <= regsEven[i].down)
                        regsEven[i].bank(val);
            }
            else
            {
                for (int i = 0; i < regsOdd.Length; i++)
                    if (regsEven[i].up <= adr && adr <= regsEven[i].down)
                        regsEven[i].bank(val);
            }
        }

        public static void Bank_select(byte val)
        { }
        public static void Mirroring_(byte val)
        { }
        public static void IRQ_latch(byte val)
        { }
        public static void IRQ_disable(byte val)
        { }


        public static void Bank_data(byte val)
        { }
        public static void PRG_RAM_protect(byte val)
        { }
        public static void IRQ_reload(byte val)
        { }
        public static void IRQ_enable(byte val)
        { }
    }
}
