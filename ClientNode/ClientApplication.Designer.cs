using System.Drawing;

namespace ClientNode
{
    partial class ClientApplication
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
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.textBoxReceived = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxClients = new System.Windows.Forms.ComboBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonConnectToCloud = new System.Windows.Forms.Button();
            this.buttonStopSending = new System.Windows.Forms.Button();
            this.buttonDifferentMessages = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(12, 64);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(245, 113);
            this.textBoxMessage.TabIndex = 0;
            // 
            // textBoxReceived
            // 
            this.textBoxReceived.Location = new System.Drawing.Point(270, 64);
            this.textBoxReceived.Multiline = true;
            this.textBoxReceived.Name = "textBoxReceived";
            this.textBoxReceived.ReadOnly = true;
            this.textBoxReceived.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxReceived.Size = new System.Drawing.Size(245, 113);
            this.textBoxReceived.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Message to send";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(280, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Received message";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxLog.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxLog.Location = new System.Drawing.Point(12, 270);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(503, 95);
            this.textBoxLog.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 254);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Log";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Choose client";
            // 
            // comboBoxClients
            // 
            this.comboBoxClients.FormattingEnabled = true;
            this.comboBoxClients.Location = new System.Drawing.Point(12, 195);
            this.comboBoxClients.Name = "comboBoxClients";
            this.comboBoxClients.Size = new System.Drawing.Size(161, 21);
            this.comboBoxClients.TabIndex = 7;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(187, 194);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(70, 22);
            this.buttonSend.TabIndex = 8;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // buttonConnectToCloud
            // 
            this.buttonConnectToCloud.Location = new System.Drawing.Point(13, 13);
            this.buttonConnectToCloud.Name = "buttonConnectToCloud";
            this.buttonConnectToCloud.Size = new System.Drawing.Size(502, 23);
            this.buttonConnectToCloud.TabIndex = 9;
            this.buttonConnectToCloud.Text = "CONNECT AND LISTEN";
            this.buttonConnectToCloud.UseVisualStyleBackColor = true;
            this.buttonConnectToCloud.Click += new System.EventHandler(this.buttonConnectToCloud_Click);
            // 
            // buttonStopSending
            // 
            this.buttonStopSending.Location = new System.Drawing.Point(13, 222);
            this.buttonStopSending.Name = "buttonStopSending";
            this.buttonStopSending.Size = new System.Drawing.Size(244, 23);
            this.buttonStopSending.TabIndex = 10;
            this.buttonStopSending.Text = "Stop sending";
            this.buttonStopSending.UseVisualStyleBackColor = true;
            this.buttonStopSending.Click += new System.EventHandler(this.buttonStopSending_Click);
            // 
            // buttonDifferentMessages
            // 
            this.buttonDifferentMessages.Location = new System.Drawing.Point(270, 195);
            this.buttonDifferentMessages.Name = "buttonDifferentMessages";
            this.buttonDifferentMessages.Size = new System.Drawing.Size(246, 50);
            this.buttonDifferentMessages.TabIndex = 11;
            this.buttonDifferentMessages.Text = "Continuous sending of different length messages";
            this.buttonDifferentMessages.UseVisualStyleBackColor = true;
            this.buttonDifferentMessages.Click += new System.EventHandler(this.buttonDifferentMessages_Click);
            // 
            // ClientApplication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(528, 378);
            this.Controls.Add(this.buttonDifferentMessages);
            this.Controls.Add(this.buttonStopSending);
            this.Controls.Add(this.buttonConnectToCloud);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.comboBoxClients);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxReceived);
            this.Controls.Add(this.textBoxMessage);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "ClientApplication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClientNode";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.TextBox textBoxReceived;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxClients;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonConnectToCloud;
        private System.Windows.Forms.Button buttonStopSending;
        private System.Windows.Forms.Button buttonDifferentMessages;
    }
}

