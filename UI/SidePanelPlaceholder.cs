using System;
using System.Windows.Forms;

namespace HashTrack
{
    public partial class SidePanelPlaceholder : UserControl
    {
        // Define a delegate for search event
        public delegate void SearchEventHandler(string keyword);

        public SidePanelPlaceholder(SidePanelWpfControl child)
        {
            InitializeComponent();
            elementHost1.Child = child;
        }


        private void UserControl1_Load(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        // Define an event based on the delegate
        public event SearchEventHandler SearchInitiated;

        // Method to call when search is initiated (e.g., button click)
        protected void OnSearch(string keyword)
        {
            SearchInitiated?.Invoke(keyword);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //string keyword = tb_search.Text;
            //OnSearch(keyword);
        }

        private void tb_search_TextChanged(object sender, EventArgs e)
        {
        }
    }
}