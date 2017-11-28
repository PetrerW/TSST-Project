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
            this.comboBoxNode1IP = new System.Windows.Forms.ComboBox();
            this.comboBoxNode2IP = new System.Windows.Forms.ComboBox();
            this.comboBoxActions = new System.Windows.Forms.ComboBox();
            this.comboBoxLambdas = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxfrequency = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPort_out = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(113, 219);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(165, 58);
            this.buttonSend.TabIndex = 0;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // buttonListen
            // 
            this.buttonListen.Location = new System.Drawing.Point(290, 12);
            this.buttonListen.Name = "buttonListen";
            this.buttonListen.Size = new System.Drawing.Size(169, 57);
            this.buttonListen.TabIndex = 1;
            this.buttonListen.Text = "Run";
            this.buttonListen.UseVisualStyleBackColor = true;
            this.buttonListen.Click += new System.EventHandler(this.buttonListen_Click_1);
            // 
            // listBoxReceived
            // 
            this.listBoxReceived.FormattingEnabled = true;
            this.listBoxReceived.ItemHeight = 16;
            this.listBoxReceived.Location = new System.Drawing.Point(-7, 283);
            this.listBoxReceived.Name = "listBoxReceived";
            this.listBoxReceived.Size = new System.Drawing.Size(476, 164);
            this.listBoxReceived.TabIndex = 2;
            // 
            // comboBoxRouters
            // 
            this.comboBoxRouters.FormattingEnabled = true;
            this.comboBoxRouters.Location = new System.Drawing.Point(12, 45);
            this.comboBoxRouters.Name = "comboBoxRouters";
            this.comboBoxRouters.Size = new System.Drawing.Size(145, 24);
            this.comboBoxRouters.TabIndex = 3;
            // 
            // comboBoxNode1IP
            // 
            this.comboBoxNode1IP.FormattingEnabled = true;
            this.comboBoxNode1IP.Location = new System.Drawing.Point(12, 101);
            this.comboBoxNode1IP.Name = "comboBoxNode1IP";
            this.comboBoxNode1IP.Size = new System.Drawing.Size(121, 24);
            this.comboBoxNode1IP.TabIndex = 4;
            // 
            // comboBoxNode2IP
            // 
            this.comboBoxNode2IP.FormattingEnabled = true;
            this.comboBoxNode2IP.Location = new System.Drawing.Point(210, 173);
            this.comboBoxNode2IP.Name = "comboBoxNode2IP";
            this.comboBoxNode2IP.Size = new System.Drawing.Size(114, 24);
            this.comboBoxNode2IP.TabIndex = 5;
            // 
            // comboBoxActions
            // 
            this.comboBoxActions.FormattingEnabled = true;
            this.comboBoxActions.Location = new System.Drawing.Point(358, 172);
            this.comboBoxActions.Name = "comboBoxActions";
            this.comboBoxActions.Size = new System.Drawing.Size(101, 24);
            this.comboBoxActions.TabIndex = 6;
            // 
            // comboBoxLambdas
            // 
            this.comboBoxLambdas.FormattingEnabled = true;
            this.comboBoxLambdas.Location = new System.Drawing.Point(181, 101);
            this.comboBoxLambdas.Name = "comboBoxLambdas";
            this.comboBoxLambdas.Size = new System.Drawing.Size(73, 24);
            this.comboBoxLambdas.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Agent to communicate";
            // 
            // textBoxfrequency
            // 
            this.textBoxfrequency.Location = new System.Drawing.Point(12, 174);
            this.textBoxfrequency.Name = "textBoxfrequency";
            this.textBoxfrequency.Size = new System.Drawing.Size(100, 22);
            this.textBoxfrequency.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "frequency_in";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(133, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Port_out";
            // 
            // textBoxPort_out
            // 
            this.textBoxPort_out.Location = new System.Drawing.Point(136, 175);
            this.textBoxPort_out.Name = "textBoxPort_out";
            this.textBoxPort_out.Size = new System.Drawing.Size(59, 22);
            this.textBoxPort_out.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(241, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "IP_out";
            // 
            // Application
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 447);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxPort_out);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxfrequency);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxLambdas);
            this.Controls.Add(this.comboBoxActions);
            this.Controls.Add(this.comboBoxNode2IP);
            this.Controls.Add(this.comboBoxNode1IP);
            this.Controls.Add(this.comboBoxRouters);
            this.Controls.Add(this.listBoxReceived);
            this.Controls.Add(this.buttonListen);
            this.Controls.Add(this.buttonSend);
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
        private System.Windows.Forms.ComboBox comboBoxNode1IP;
        private System.Windows.Forms.ComboBox comboBoxNode2IP;
        private System.Windows.Forms.ComboBox comboBoxActions;
        private System.Windows.Forms.ComboBox comboBoxLambdas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxfrequency;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPort_out;
        private System.Windows.Forms.Label label4;
    }
}

