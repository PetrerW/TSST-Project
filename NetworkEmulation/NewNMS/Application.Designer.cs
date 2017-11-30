namespace NewNMS
{
    partial class Application
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
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonListen = new System.Windows.Forms.Button();
            this.listBoxReceived = new System.Windows.Forms.ListBox();
            this.comboBoxRouters = new System.Windows.Forms.ComboBox();
            this.comboBoxActions = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxfrequency_IN = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPort_in = new System.Windows.Forms.TextBox();
            this.comboBoxTables = new System.Windows.Forms.ComboBox();
            this.labelTable = new System.Windows.Forms.Label();
            this.textBox_IP_IN = new System.Windows.Forms.TextBox();
            this.textBox_Port_IN = new System.Windows.Forms.TextBox();
            this.textBoxBand = new System.Windows.Forms.TextBox();
            this.textBoxFrequency = new System.Windows.Forms.TextBox();
            this.textBoxModulation = new System.Windows.Forms.TextBox();
            this.textBoxBitrate = new System.Windows.Forms.TextBox();
            this.textBoxCloud_IP = new System.Windows.Forms.TextBox();
            this.textBoxPort_OUT = new System.Windows.Forms.TextBox();
            this.textBoxHops = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(705, 210);
            this.buttonSend.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(124, 47);
            this.buttonSend.TabIndex = 0;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // buttonListen
            // 
            this.buttonListen.Location = new System.Drawing.Point(218, 10);
            this.buttonListen.Margin = new System.Windows.Forms.Padding(2);
            this.buttonListen.Name = "buttonListen";
            this.buttonListen.Size = new System.Drawing.Size(127, 46);
            this.buttonListen.TabIndex = 1;
            this.buttonListen.Text = "Run";
            this.buttonListen.UseVisualStyleBackColor = true;
            this.buttonListen.Click += new System.EventHandler(this.buttonListen_Click_1);
            // 
            // listBoxReceived
            // 
            this.listBoxReceived.FormattingEnabled = true;
            this.listBoxReceived.Location = new System.Drawing.Point(-6, 273);
            this.listBoxReceived.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxReceived.Name = "listBoxReceived";
            this.listBoxReceived.Size = new System.Drawing.Size(849, 160);
            this.listBoxReceived.TabIndex = 2;
            // 
            // comboBoxRouters
            // 
            this.comboBoxRouters.FormattingEnabled = true;
            this.comboBoxRouters.Location = new System.Drawing.Point(9, 37);
            this.comboBoxRouters.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxRouters.Name = "comboBoxRouters";
            this.comboBoxRouters.Size = new System.Drawing.Size(110, 21);
            this.comboBoxRouters.TabIndex = 3;
            // 
            // comboBoxActions
            // 
            this.comboBoxActions.FormattingEnabled = true;
            this.comboBoxActions.Location = new System.Drawing.Point(721, 176);
            this.comboBoxActions.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxActions.Name = "comboBoxActions";
            this.comboBoxActions.Size = new System.Drawing.Size(77, 21);
            this.comboBoxActions.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Agent to communicate";
            // 
            // textBoxfrequency_IN
            // 
            this.textBoxfrequency_IN.Location = new System.Drawing.Point(13, 90);
            this.textBoxfrequency_IN.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxfrequency_IN.Name = "textBoxfrequency_IN";
            this.textBoxfrequency_IN.Size = new System.Drawing.Size(76, 20);
            this.textBoxfrequency_IN.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 75);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "frequency_in";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(108, 75);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Port_in";
            // 
            // textBoxPort_in
            // 
            this.textBoxPort_in.Location = new System.Drawing.Point(111, 90);
            this.textBoxPort_in.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPort_in.Name = "textBoxPort_in";
            this.textBoxPort_in.Size = new System.Drawing.Size(45, 20);
            this.textBoxPort_in.TabIndex = 12;
            // 
            // comboBoxTables
            // 
            this.comboBoxTables.FormattingEnabled = true;
            this.comboBoxTables.Location = new System.Drawing.Point(707, 117);
            this.comboBoxTables.Name = "comboBoxTables";
            this.comboBoxTables.Size = new System.Drawing.Size(121, 21);
            this.comboBoxTables.TabIndex = 13;
            this.comboBoxTables.SelectedIndexChanged += new System.EventHandler(this.comboBoxTables_SelectedIndexChanged);
            // 
            // labelTable
            // 
            this.labelTable.AutoSize = true;
            this.labelTable.Location = new System.Drawing.Point(725, 101);
            this.labelTable.Name = "labelTable";
            this.labelTable.Size = new System.Drawing.Size(73, 13);
            this.labelTable.TabIndex = 14;
            this.labelTable.Text = "Choose Table";
            // 
            // textBox_IP_IN
            // 
            this.textBox_IP_IN.Location = new System.Drawing.Point(9, 142);
            this.textBox_IP_IN.Name = "textBox_IP_IN";
            this.textBox_IP_IN.Size = new System.Drawing.Size(80, 20);
            this.textBox_IP_IN.TabIndex = 15;
            // 
            // textBox_Port_IN
            // 
            this.textBox_Port_IN.Location = new System.Drawing.Point(111, 142);
            this.textBox_Port_IN.Name = "textBox_Port_IN";
            this.textBox_Port_IN.Size = new System.Drawing.Size(61, 20);
            this.textBox_Port_IN.TabIndex = 16;
            // 
            // textBoxBand
            // 
            this.textBoxBand.Location = new System.Drawing.Point(189, 142);
            this.textBoxBand.Name = "textBoxBand";
            this.textBoxBand.Size = new System.Drawing.Size(65, 20);
            this.textBoxBand.TabIndex = 17;
            // 
            // textBoxFrequency
            // 
            this.textBoxFrequency.Location = new System.Drawing.Point(269, 142);
            this.textBoxFrequency.Name = "textBoxFrequency";
            this.textBoxFrequency.Size = new System.Drawing.Size(57, 20);
            this.textBoxFrequency.TabIndex = 18;
            // 
            // textBoxModulation
            // 
            this.textBoxModulation.Location = new System.Drawing.Point(344, 141);
            this.textBoxModulation.Name = "textBoxModulation";
            this.textBoxModulation.Size = new System.Drawing.Size(56, 20);
            this.textBoxModulation.TabIndex = 19;
            // 
            // textBoxBitrate
            // 
            this.textBoxBitrate.Location = new System.Drawing.Point(426, 142);
            this.textBoxBitrate.Name = "textBoxBitrate";
            this.textBoxBitrate.Size = new System.Drawing.Size(57, 20);
            this.textBoxBitrate.TabIndex = 20;
            // 
            // textBoxCloud_IP
            // 
            this.textBoxCloud_IP.Location = new System.Drawing.Point(496, 141);
            this.textBoxCloud_IP.Name = "textBoxCloud_IP";
            this.textBoxCloud_IP.Size = new System.Drawing.Size(47, 20);
            this.textBoxCloud_IP.TabIndex = 21;
            // 
            // textBoxPort_OUT
            // 
            this.textBoxPort_OUT.Location = new System.Drawing.Point(565, 141);
            this.textBoxPort_OUT.Name = "textBoxPort_OUT";
            this.textBoxPort_OUT.Size = new System.Drawing.Size(37, 20);
            this.textBoxPort_OUT.TabIndex = 22;
            // 
            // textBoxHops
            // 
            this.textBoxHops.Location = new System.Drawing.Point(619, 142);
            this.textBoxHops.Name = "textBoxHops";
            this.textBoxHops.Size = new System.Drawing.Size(41, 20);
            this.textBoxHops.TabIndex = 23;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "IP_IN";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(113, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Port_IN";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(201, 126);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "Band";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(280, 126);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "Frequency";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(355, 126);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Modulation";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(434, 126);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "BitRate";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(502, 125);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = "Cloud_IP";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(562, 126);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 13);
            this.label11.TabIndex = 31;
            this.label11.Text = "Port_OUT";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(619, 126);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 13);
            this.label12.TabIndex = 32;
            this.label12.Text = "Hops";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(723, 161);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(75, 13);
            this.label13.TabIndex = 33;
            this.label13.Text = "Choose action";
            // 
            // Application
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 433);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxHops);
            this.Controls.Add(this.textBoxPort_OUT);
            this.Controls.Add(this.textBoxCloud_IP);
            this.Controls.Add(this.textBoxBitrate);
            this.Controls.Add(this.textBoxModulation);
            this.Controls.Add(this.textBoxFrequency);
            this.Controls.Add(this.textBoxBand);
            this.Controls.Add(this.textBox_Port_IN);
            this.Controls.Add(this.textBox_IP_IN);
            this.Controls.Add(this.labelTable);
            this.Controls.Add(this.comboBoxTables);
            this.Controls.Add(this.textBoxPort_in);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxfrequency_IN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxActions);
            this.Controls.Add(this.comboBoxRouters);
            this.Controls.Add(this.listBoxReceived);
            this.Controls.Add(this.buttonListen);
            this.Controls.Add(this.buttonSend);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Application";
            this.Text = "Application";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonListen;
        private System.Windows.Forms.ListBox listBoxReceived;
        private System.Windows.Forms.ComboBox comboBoxRouters;
        private System.Windows.Forms.ComboBox comboBoxActions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxfrequency_IN;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPort_in;
        private System.Windows.Forms.ComboBox comboBoxTables;
        private System.Windows.Forms.Label labelTable;
        private System.Windows.Forms.TextBox textBox_IP_IN;
        private System.Windows.Forms.TextBox textBox_Port_IN;
        private System.Windows.Forms.TextBox textBoxBand;
        private System.Windows.Forms.TextBox textBoxFrequency;
        private System.Windows.Forms.TextBox textBoxModulation;
        private System.Windows.Forms.TextBox textBoxBitrate;
        private System.Windows.Forms.TextBox textBoxCloud_IP;
        private System.Windows.Forms.TextBox textBoxPort_OUT;
        private System.Windows.Forms.TextBox textBoxHops;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
    }
}

