namespace Client
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.connectBtn = new System.Windows.Forms.Button();
            this.HeartMessageSendBtn = new System.Windows.Forms.Button();
            this.HeartBeatSendGetResBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(75, 309);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(103, 30);
            this.connectBtn.TabIndex = 0;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // HeartMessageSendBtn
            // 
            this.HeartMessageSendBtn.Location = new System.Drawing.Point(75, 356);
            this.HeartMessageSendBtn.Name = "HeartMessageSendBtn";
            this.HeartMessageSendBtn.Size = new System.Drawing.Size(103, 34);
            this.HeartMessageSendBtn.TabIndex = 1;
            this.HeartMessageSendBtn.Text = "HeartBeat Send";
            this.HeartMessageSendBtn.UseVisualStyleBackColor = true;
            this.HeartMessageSendBtn.Click += new System.EventHandler(this.HeartMessageSendBtn_Click);
            // 
            // HeartBeatSendGetResBtn
            // 
            this.HeartBeatSendGetResBtn.Location = new System.Drawing.Point(253, 393);
            this.HeartBeatSendGetResBtn.Name = "HeartBeatSendGetResBtn";
            this.HeartBeatSendGetResBtn.Size = new System.Drawing.Size(134, 32);
            this.HeartBeatSendGetResBtn.TabIndex = 2;
            this.HeartBeatSendGetResBtn.Text = "HeartBeatSend Get Response";
            this.HeartBeatSendGetResBtn.UseVisualStyleBackColor = true;
            this.HeartBeatSendGetResBtn.Click += new System.EventHandler(this.HeartBeatSendGetResBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.HeartBeatSendGetResBtn);
            this.Controls.Add(this.HeartMessageSendBtn);
            this.Controls.Add(this.connectBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.Button HeartMessageSendBtn;
        private System.Windows.Forms.Button HeartBeatSendGetResBtn;
    }
}

