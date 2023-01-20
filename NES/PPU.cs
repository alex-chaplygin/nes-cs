﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{

    /// <summary>
    ///   Графический процессор
    /// </summary>
    public class PPU
    {
        const int PPU_MEM_SIZE = 0x4000;
        public const int PATTERN_TABLE_0 = 0x0;
        public const int PATTERN_TABLE_1 = 0x1000;
        public const int NAME_TABLE = 0x2000;
        public const int PALETTE = 0x3F00;
        public const int WIDTH_TILES = 32;
        public const int HEIGHT_TILES = 30;
        public const int OAM_SIZE = 0x100;
        const int ATTRIBUTE_GRID = 0x23C0;
        const int MAX_SPRITES = 64;

        /// <summary>
        /// структура видимого спрайта
        /// </summary>
        struct Sprite
        {
            public int id;
            /// <summary>
            /// координат x левого верхнего угла спрайта
            /// </summary>
            public int x;
            /// <summary>
            /// координата y левого верхнего угла срайта
            /// </summary>
            public int y;
            /// <summary>
            /// номер тайла спрайта
            /// </summary>
            public int tile;
            /// <summary>
            /// младшие два бита - номер палитры спрайта
            /// </summary>
            public int atribute;

            public Sprite(int x, int y, int tile, int atribute, int id)
            {
                this.x = x;
                this.y = y;
                this.tile = tile;
                this.atribute = atribute;
                this.id = id;
            }
        } 

        /// <summary>
        /// Ширина экрана в точках
        /// </summary>
        public const int WIDTH = 256;

        /// <summary>
        /// Высота экрана в точках
        /// </summary>
        public const int HEIGHT = 240;

        /// <summary>
        /// Память видеопроцессора
        /// </summary>
        public static byte[] memory = new byte[PPU_MEM_SIZE];

        /// <summary>
        /// Память спрайтов
        /// </summary>
        public static byte[] OAM_memory = new byte[OAM_SIZE];

        /// <summary>
        /// список рисуемых спрайтов
        /// </summary>
        static List<Sprite> sprites = new List<Sprite>();

        /// <summary>
        /// текущий рисуемый спрайт
        /// </summary>
        static Sprite current_sprite;

        /// <summary>
        /// Палитра 64 цвета
        /// </summary>
        public static byte[] palette = new byte[] {
            84, 84, 84, 0, 30, 116, 8, 16, 144, 48, 0, 136, 68, 0, 100, 92, 0, 48, 84, 4, 0, 60, 24, 0, 32, 42, 0, 8, 58, 0, 0, 64, 0, 0, 60, 0, 0, 50, 60, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            152, 150, 152, 8, 76, 19, 48, 50, 236, 92, 30, 228, 136, 20, 176, 160, 20, 100, 152, 34, 32, 120, 60, 0, 84, 90, 0, 40, 114, 0, 8, 124, 0, 0, 118, 40, 0, 102, 120, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            236, 238, 236, 76, 154, 236, 120, 124, 236, 176, 98, 236, 228, 84, 236, 236, 88, 180, 236, 106, 100, 212, 136, 32, 160, 170, 0, 116, 196, 0, 76, 208, 32, 56, 204, 108, 56, 180, 204, 60, 60, 60, 0, 0, 0, 0, 0, 0,
            236, 238, 236, 168, 204, 236, 188, 188, 236, 212, 178, 236, 236, 174, 236, 236, 174, 212, 236, 180, 176, 228, 196, 144, 204, 210, 120, 180, 222, 120, 168, 226, 144, 152, 226, 180, 160, 214, 228, 160, 162, 160, 0, 0, 0, 0, 0, 0,
        };

        /// <summary>
        /// Массив точек экрана
        /// </summary>
        public static byte[] screen = new byte[WIDTH * HEIGHT * 3];

        /// <summary>
        /// позиция в массиве экрана
        /// </summary>
        public static int screen_pos;

        /// <summary>
        /// Номер экрана
        /// </summary>
        public static int nametable;

        /// <summary>
        /// Если переменная = 0, то к адресу добавляется 1, если переменная = 1, то к адресу добавляется 32
        /// </summary>
        public static int increment;

        /// <summary>
        /// Номер таблицы шаблонов для спрайтa 8x8  0: $0000; 1: $1000. Для спрайтa 8x16 игнорируется
        /// </summary>
        public static int sprite_table;

        /// <summary>
        /// Номер таблицы для фона; 0: $0000; 1: $1000
        /// </summary>
        public static int background_table;

        /// <summary>
        /// Размер спрайта. Если 0, то 8x8, если 1, то 8x16
        /// </summary>
        public static int sprite_size;

        /// <summary>
        /// Если 0, то прерывание НЕ генерируется, если 1, то генерируется
        /// </summary>
        public static bool generate_nmi;

        /// <summary>
        /// Если 0, то обычные цвета, если 1 то изображение становится чёрн-белым
        /// </summary>
        public static bool greyscale;

        /// <summary>
        /// Если 1, то задний фон показывается в самых левых 8 пикселях, если 0, то спрятан
        /// </summary>
        public static bool show_8_background;

        /// <summary>
        /// Если 1, то спрайты показываются в самых левых 8 пикселях, если 0, то спрятаны
        /// </summary>
        public static bool show_8_sprites;

        /// <summary>
        /// Если 1, то задний фон показывается, если 0, то нет
        /// </summary>
        public static bool show_background;

        /// <summary>
        /// Если 1, то спрайты показываются, если 0, то нет
        /// </summary>
        public static bool show_sprites;

        /// <summary>
        /// Если 0, то цвет теряет значение красного (= 0), если 1, то ничего
        /// </summary>
        public static bool red_available;

        /// <summary>
        /// Если 0, то цвет теряет значение зеленого (= 0), если 1, то ничего
        /// </summary>
        public static bool green_available;

        /// <summary>
        /// Если 0, то цвет теряет значение синего (= 0), если 1, то ничего
        /// </summary>
        public static bool blue_available;

        /// <summary>
        ///   Регистр адреса
        /// </summary>
        public static ushort address;

        /// <summary>
        ///   Регистр адреса OAM
        /// </summary>
        public static byte OAM_address;

        /// <summary>
        /// Буффер памяти
        /// </summary>
        public static byte read_buffer;

        /// <summary>
        /// Регистр позиции прокрутки ППУ
        /// </summary>
        public static ushort scroll;

        /// <summary>
        ///   Первая запись в адрес
        /// </summary>
        static bool isFirst = true;

        /// <summary>
        /// Флаг переполнения спрайтов
        /// </summary>
        public static bool sprite_overflow;

        /// <summary>
        /// Флаг наложения непустого пикселя спрайта 0 на непустой пиксель фона
        /// </summary>
        public static bool sprite_0_hit;

        /// <summary>
        /// Флаг кадровой развертки
        /// </summary>
        public static bool vertical_blank;

        /// <summary>
        ///   Координата x внутри тайла от 0 до 7.
        /// </summary>
        static byte tile_x;


        /// <summary>
        ///   Координата y внутри тайла от 0 до 7.
        /// </summary>
        static byte tile_y;


        /// <summary>
        /// Функция чтения из памяти
        /// </summary>
        /// <returns></returns>
        delegate byte MemoryRead();

        /// <summary>
        /// Функция записи в память
        /// </summary>
        /// <param name="value">Значение, записываемое в память</param>
        delegate void MemoryWrite(byte value);

        struct Register
        {
            /// <summary>
            /// Адрес
            /// </summary>
            public int adr;

            /// <summary>
            ///   Функция чтения
            /// </summary>
            public MemoryRead read;

            /// <summary>
            ///   Функция записи
            /// </summary>
            public MemoryWrite write;

            public Register(int a, MemoryRead r, MemoryWrite w)
            {
                adr = a;
                read = r;
                write = w;
            }
        }

        static Register[] memoryTable = new Register[]
        {
            new Register( 0x2000, null, ControllerWrite ),
        new Register( 0x2001, null, MaskWrite ),
            new Register( 0x2002, StatusRead, null ),
            new Register( 0x2003, null, OAMadrWrite ),
            new Register( 0x2004, OAMRead, OAMWrite ),
            new Register( 0x2005, null, SetScroll ),
            new Register( 0x2006, null, SetAddress ),
            new Register( 0x2007, DataRead, DataWrite ),
            new Register( 0x4014, null, DMA )
        };

        /// <summary>
        /// Инициализация PPU
        /// </summary>
        public static void Init()
        {
            ControllerWrite(0x0);
            MaskWrite(0x0);
            vertical_blank = true;
            sprite_0_hit = false;
            sprite_overflow = true;
            OAMadrWrite(0x0);
            SetAddress(0x0);
            SetScroll(0x0);
            DataWrite(0x0);
        }

        /// <summary>
        ///   Запись в регистр PPU
        /// </summary>
        public static void Write(ushort adr, byte value)
        {
            for (int i = 0; i < memoryTable.Length; i++)
                if (adr == memoryTable[i].adr)
                {
                    if (memoryTable[i].write == null)
                        return;
                    else
                    {
                        memoryTable[i].write(value);
                        return;
                    }
                }
        }

        /// <summary>
        ///   Чтение из регистра PPU
        /// </summary>
        public static byte Read(ushort adr)
        {
            for (int i = 0; i < memoryTable.Length; i++)
                if (adr == memoryTable[i].adr)
                {
                    if (memoryTable[i].read == null)
                        return 0;
                    else
                    {
                        return memoryTable[i].read();
                    }
                }
            return 0;
        }

        /// <summary>
        ///   Записать адрес, сначала старший байт, затем младший
        /// </summary>
        public static void SetAddress(byte val)
        {
            if (isFirst)
            {
                address = (ushort)(val << 8 | (address & 0xFF));
                isFirst = false;
            }
            else
            {
                address = (ushort)(address & 0xFF00);
                address = (ushort)(address + val);
                isFirst = true;
            }
        }

        /// <summary>
        /// Увеличить адрес, если increment=0, то на 1; если increment=1, то на 32
        /// </summary>
        /// <param name="adr"></param>
        static void IncreaseAddress()
        {
            if (increment == 0)
                address++;
            else if (increment == 1)
                address += 32;
        }

        /// <summary>
        /// Записать в память байт по адресу
        /// </summary>
        /// <param name="bt"></param>
        public static void DataWrite(byte bt)
        {
            memory[address & 0x3FFF] = bt;
            IncreaseAddress();
        }

        /// <summary>
        /// Прочитать из памяти байт по адресу
        /// </summary>
        /// <param name="bt"></param>
        static byte DataRead()
        {
            byte b = read_buffer;
            read_buffer = memory[address];
            IncreaseAddress();
            return b;
        }

        /// <summary>
        ///   Запись в память спрайтов
        /// </summary>
        public static void OAMWrite(byte value)
        {
            OAM_memory[OAM_address++] = value;
        }

	/// Подушкин Иван
        /// <summary>
        /// Задание адреса OAM
        /// </summary>
        /// <param name="value"></param>
        public static void OAMadrWrite(byte value)
        {
            OAM_address = value;
        }
	
        /// <summary>
        ///   Чтение из памяти спрайтов
        /// </summary>
        public static byte OAMRead()
        {
            if (vertical_blank)
                return OAM_memory[OAM_address];
            else
                return OAM_memory[OAM_address++];
        }

        /// <summary>
        ///   Direct Memory Access  
        /// </summary>
        public static void DMA(byte value)
        {
            for (int i = 0; i < OAM_SIZE; i++)
                OAMWrite(Memory.Read((ushort)((value << 8) + i)));
        }

        /// <summary>
        /// Записать таблицу шаблонов 0
        /// </summary>
        /// <param name="data">Данные шаблонов</param>
        public static void WritePattern0(byte[] data)
        {
            data.CopyTo(memory, PATTERN_TABLE_0);
        }

        /// <summary>
        /// Записать таблицу шаблонов 1
        /// </summary>
        /// <param name="data">Данные шаблонов</param>
        public static void WritePattern1(byte[] data)
        {
            data.CopyTo(memory, PATTERN_TABLE_1);
        }

        // Акименко Максим 
        /// <summary>
        /// первый раз старший байт регистра прокрутки второй младший байт регистра прокрутки
        /// </summary>
        /// <param name="val">старший или младший байт</param>
        static void SetScroll(byte val)
        {
            if (isFirst)
            {
                scroll = (ushort)(scroll & 0xFF);
                scroll = (ushort)(scroll | val<<8);
                isFirst = false;
            }
            else
            {
                scroll = (ushort)(scroll & 0xFF00);
                scroll = (ushort)(scroll | val);
                isFirst = true;
            }
        }

        /// <summary>
        /// Записать в регистр управления
        /// </summary>
        /// <param name="val">значение</param>
        public static void ControllerWrite(byte val)
        {
            nametable = val & 3;
            increment = (val >> 2) & 1;
            sprite_table = (val >> 3) & 1;
            background_table = (val >> 4) & 1;
            sprite_size = (val >> 5) & 1;
            generate_nmi = ((val >> 7) & 1) > 0;
        }

	delegate ushort MirrorReturn(ushort adr);
	struct MirAdr
        {
            public ushort adr;

            public MirrorReturn ret;

            public MirAdr(ushort a, MirrorReturn r)
            {
                adr = a;
                ret = r;
            }
        }
	
	static MirAdr[] mirroringTable = new MirAdr[]
        {
            new MirAdr(0x2FFF, MirrorAdrType),
            new MirAdr(0x3EFF, MirrorOnMir),
            new MirAdr(0x3F1F, MirrorPalRAM),
            new MirAdr(0x3FFF, MirrorOnPalRAM)
        };

	static ushort MirrorAdrType(ushort adr)
        {

            if (Cartridge.mirroring == Mirroring.Horisontal)
                if (((adr >= 0x2400) && (adr < 0x2800)) || (adr >= 0x2C00))
                    adr -= 0x400;
            if (Cartridge.mirroring == Mirroring.Vertical)
                if (adr >= 0x2800)
                    adr -= 0x800;
            if (Cartridge.mirroring == Mirroring.Single)
                if (adr >= 0x2400)
                {
                    while (adr >= 0x2400)
                        adr -= 0x400;
                }
            return adr;
        }

        static ushort MirrorOnMir(ushort adr)
        {
            adr -= 0x1000;
            return MirrorAdrType(adr);
        }

	static ushort MirrorPalRAM(ushort adr)
        {
            if ((adr == 0x3F10) || (adr == 0x3F14) || (adr == 0x3F18) || (adr == 0x3F1C))
                adr -= 0x10;
            return adr;
        }

	static ushort MirrorOnPalRAM(ushort adr)
        {
            while(adr >= 0x3F1F)
                adr -= 0x20;
            return adr;
        }
	
        /// <summary>
        /// Вычисляет настоящий адрес на основе зеркалированного адреса
        /// </summary>
        /// <param name="adr">Зеркалированный адрес</param>
        /// <returns>Настоящий адрес</returns>
        public static ushort MirrorAdr(ushort adr)
        {
            for(int i = 0; i < mirroringTable.Length; i++)
            {
                if(adr <= mirroringTable[i].adr)
                {
                    return mirroringTable[i].ret(adr);
                }
            }
            return 0;
        }

        /// <summary>
        /// Извлечь значения маски PPU
        /// </summary>
        /// <param name="val">значение</param>
        public static void MaskWrite(byte val)
        {
            greyscale = (val & 1) > 0;
            show_8_background = ((val >> 1) & 1) > 0;
            show_8_sprites = ((val >> 2) & 1) > 0;
            show_background = ((val >> 3) & 1) > 0;
            show_sprites = ((val >> 4) & 1) > 0;
            red_available = true;// ((val >> 5) & 1) > 0;
            green_available = true;// ((val >> 6) & 1) > 0;
            blue_available = true;// ((val >> 7) & 1) > 0;
        }

        /// <summary>
        /// Чтение регистра статуса
        /// </summary>
        /// <returns></returns>
        public static byte StatusRead()
        {
            byte result = (byte)((Convert.ToByte(sprite_overflow) << 5) | (Convert.ToByte(sprite_0_hit) << 6) | (Convert.ToByte(vertical_blank) << 7));
            isFirst = true;
            vertical_blank = false;
            return result;
        }

        /// <summary>
        /// Вычислить адрес, где хранятся данные строчки тайла
        /// </summary>
        /// <param name="tile">номер тайла 0-255</param>
        /// <param name="tile_y">строчка тайла 0-7</param>
        /// <param name="table">номер таблицы тайлов 0-1</param>
        /// <returns></returns>
        public static ushort GetTileAdr(byte tile, int tile_y, int table)
        {
            ushort adr = (ushort)(16 * tile + tile_y + table * 0x1000);
            return adr;
        }

        /// <summary>
        /// Отрисовать очередной пиксель
        /// </summary>
        /// <param name="value">Номер цвета палитры</param>
        static void RenderPixel(int value)
        {
            if (greyscale)
                value &= 0x30;
            screen[screen_pos] = blue_available ? palette[value * 3 + 2] : (byte)0;
            screen[screen_pos + 1] = green_available ? palette[value * 3 + 1] : (byte)0;
            screen[screen_pos + 2] = red_available ? palette[value * 3] : (byte)0;
            screen_pos += 3;
        }

        /// <summary>
        /// Основной цикл отрисовки
        /// </summary>
        /// <returns>Массив точек экрана</returns>
        public static byte[] GetScreen()
        {
            BeginFrame();

            for (int i = 0; i < HEIGHT; i++)
            {
                ushort address2 = address;
                EvaluateSprites(i);
                for (int j = 0; j < WIDTH; j++)
                {
                    int back_color;
                    int sprite_color;
                    if (j < 8)
                    {
                        show_8_background = show_background ? show_8_background : false;
                        back_color = show_8_background ? GetTilePixel(memory[address], tile_x, tile_y, background_table) : 0;
                        show_8_sprites = show_sprites ? show_8_sprites : false;
                        sprite_color = show_8_sprites ? GetSpriteColor(j, i) : 0;
                    }
                    else
                    {
                        back_color = show_background ? GetTilePixel(memory[address], tile_x, tile_y, background_table) : 0;
                        sprite_color = show_sprites ? GetSpriteColor(j, i) : 0;
                    }
                    int atr = GetAttribute();
                    int color = Combine(back_color, sprite_color, ref atr);
                    int pixel = GetPalettePixel(color, atr, j);
                    RenderPixel(pixel);
                    tile_x++;
                    if (tile_x == 8)
                        NextTile();
                }
                EndLine(address2);
            }
            return screen;
        }

        /// <summary>
        /// Вычисление цвета пикселя при наложении спрайта на фон
        /// </summary>
        /// <param name="back_color">Цвет пикселя фона</param>
        /// <param name="sprite_color">Цвет пикселя спрайта</param>
        /// <param name="atr">возвращаемый номер палитры для спрайта</param>
        /// <returns>Финальный цвет пикселя</returns>
        static int Combine(int back_color, int sprite_color, ref int atr)
        {
if (sprite_color == 0)
                return back_color;
            if (current_sprite.id == 0 && back_color != 0)
                sprite_0_hit = true;
            int ret = back_color == 0 ? sprite_color : back_color;
            if (!GetPriority(current_sprite))
            {
                atr = (current_sprite.atribute & 3) + 4;
                return sprite_color;
            }
            else
		return ret;
        }

        /// <summary>
        /// Подготовитеьные действия в начале кадра
        /// </summary>
        static void BeginFrame()
        {
            screen_pos = 0;
            byte scroll_y = (byte)scroll;
            byte scroll_x = (byte)(scroll >> 8);
            byte coarse_x = (byte)(scroll_x >> 3);
            byte coarse_y = (byte)(scroll_y >> 3);
            tile_x = (byte)(scroll_x & 0x07);
            tile_y = (byte)(scroll_y & 0x07);
            sprite_overflow = false;
            sprite_0_hit = false;
            address = MirrorAdr((ushort)(NAME_TABLE + (nametable << 10) + (coarse_y << 5) + coarse_x));
        }


        /// <summary>
        /// Перейти на следующую плитку
        /// </summary>
        static void NextTile()
        {
            if (GetCurX() == 31)
            {
                address = (ushort)(address ^ 0x400);
                address = (ushort)(address & 0xFFE0);
            }
            else
                address++;
            tile_x = 0;
        }

        /// <summary>
        /// Окончание отрисовки строки
        /// </summary>
        static void EndLine(ushort address2)
        {
            tile_y++;
            address = address2;

            if (tile_y == 8)
            {
                tile_y = 0;
                address += WIDTH_TILES;
            }
        }

        /// <summary>
        /// Получение текущего пикселя плитки
        /// </summary>
        static int GetTilePixel(byte tile, int tile_x, int tile_y, int table)
        {
            ushort a = GetTileAdr(tile, tile_y, table);
            byte low = memory[a];
            byte up = memory[a + 8];

            int l = (low >> (7 - tile_x)) & 1;
            int u = (up >> (7 - tile_x)) & 1;

            int color = l + (u << 1);

            return color;
        }


        /// <summary>
        /// получить точку всех наложенных спрайтов
        /// </summary>
        /// <param name="x">координата x экрана(от 0 до 255)</param>
        /// <param name="y">координата y экрана(от 0 до 239)</param>
        /// <returns></returns>
        static int GetSpriteColor(int x, int y)
        {
            int sprite_height = sprite_size * 8 + 7;
            foreach (Sprite sprite in sprites)
            {
                if (sprite.x > x - 8 && sprite.x <= x)
                {
                    // проверить наложение спрайтов
                    int color = GetTilePixel((byte)sprite.tile,
                        GetHFlip(sprite) ? 7 - x + sprite.x : x - sprite.x,
                        GetVFlip(sprite) ? sprite_height - y + sprite.y : y - sprite.y, sprite_table);
                    if (color != 0)
                    {
                        current_sprite = sprite;
                        return color;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// получить координату x текущего тайла
        /// </summary>
        /// <returns></returns>
        static int GetCurX()
        {
            return address & 0x1F;
        }

        /// <summary>
        /// получить координату y текущего тайла
        /// </summary>
        /// <returns></returns>
        static int GetCurY()
        {
            return (address >> 5) & 0x1F;
        }

        /// <summary>
        /// Получить номер палитры тайла по аттрибуту
        /// </summary>
        /// <returns></returns>
        static byte GetAttribute()
        {
            int x = GetCurX();
            int y = GetCurY();
            byte attr = memory[0x3c0 + NAME_TABLE + nametable * 0x400 + x / 4  + y / 4 * 8];
            //int b2 = attr / 8 % 8;

            int ix = x % 4;
            int iy = y % 4;

            if (ix <= 1 && iy <= 1) return (byte)(attr & 3);
            if (ix > 1 && iy <= 1) return (byte)((attr >> 2) & 3);
            if (ix <= 1 && iy > 1) return (byte)((attr >> 4) & 3);
            if (ix > 1 && iy > 1) return (byte)(attr >> 6);
            return 0;
        }


        /// <summary>
        /// Окраска пикселя по палитре
        /// </summary>
        static int GetPalettePixel(int color, int palette, int x)
        {
            if (color == 0)
            {
                if (!show_background || !show_8_background && x < 8)
                    return 0x0d;
                else
                    palette = 0;
            }
            return memory[PALETTE + palette * 4 + color];
        }

        /// <summary>
        /// Вычилить первые 8 спрайтов на заданой строке
        /// </summary>
        /// <param name="line_y">номер строки</param>
        static void EvaluateSprites(int line_y)
        {
            int sprite_height = sprite_size * 8 + 8;
            sprites.Clear();
            for (int i = 0; i < MAX_SPRITES; i++)
            {
                int y = OAM_memory[i * 4];
                int tile = OAM_memory[i * 4 + 1];
                int atr = OAM_memory[i * 4 + 2];
                int x = OAM_memory[i * 4 + 3];

                if (y <= line_y && y + sprite_height - 1 >= line_y)
                {
                    if (sprites.Count >= 8)
                    {
                        sprite_overflow = true;
                        return;
                    }
                    sprites.Add(new Sprite(x, y, tile, atr, i));
                }
            }
        }
        public static void VBlankStart()
        {
            vertical_blank = true;
            if (generate_nmi)
                CPU.Interrupt(Interruption.NMI);
        }
        public static void VBlankStop()
        {
            vertical_blank = false;
            sprite_0_hit = false;
        }

        /// <summary>
        /// возвращает аттрибут спрайта - поворот по горизонтали
        /// </summary>
        /// <param name="s">Спрайт</param>
        /// <returns></returns>
        static bool GetHFlip(Sprite s)
        {
            return Convert.ToBoolean((s.atribute >> 6) & 1);
        }

        /// <summary>
        /// возвращает аттрибут спрайта - поворот по вертикали
        /// </summary>
        /// <param name="s">Спрайт</param>
        /// <returns></returns>
        static bool GetVFlip(Sprite s)
        {
            return Convert.ToBoolean(s.atribute >> 7);
        }

        /// <summary>
        /// возвращает аттрибут спрайта - приоритет
        /// </summary>
        /// <param name="s">Спрайт</param>
        /// <returns>true- спрайт перед фоном, false - спрайт позади фона </returns>
        static bool GetPriority(Sprite s)
        {
            return Convert.ToBoolean((s.atribute >> 5) & 1);
        }
    }
}
