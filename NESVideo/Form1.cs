using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NES;

namespace NESVideo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "NES files(*.nes)|*.nes|All files(*.*)|*.*";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            PPU.Init();
            Cartridge.ReadFile(openFileDialog1.FileName);
	        Memory.WriteROM1(Cartridge.GetPrgBank(0));            
            Memory.WriteROM2(Cartridge.GetPrgBank(Cartridge.prg_count-1));
	        if (Cartridge.chr_count > 0)
            PPU.WritePattern0(Cartridge.GetChrBank(0));
            CPU.Interrupt(Interruption.RESET);
            timer1.Start();
	}

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PPU.Init();
            CPU.Interrupt(Interruption.RESET);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            int cycles = 0;
            while (cycles < 29780)
                cycles += CPU.Step();
            PPU.VBlankStart();
            cycles = 0;
            while (cycles < 4780)
                cycles += CPU.Step();
            PPU.VBlankStop();
            Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Z:
                    Input.KeyDown(Key.A);
                    break;
                case Keys.X:
                    Input.KeyDown(Key.B);
                    break;
                case Keys.Up:
                    Input.KeyDown(Key.Up);
                    break;
                case Keys.Right:
                    Input.KeyDown(Key.Right);
                    break;
                case Keys.Down:
                    Input.KeyDown(Key.Down);
                    break;
                case Keys.Left:
                    Input.KeyDown(Key.Left);
                    break;
                case Keys.Space:
                    Input.KeyDown(Key.Select);
                    break;
                case Keys.Enter:
                    Input.KeyDown(Key.Start);
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Z:
                    Input.KeyUp(Key.A);
                    break;
                case Keys.X:
                    Input.KeyUp(Key.B);
                    break;
                case Keys.Up:
                    Input.KeyUp(Key.Up);
                    break;
                case Keys.Right:
                    Input.KeyUp(Key.Right);
                    break;
                case Keys.Down:
                    Input.KeyUp(Key.Down);
                    break;
                case Keys.Left:
                    Input.KeyUp(Key.Left);
                    break;
                case Keys.Space:
                    Input.KeyUp(Key.Select);
                    break;
                case Keys.Enter:
                    Input.KeyUp(Key.Start);
                    break;
            }
        }
    }
}
