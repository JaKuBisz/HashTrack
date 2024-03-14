using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }


        private void UserControl1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }
        // Define a delegate for search event
        public delegate void SearchEventHandler(string keyword);
        // Define an event based on the delegate
        public event SearchEventHandler SearchInitiated;

        // Method to call when search is initiated (e.g., button click)
        protected void OnSearch(string keyword)
        {
            SearchInitiated?.Invoke(keyword);
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            string keyword = tb_search.Text;
            OnSearch(keyword);
        }

        private void tb_search_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
