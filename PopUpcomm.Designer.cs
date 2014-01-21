namespace TNCAX25Emulator
{
    partial class PopUpcomm
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
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.soundOutcomboBox = new System.Windows.Forms.ComboBox();
            this.APRSmsgIntcomboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.soundInComboBox = new System.Windows.Forms.ComboBox();
            this.payLoadCheckbox = new System.Windows.Forms.CheckBox();
            this.GPRScheckBox = new System.Windows.Forms.CheckBox();
            this.localGPRScombobox = new System.Windows.Forms.ComboBox();
            this.AudioTrackBox = new System.Windows.Forms.CheckBox();
            this.SoundEnabledInCB = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.serverportBox = new System.Windows.Forms.TextBox();
            this.audioAPRScheckBox = new System.Windows.Forms.CheckBox();
            this.KissCheckBox = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "TNC Comport";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(155, 263);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(258, 261);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(204, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Sound Card Out";
            // 
            // soundOutcomboBox
            // 
            this.soundOutcomboBox.FormattingEnabled = true;
            this.soundOutcomboBox.Location = new System.Drawing.Point(212, 55);
            this.soundOutcomboBox.Name = "soundOutcomboBox";
            this.soundOutcomboBox.Size = new System.Drawing.Size(121, 21);
            this.soundOutcomboBox.TabIndex = 7;
            this.soundOutcomboBox.SelectedIndexChanged += new System.EventHandler(this.soundOutcomboBox_SelectedIndexChanged);
            // 
            // APRSmsgIntcomboBox
            // 
            this.APRSmsgIntcomboBox.FormattingEnabled = true;
            this.APRSmsgIntcomboBox.Location = new System.Drawing.Point(212, 222);
            this.APRSmsgIntcomboBox.Name = "APRSmsgIntcomboBox";
            this.APRSmsgIntcomboBox.Size = new System.Drawing.Size(121, 21);
            this.APRSmsgIntcomboBox.TabIndex = 10;
            this.APRSmsgIntcomboBox.Visible = false;
            this.APRSmsgIntcomboBox.SelectedIndexChanged += new System.EventHandler(this.APRSmsgIntcomboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(213, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "APRS Message Interval";
            this.label4.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 222);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "SERVER:PORT";
            this.label2.Visible = false;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(378, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Sound Card Input";
            // 
            // soundInComboBox
            // 
            this.soundInComboBox.FormattingEnabled = true;
            this.soundInComboBox.Location = new System.Drawing.Point(381, 55);
            this.soundInComboBox.Name = "soundInComboBox";
            this.soundInComboBox.Size = new System.Drawing.Size(109, 21);
            this.soundInComboBox.TabIndex = 16;
            this.soundInComboBox.SelectedIndexChanged += new System.EventHandler(this.soundInComboBox_SelectedIndexChanged);
            // 
            // payLoadCheckbox
            // 
            this.payLoadCheckbox.AutoSize = true;
            this.payLoadCheckbox.Checked = global::TNCAX25Emulator.Properties.Settings.Default.PayLoadCallsign;
            this.payLoadCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TNCAX25Emulator.Properties.Settings.Default, "PayLoadCallsign", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.payLoadCheckbox.Location = new System.Drawing.Point(24, 179);
            this.payLoadCheckbox.Name = "payLoadCheckbox";
            this.payLoadCheckbox.Size = new System.Drawing.Size(127, 17);
            this.payLoadCheckbox.TabIndex = 21;
            this.payLoadCheckbox.Text = "Use Payload CallSign";
            this.payLoadCheckbox.UseVisualStyleBackColor = true;
            // 
            // GPRScheckBox
            // 
            this.GPRScheckBox.AutoSize = true;
            this.GPRScheckBox.Checked = global::TNCAX25Emulator.Properties.Settings.Default.localGPRSenabled;
            this.GPRScheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TNCAX25Emulator.Properties.Settings.Default, "localGPRSenabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.GPRScheckBox.Location = new System.Drawing.Point(24, 146);
            this.GPRScheckBox.Name = "GPRScheckBox";
            this.GPRScheckBox.Size = new System.Drawing.Size(121, 17);
            this.GPRScheckBox.TabIndex = 20;
            this.GPRScheckBox.Text = "Enable Local GPRS";
            this.GPRScheckBox.UseVisualStyleBackColor = true;
            // 
            // localGPRScombobox
            // 
            this.localGPRScombobox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "comport2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.localGPRScombobox.FormattingEnabled = true;
            this.localGPRScombobox.Location = new System.Drawing.Point(25, 119);
            this.localGPRScombobox.Name = "localGPRScombobox";
            this.localGPRScombobox.Size = new System.Drawing.Size(121, 21);
            this.localGPRScombobox.TabIndex = 19;
            this.localGPRScombobox.Text = global::TNCAX25Emulator.Properties.Settings.Default.comport2;
            // 
            // AudioTrackBox
            // 
            this.AudioTrackBox.AutoSize = true;
            this.AudioTrackBox.Checked = global::TNCAX25Emulator.Properties.Settings.Default.AudioTrack;
            this.AudioTrackBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TNCAX25Emulator.Properties.Settings.Default, "AudioTrack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.AudioTrackBox.Location = new System.Drawing.Point(212, 107);
            this.AudioTrackBox.Name = "AudioTrackBox";
            this.AudioTrackBox.Size = new System.Drawing.Size(84, 17);
            this.AudioTrackBox.TabIndex = 18;
            this.AudioTrackBox.Text = "Audio Track";
            this.AudioTrackBox.UseVisualStyleBackColor = true;
            // 
            // SoundEnabledInCB
            // 
            this.SoundEnabledInCB.AutoSize = true;
            this.SoundEnabledInCB.Checked = global::TNCAX25Emulator.Properties.Settings.Default.SoundEnabledIn;
            this.SoundEnabledInCB.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TNCAX25Emulator.Properties.Settings.Default, "SoundEnabledIn", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.SoundEnabledInCB.Location = new System.Drawing.Point(381, 107);
            this.SoundEnabledInCB.Name = "SoundEnabledInCB";
            this.SoundEnabledInCB.Size = new System.Drawing.Size(105, 17);
            this.SoundEnabledInCB.TabIndex = 17;
            this.SoundEnabledInCB.Text = "Enable Sound In";
            this.SoundEnabledInCB.UseVisualStyleBackColor = true;
            this.SoundEnabledInCB.Visible = false;
            this.SoundEnabledInCB.CheckedChanged += new System.EventHandler(this.SoundEnabledInCB_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.ButtonDropDownGrid;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = global::TNCAX25Emulator.Properties.Settings.Default.mapenabled;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TNCAX25Emulator.Properties.Settings.Default, "mapenabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(24, 267);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(82, 17);
            this.checkBox1.TabIndex = 14;
            this.checkBox1.Text = "Enable map";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // serverportBox
            // 
            this.serverportBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "serverport", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.serverportBox.Location = new System.Drawing.Point(24, 241);
            this.serverportBox.Name = "serverportBox";
            this.serverportBox.Size = new System.Drawing.Size(121, 20);
            this.serverportBox.TabIndex = 12;
            this.serverportBox.Text = global::TNCAX25Emulator.Properties.Settings.Default.serverport;
            this.serverportBox.Visible = false;
            this.serverportBox.TextChanged += new System.EventHandler(this.serverportBox_TextChanged);
            // 
            // audioAPRScheckBox
            // 
            this.audioAPRScheckBox.AutoSize = true;
            this.audioAPRScheckBox.Checked = global::TNCAX25Emulator.Properties.Settings.Default.audioAPRS;
            this.audioAPRScheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TNCAX25Emulator.Properties.Settings.Default, "audioAPRS", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.audioAPRScheckBox.Location = new System.Drawing.Point(212, 83);
            this.audioAPRScheckBox.Name = "audioAPRScheckBox";
            this.audioAPRScheckBox.Size = new System.Drawing.Size(105, 17);
            this.audioAPRScheckBox.TabIndex = 9;
            this.audioAPRScheckBox.Text = "APRS sound out";
            this.audioAPRScheckBox.UseVisualStyleBackColor = true;
            this.audioAPRScheckBox.CheckedChanged += new System.EventHandler(this.soundeEnablecheckBox_CheckedChanged);
            // 
            // KissCheckBox
            // 
            this.KissCheckBox.AutoSize = true;
            this.KissCheckBox.Checked = global::TNCAX25Emulator.Properties.Settings.Default.kissenabled;
            this.KissCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TNCAX25Emulator.Properties.Settings.Default, "kissenabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.KissCheckBox.Location = new System.Drawing.Point(24, 83);
            this.KissCheckBox.Name = "KissCheckBox";
            this.KissCheckBox.Size = new System.Drawing.Size(106, 17);
            this.KissCheckBox.TabIndex = 8;
            this.KissCheckBox.Text = "Enable Kiss TNC";
            this.KissCheckBox.UseVisualStyleBackColor = true;
            this.KissCheckBox.CheckedChanged += new System.EventHandler(this.KissCheckBox_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "comport", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(24, 55);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.Text = global::TNCAX25Emulator.Properties.Settings.Default.comport;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // PopUpcomm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 296);
            this.Controls.Add(this.payLoadCheckbox);
            this.Controls.Add(this.GPRScheckBox);
            this.Controls.Add(this.localGPRScombobox);
            this.Controls.Add(this.AudioTrackBox);
            this.Controls.Add(this.SoundEnabledInCB);
            this.Controls.Add(this.soundInComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverportBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.APRSmsgIntcomboBox);
            this.Controls.Add(this.audioAPRScheckBox);
            this.Controls.Add(this.KissCheckBox);
            this.Controls.Add(this.soundOutcomboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "PopUpcomm";
            this.Text = "Communication";
            this.Load += new System.EventHandler(this.PopUpcomm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox soundOutcomboBox;
        private System.Windows.Forms.CheckBox KissCheckBox;
        private System.Windows.Forms.CheckBox audioAPRScheckBox;
        private System.Windows.Forms.ComboBox APRSmsgIntcomboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox serverportBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox soundInComboBox;
        private System.Windows.Forms.CheckBox SoundEnabledInCB;
        private System.Windows.Forms.CheckBox AudioTrackBox;
        private System.Windows.Forms.ComboBox localGPRScombobox;
        private System.Windows.Forms.CheckBox GPRScheckBox;
        private System.Windows.Forms.CheckBox payLoadCheckbox;
    }
}