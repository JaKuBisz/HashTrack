using System;
using System.Windows.Forms;

namespace HashTrack.UI
{
    public partial class Form1 : Form
    {
        public Form1(HashTagSettings child)
        {
            InitializeComponent();
            elementHost1.Child = child;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            elementHost1.Child = null;
        }
    }
}