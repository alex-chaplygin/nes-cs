using NES;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoTest
{
    public partial class form11: Form
    {
        public form11()
        {
            InitializeComponent();
            Width = 768 + 15;
            Height = 720 + 37;

            PPU.MaskWrite(0xFE);
            Memory.Write(0x2000, 0x01);
            Memory.Write(0x2001, 0xFE);
            SetupTiles();
            SetupPalette();
            SetupNames();
            SetupSprites();
	    SetupScroll();
            Cartridge.mirroring = Mirroring.FourScreen;
        }

        static void SetupTiles()
        {
            byte[] tile_bytes = new byte[] {0x41, 0xC2, 0x44, 0x48, 0x10, 0x20, 0x40, 0x80,
            0x01, 0x02, 0x04, 0x08, 0x16, 0x21, 0x42, 0x87};

            Memory.Write(0x2006, 0);
            Memory.Write(0x2006, 0);

            for (int i = 0; i < tile_bytes.Length; i++)
                Memory.Write(0x2007, tile_bytes[i]);

            string fileName = @"Tetris.nes";
            if (!Cartridge.ReadFile(fileName))
            {
                Console.WriteLine("File not found");

            }
            PPU.WritePattern0(Cartridge.GetChrBank(0));
        }

        static void SetupPalette()
        {
            byte[] palette_bytes = new byte[] { 
                0x03, 0x28, 0x15, 0x23, 
                0x0, 0x14, 0x3F, 0x12,
                0x0, 0x02, 0x04, 0x05,
                0x0, 0x07, 0x08, 0x09,

                0x0, 0x21, 0x14, 0x02,
                0x0, 0x14, 0xF, 0xA,
                0x0, 0x19, 0x15, 0xA,
                0x0, 0x2B, 0x8, 0x31
            };

            Memory.Write(0x2006, 0x3F);
            Memory.Write(0x2006, 0x00);

            for (int i = 0; i < palette_bytes.Length; i++)
                Memory.Write(0x2007, palette_bytes[i]);
        }

        static void SetupNames()
        {
            Memory.Write(0x2006, 0x20);
            Memory.Write(0x2006, 0x00);

            for (int y = 0; y < 30; y++)
                for (int x = 0; x < 32; x++)
                    if (x < 8 && y < 8)
                        Memory.Write(0x2007, (byte)(x + 8 * y));
                    else
                        Memory.Write(0x2007, 0);
            
            for (int y = 0; y < 64; y++)
                 Memory.Write(0x2007, (byte)(y % 256));

            for (int y = 0; y < 30; y++)
                for (int x = 0; x < 32; x++)
                    Memory.Write(0x2007, (byte)(x+y));

            for (int y = 0; y < 64; y++)
                Memory.Write(0x2007, 1);

            for (int y = 0; y < 30; y++)
                for (int x = 0; x < 32; x++)
                    Memory.Write(0x2007, 2);

            for (int y = 0; y < 64; y++)
                Memory.Write(0x2007, 5);

            for (int y = 0; y < 30; y++)
                for (int x = 0; x < 32; x++)
                    Memory.Write(0x2007, 3);

            for (int y = 0; y < 64; y++)
                Memory.Write(0x2007, 8);
        }

        void SetupSprites()
        {
            Memory.Write(0x2003, 0);
            for (int i = 0; i < 64; i++)
                for(int j = 0; j < 4; j++)
                    Memory.Write(0x2004, 0xFF);
            Memory.Write(0x2003, 0);
            Memory.Write(0x2004, 15);
            Memory.Write(0x2004, 0);
            Memory.Write(0x2004, 0);
            Memory.Write(0x2004, 125);

           /* Memory.Write(0x2004, 15);
            Memory.Write(0x2004, 1);
            Memory.Write(0x2004, 1);
            Memory.Write(0x2004, 125);

            Memory.Write(0x2004, 15);
            Memory.Write(0x2004, 2);
            Memory.Write(0x2004, 2);
            Memory.Write(0x2004, 125);

            Memory.Write(0x2004, 15);
            Memory.Write(0x2004, 3);
            Memory.Write(0x2004, 3);
            Memory.Write(0x2004, 125);*/
        }

        static void SetupScroll()
        {
	    Memory.Write(0x2005, 125);
            Memory.Write(0x2005, 0);
	}
	
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            unsafe
            {
                fixed (byte* ptr = PPU.GetScreen())
                {
                    Image bmp = new Bitmap(256, 240, 256 * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, new IntPtr(ptr));
                    e.Graphics.DrawImage(bmp, new Rectangle(0, 0, Width - 10, Height - 34));
                }
            }
        }
    }
}
