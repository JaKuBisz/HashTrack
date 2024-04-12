using HashTrack.IoC;

namespace HashTrack
{
    partial class SidePanelPlaceholder
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.hashTrackSearchWpfControl1 = Startup.ServiceLocator.Resolve<SidePanelWpfControl>(); // new HashTrack.SidePanelWpfControl();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(268, 68);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(55, 51);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(361, 675);
            this.elementHost1.TabIndex = 4;
            this.elementHost1.Text = "elementHost2";
            this.elementHost1.Child = this.hashTrackSearchWpfControl1;
            // 
            // SidePanelPlaceholder
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.elementHost1);
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "SidePanelPlaceholder";
            this.Size = new System.Drawing.Size(361, 675);
            this.Load += new System.EventHandler(this.UserControl1_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btn_search;

        private System.Windows.Forms.Button button1;

        #endregion
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private SidePanelWpfControl hashTrackSearchWpfControl1;
    }
}
