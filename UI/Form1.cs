using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.elementHost1.Child = null;
        }
    }
}
