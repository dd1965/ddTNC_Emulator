namespace TNCAX25Emulator
{
    partial class PopUprtty1
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
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.afc = new System.Windows.Forms.CheckBox();
            this.reverseBox = new System.Windows.Forms.CheckBox();
            this.offsetBox = new System.Windows.Forms.TextBox();
            this.MarkBox = new System.Windows.Forms.TextBox();
            this.baudBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(36, 121);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(201, 121);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 1;
            this.ok.Text = "Ok";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "Baud\r\n(45 - 300)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 26);
            this.label2.TabIndex = 6;
            this.label2.Text = "MARK Hz \r\n(1000 - 2500)";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(218, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 26);
            this.label3.TabIndex = 7;
            this.label3.Text = "Offset Hz\r\n(100 - 500)";
            // 
            // afc
            // 
            this.afc.AutoSize = true;
            this.afc.Checked = global::TNCAX25Emulator.Properties.Settings.Default.afcenabled;
            this.afc.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TNCAX25Emulator.Properties.Settings.Default, "afcenabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.afc.Location = new System.Drawing.Point(120, 80);
            this.afc.Name = "afc";
            this.afc.Size = new System.Drawing.Size(46, 17);
            this.afc.TabIndex = 10;
            this.afc.Text = "AFC";
            this.afc.UseVisualStyleBackColor = true;
            // 
            // reverseBox
            // 
            this.reverseBox.AutoSize = true;
            this.reverseBox.Checked = global::TNCAX25Emulator.Properties.Settings.Default.reverseenabled;
            this.reverseBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TNCAX25Emulator.Properties.Settings.Default, "reverseenabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.reverseBox.Location = new System.Drawing.Point(15, 80);
            this.reverseBox.Name = "reverseBox";
            this.reverseBox.Size = new System.Drawing.Size(66, 17);
            this.reverseBox.TabIndex = 8;
            this.reverseBox.Text = "Reverse";
            this.reverseBox.UseVisualStyleBackColor = true;
            this.reverseBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // offsetBox
            // 
            this.offsetBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "offset", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.offsetBox.Location = new System.Drawing.Point(221, 45);
            this.offsetBox.Name = "offsetBox";
            this.offsetBox.Size = new System.Drawing.Size(75, 20);
            this.offsetBox.TabIndex = 4;
            this.offsetBox.Text = global::TNCAX25Emulator.Properties.Settings.Default.offset;
            this.offsetBox.TextChanged += new System.EventHandler(this.offsetBox_TextChanged_1);
            // 
            // MarkBox
            // 
            this.MarkBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "markfreq", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MarkBox.Location = new System.Drawing.Point(120, 45);
            this.MarkBox.Name = "MarkBox";
            this.MarkBox.Size = new System.Drawing.Size(75, 20);
            this.MarkBox.TabIndex = 3;
            this.MarkBox.Text = global::TNCAX25Emulator.Properties.Settings.Default.markfreq;
            this.MarkBox.TextChanged += new System.EventHandler(this.MarkBox_TextChanged_1);
            // 
            // baudBox
            // 
            this.baudBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "baud", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.baudBox.Location = new System.Drawing.Point(12, 45);
            this.baudBox.Name = "baudBox";
            this.baudBox.Size = new System.Drawing.Size(80, 20);
            this.baudBox.TabIndex = 2;
            this.baudBox.Text = global::TNCAX25Emulator.Properties.Settings.Default.baud;
            this.baudBox.TextChanged += new System.EventHandler(this.baudBox_TextChanged_1);
            // 
            // PopUprtty1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 171);
            this.Controls.Add(this.afc);
            this.Controls.Add(this.reverseBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.offsetBox);
            this.Controls.Add(this.MarkBox);
            this.Controls.Add(this.baudBox);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.cancel);
            this.Name = "PopUprtty1";
            this.Text = "RTTY config";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.TextBox baudBox;
        private System.Windows.Forms.TextBox MarkBox;
        private System.Windows.Forms.TextBox offsetBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox reverseBox;
        private System.Windows.Forms.CheckBox afc;
    }
}