namespace LoopController
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.autotunebutton = new System.Windows.Forms.Button();
            this.SWRdisplayrichTextBox = new System.Windows.Forms.RichTextBox();
            this.stepbutton = new System.Windows.Forms.Button();
            this.continuoscheckBox = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.VSWR = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.antfreqdisplay = new System.Windows.Forms.RichTextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.TargetFrequency = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CurrentPosition = new System.Windows.Forms.ProgressBar();
            this.FPower = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.motorBox = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.stepcountBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.motorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // autotunebutton
            // 
            this.autotunebutton.Location = new System.Drawing.Point(185, 139);
            this.autotunebutton.Margin = new System.Windows.Forms.Padding(4);
            this.autotunebutton.Name = "autotunebutton";
            this.autotunebutton.Size = new System.Drawing.Size(112, 28);
            this.autotunebutton.TabIndex = 0;
            this.autotunebutton.Text = "Auto tune";
            this.autotunebutton.UseVisualStyleBackColor = true;
            this.autotunebutton.Click += new System.EventHandler(this.autotunebutton_Click);
            // 
            // SWRdisplayrichTextBox
            // 
            this.SWRdisplayrichTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.SWRdisplayrichTextBox.Location = new System.Drawing.Point(325, 142);
            this.SWRdisplayrichTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.SWRdisplayrichTextBox.Name = "SWRdisplayrichTextBox";
            this.SWRdisplayrichTextBox.ReadOnly = true;
            this.SWRdisplayrichTextBox.Size = new System.Drawing.Size(111, 26);
            this.SWRdisplayrichTextBox.TabIndex = 1;
            this.SWRdisplayrichTextBox.Text = "";
            // 
            // stepbutton
            // 
            this.stepbutton.Location = new System.Drawing.Point(13, 291);
            this.stepbutton.Margin = new System.Windows.Forms.Padding(4);
            this.stepbutton.Name = "stepbutton";
            this.stepbutton.Size = new System.Drawing.Size(112, 28);
            this.stepbutton.TabIndex = 4;
            this.stepbutton.Text = "Step";
            this.stepbutton.UseVisualStyleBackColor = true;
            this.stepbutton.Click += new System.EventHandler(this.stepbutton_Click);
            // 
            // continuoscheckBox
            // 
            this.continuoscheckBox.AutoSize = true;
            this.continuoscheckBox.Location = new System.Drawing.Point(133, 299);
            this.continuoscheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.continuoscheckBox.Name = "continuoscheckBox";
            this.continuoscheckBox.Size = new System.Drawing.Size(95, 20);
            this.continuoscheckBox.TabIndex = 5;
            this.continuoscheckBox.Text = "Continous";
            this.continuoscheckBox.UseVisualStyleBackColor = true;
            this.continuoscheckBox.CheckedChanged += new System.EventHandler(this.continuoscheckBox_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 57600;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // VSWR
            // 
            this.VSWR.AutoSize = true;
            this.VSWR.Location = new System.Drawing.Point(322, 121);
            this.VSWR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.VSWR.Name = "VSWR";
            this.VSWR.Size = new System.Drawing.Size(53, 16);
            this.VSWR.TabIndex = 6;
            this.VSWR.Text = "VSWR";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 70);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 29);
            this.button1.TabIndex = 7;
            this.button1.Text = "Calibrate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // antfreqdisplay
            // 
            this.antfreqdisplay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.antfreqdisplay.Location = new System.Drawing.Point(184, 29);
            this.antfreqdisplay.Name = "antfreqdisplay";
            this.antfreqdisplay.Size = new System.Drawing.Size(191, 28);
            this.antfreqdisplay.TabIndex = 8;
            this.antfreqdisplay.Text = "";
            this.antfreqdisplay.TextChanged += new System.EventHandler(this.antfreqdisplay_TextChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(18, 254);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(46, 20);
            this.radioButton1.TabIndex = 9;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Up";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(77, 254);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(64, 20);
            this.radioButton2.TabIndex = 10;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Down";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 235);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "Antenna Frequency";
            // 
            // TargetFrequency
            // 
            this.TargetFrequency.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.TargetFrequency.Location = new System.Drawing.Point(402, 29);
            this.TargetFrequency.Name = "TargetFrequency";
            this.TargetFrequency.Size = new System.Drawing.Size(191, 28);
            this.TargetFrequency.TabIndex = 12;
            this.TargetFrequency.Text = "";
            this.TargetFrequency.TextChanged += new System.EventHandler(this.TargetFrequency_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "Ant Frequency";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(399, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "SDR Frequency";
            // 
            // CurrentPosition
            // 
            this.CurrentPosition.Location = new System.Drawing.Point(184, 70);
            this.CurrentPosition.Name = "CurrentPosition";
            this.CurrentPosition.Size = new System.Drawing.Size(191, 29);
            this.CurrentPosition.TabIndex = 15;
            this.CurrentPosition.Click += new System.EventHandler(this.CurrentPosition_Click);
            // 
            // FPower
            // 
            this.FPower.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.FPower.Location = new System.Drawing.Point(466, 142);
            this.FPower.Margin = new System.Windows.Forms.Padding(4);
            this.FPower.Name = "FPower";
            this.FPower.ReadOnly = true;
            this.FPower.Size = new System.Drawing.Size(111, 26);
            this.FPower.TabIndex = 17;
            this.FPower.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(464, 121);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 18;
            this.label4.Text = "Power (Watts)";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(11, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(29, 26);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.Location = new System.Drawing.Point(641, 28);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(29, 26);
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.White;
            this.pictureBox3.Location = new System.Drawing.Point(702, 28);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(29, 26);
            this.pictureBox3.TabIndex = 21;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(638, 9);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 16);
            this.label5.TabIndex = 22;
            this.label5.Text = "Tx";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(699, 10);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 16);
            this.label6.TabIndex = 23;
            this.label6.Text = "Rx";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 10);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 16);
            this.label7.TabIndex = 24;
            this.label7.Text = "USB Connected";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(13, 21);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(72, 17);
            this.radioButton3.TabIndex = 25;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Automatic";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(13, 47);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(60, 17);
            this.radioButton4.TabIndex = 26;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "Manual";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton4);
            this.groupBox1.Location = new System.Drawing.Point(18, 139);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 81);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Control";
            // 
            // motorBox
            // 
            this.motorBox.BackColor = System.Drawing.Color.White;
            this.motorBox.Location = new System.Drawing.Point(641, 76);
            this.motorBox.Name = "motorBox";
            this.motorBox.Size = new System.Drawing.Size(29, 26);
            this.motorBox.TabIndex = 28;
            this.motorBox.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(638, 57);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 16);
            this.label8.TabIndex = 29;
            this.label8.Text = "Motor";
            // 
            // stepcountBox
            // 
            this.stepcountBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.stepcountBox.Location = new System.Drawing.Point(402, 70);
            this.stepcountBox.Name = "stepcountBox";
            this.stepcountBox.Size = new System.Drawing.Size(191, 28);
            this.stepcountBox.TabIndex = 30;
            this.stepcountBox.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(752, 332);
            this.Controls.Add(this.stepcountBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.motorBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FPower);
            this.Controls.Add(this.CurrentPosition);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TargetFrequency);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.antfreqdisplay);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.VSWR);
            this.Controls.Add(this.continuoscheckBox);
            this.Controls.Add(this.stepbutton);
            this.Controls.Add(this.SWRdisplayrichTextBox);
            this.Controls.Add(this.autotunebutton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Motor Controller Magnetic Loop";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.motorBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button autotunebutton;
        private System.Windows.Forms.RichTextBox SWRdisplayrichTextBox;
        private System.Windows.Forms.Button stepbutton;
        private System.Windows.Forms.CheckBox continuoscheckBox;
        private System.Windows.Forms.Timer timer1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Label VSWR;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox antfreqdisplay;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox TargetFrequency;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar CurrentPosition;
        private System.Windows.Forms.RichTextBox FPower;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox motorBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RichTextBox stepcountBox;
    }
}

