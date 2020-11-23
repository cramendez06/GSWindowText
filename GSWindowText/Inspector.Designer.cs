namespace GSWindowText
{
    partial class Inspector
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
            this.txtWindowText = new System.Windows.Forms.TextBox();
            this.dragPictureBox = new System.Windows.Forms.PictureBox();
            this.tbDetails = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dragPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // txtWindowText
            // 
            this.txtWindowText.Location = new System.Drawing.Point(106, 13);
            this.txtWindowText.Multiline = true;
            this.txtWindowText.Name = "txtWindowText";
            this.txtWindowText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWindowText.Size = new System.Drawing.Size(306, 93);
            this.txtWindowText.TabIndex = 1;
            // 
            // dragPictureBox
            // 
            this.dragPictureBox.Location = new System.Drawing.Point(10, 35);
            this.dragPictureBox.Name = "dragPictureBox";
            this.dragPictureBox.Size = new System.Drawing.Size(75, 71);
            this.dragPictureBox.TabIndex = 0;
            this.dragPictureBox.TabStop = false;
            this.dragPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dragPictureBox_MouseDown);
            this.dragPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dragPictureBox_MouseMove);
            this.dragPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dragPictureBox_MouseUp);
            // 
            // tbDetails
            // 
            this.tbDetails.Location = new System.Drawing.Point(106, 112);
            this.tbDetails.Name = "tbDetails";
            this.tbDetails.Size = new System.Drawing.Size(306, 20);
            this.tbDetails.TabIndex = 2;
            // 
            // Inspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbDetails);
            this.Controls.Add(this.txtWindowText);
            this.Controls.Add(this.dragPictureBox);
            this.Name = "Inspector";
            this.Size = new System.Drawing.Size(426, 144);
            ((System.ComponentModel.ISupportInitialize)(this.dragPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox dragPictureBox;
        private System.Windows.Forms.TextBox txtWindowText;
        private System.Windows.Forms.TextBox tbDetails;
    }
}
