namespace P3DEFBBroadcast
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusPicture = new System.Windows.Forms.PictureBox();
            this.statusText = new System.Windows.Forms.Label();
            this.infoButton = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.statusPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoButton)).BeginInit();
            this.SuspendLayout();
            // 
            // statusPicture
            // 
            this.statusPicture.Image = global::P3DEFBBroadcast.Properties.Resources.gps_off;
            this.statusPicture.Location = new System.Drawing.Point(12, 12);
            this.statusPicture.Name = "statusPicture";
            this.statusPicture.Size = new System.Drawing.Size(128, 128);
            this.statusPicture.TabIndex = 4;
            this.statusPicture.TabStop = false;
            // 
            // statusText
            // 
            this.statusText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusText.AutoSize = true;
            this.statusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusText.Location = new System.Drawing.Point(5, 157);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(70, 13);
            this.statusText.TabIndex = 5;
            this.statusText.Text = "Connecting...";
            // 
            // infoButton
            // 
            this.infoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.infoButton.Image = global::P3DEFBBroadcast.Properties.Resources.info_small;
            this.infoButton.Location = new System.Drawing.Point(132, 154);
            this.infoButton.Name = "infoButton";
            this.infoButton.Size = new System.Drawing.Size(16, 16);
            this.infoButton.TabIndex = 6;
            this.infoButton.TabStop = false;
            this.infoButton.Click += new System.EventHandler(this.InfoButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(154, 176);
            this.Controls.Add(this.infoButton);
            this.Controls.Add(this.statusText);
            this.Controls.Add(this.statusPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "P3D EFB Broadcast";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.statusPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox statusPicture;
        private System.Windows.Forms.Label statusText;
        private System.Windows.Forms.PictureBox infoButton;
    }
}

