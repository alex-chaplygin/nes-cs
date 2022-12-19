﻿using System;
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
            Cartridge.ReadFile(openFileDialog1.FileName);
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CPU.Interrupt(Interruption.RESET);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
