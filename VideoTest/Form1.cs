using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NES;

namespace VideoTest
{
    public partial class form11 : Form
    {
        //byte[] pixelArray = new byte[256 * 240*3];
        public form11()
        {            
            InitializeComponent();
            //for (int i = 0; i < 256 * 240 * 3; i++)
            //    if (i % 3 == 2) pixelArray[i] = 200;
            PPU.MaskWrite(0xFF);
        }
      
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            unsafe
            {
                fixed (byte* ptr = PPU.GetScreen())
                {
                    Image bmp = new Bitmap(256, 240, 256 * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, new IntPtr(ptr));
                    e.Graphics.DrawImage(bmp, new Rectangle(0, 0, Width, Height));
                }
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    

}
