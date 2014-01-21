namespace TNCAX25Emulator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.speed = new System.Windows.Forms.TextBox();
            this.longitude = new System.Windows.Forms.TextBox();
            this.latitude = new System.Windows.Forms.TextBox();
            this.time = new System.Windows.Forms.TextBox();
            this.sequenceno = new System.Windows.Forms.TextBox();
            this.altitude = new System.Windows.Forms.TextBox();
            this.tempin = new System.Windows.Forms.TextBox();
            this.tempout = new System.Windows.Forms.TextBox();
            this.voltage = new System.Windows.Forms.TextBox();
            this.numofsat = new System.Windows.Forms.TextBox();
            this.gpsfix = new System.Windows.Forms.TextBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ccomportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.callsignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rttyConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sSDVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sSDVEnableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transmitPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.debug = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.spectrumBox1 = new System.Windows.Forms.PictureBox();
            this.ballooncallsign = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.waterfallBox = new System.Windows.Forms.PictureBox();
            this.NRcheckBox1 = new System.Windows.Forms.CheckBox();
            this.LogginChkbox = new System.Windows.Forms.CheckBox();
            this.WebcheckBox1 = new System.Windows.Forms.CheckBox();
            this.AudioTrackcheckBox1 = new System.Windows.Forms.CheckBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.squelchBoxLed = new System.Windows.Forms.PictureBox();
            this.statsrichTextBox2 = new System.Windows.Forms.RichTextBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.ALT_richTextBox = new System.Windows.Forms.RichTextBox();
            this.softDCDBox = new System.Windows.Forms.CheckBox();
            this.rangeBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.elevationtextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bearingtextBox = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.checkBoxFastTelemetry = new System.Windows.Forms.CheckBox();
            this.radioButton1200 = new System.Windows.Forms.RadioButton();
            this.radioButton4800 = new System.Windows.Forms.RadioButton();
            this.checkBoxReverse = new System.Windows.Forms.CheckBox();
            this.radioButton9600 = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spectrumBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.waterfallBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.squelchBoxLed)).BeginInit();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 1200;
            this.serialPort1.WriteTimeout = 2000;
            this.serialPort1.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(this.serialPort1_ErrorReceived);
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.MenuText;
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.Info;
            this.richTextBox1.Location = new System.Drawing.Point(334, 27);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(412, 175);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(357, 297);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Info text";
            this.label5.Visible = false;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(360, 312);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(121, 20);
            this.textBox3.TabIndex = 9;
            this.textBox3.Visible = false;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 218);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Sequence No";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(101, 218);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Time";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(187, 218);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Latitude";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(270, 218);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Longitude";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(270, 257);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "GPS Fix";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(357, 218);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(78, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "Altitude (Meter)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(101, 257);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "Temp In";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(441, 218);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(38, 13);
            this.label13.TabIndex = 17;
            this.label13.Text = "Speed";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(187, 257);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(60, 13);
            this.label14.TabIndex = 18;
            this.label14.Text = "Num of Sat";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 257);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(54, 13);
            this.label15.TabIndex = 19;
            this.label15.Text = "Temp Out";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(357, 257);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(43, 13);
            this.label16.TabIndex = 20;
            this.label16.Text = "Voltage";
            // 
            // speed
            // 
            this.speed.Location = new System.Drawing.Point(444, 234);
            this.speed.Name = "speed";
            this.speed.ReadOnly = true;
            this.speed.Size = new System.Drawing.Size(70, 20);
            this.speed.TabIndex = 21;
            // 
            // longitude
            // 
            this.longitude.Location = new System.Drawing.Point(273, 234);
            this.longitude.Name = "longitude";
            this.longitude.ReadOnly = true;
            this.longitude.Size = new System.Drawing.Size(70, 20);
            this.longitude.TabIndex = 22;
            // 
            // latitude
            // 
            this.latitude.Location = new System.Drawing.Point(190, 234);
            this.latitude.Name = "latitude";
            this.latitude.ReadOnly = true;
            this.latitude.Size = new System.Drawing.Size(70, 20);
            this.latitude.TabIndex = 23;
            // 
            // time
            // 
            this.time.Location = new System.Drawing.Point(104, 234);
            this.time.Name = "time";
            this.time.ReadOnly = true;
            this.time.Size = new System.Drawing.Size(70, 20);
            this.time.TabIndex = 24;
            // 
            // sequenceno
            // 
            this.sequenceno.Location = new System.Drawing.Point(15, 234);
            this.sequenceno.Name = "sequenceno";
            this.sequenceno.ReadOnly = true;
            this.sequenceno.Size = new System.Drawing.Size(70, 20);
            this.sequenceno.TabIndex = 25;
            // 
            // altitude
            // 
            this.altitude.Location = new System.Drawing.Point(360, 234);
            this.altitude.Name = "altitude";
            this.altitude.ReadOnly = true;
            this.altitude.Size = new System.Drawing.Size(70, 20);
            this.altitude.TabIndex = 26;
            // 
            // tempin
            // 
            this.tempin.Location = new System.Drawing.Point(104, 273);
            this.tempin.Name = "tempin";
            this.tempin.ReadOnly = true;
            this.tempin.Size = new System.Drawing.Size(70, 20);
            this.tempin.TabIndex = 27;
            this.tempin.TextChanged += new System.EventHandler(this.tempin_TextChanged);
            // 
            // tempout
            // 
            this.tempout.Location = new System.Drawing.Point(12, 273);
            this.tempout.Name = "tempout";
            this.tempout.ReadOnly = true;
            this.tempout.Size = new System.Drawing.Size(70, 20);
            this.tempout.TabIndex = 28;
            // 
            // voltage
            // 
            this.voltage.Location = new System.Drawing.Point(360, 273);
            this.voltage.Name = "voltage";
            this.voltage.ReadOnly = true;
            this.voltage.Size = new System.Drawing.Size(70, 20);
            this.voltage.TabIndex = 29;
            // 
            // numofsat
            // 
            this.numofsat.Location = new System.Drawing.Point(190, 273);
            this.numofsat.Name = "numofsat";
            this.numofsat.ReadOnly = true;
            this.numofsat.Size = new System.Drawing.Size(70, 20);
            this.numofsat.TabIndex = 30;
            // 
            // gpsfix
            // 
            this.gpsfix.Location = new System.Drawing.Point(273, 273);
            this.gpsfix.Name = "gpsfix";
            this.gpsfix.ReadOnly = true;
            this.gpsfix.Size = new System.Drawing.Size(70, 20);
            this.gpsfix.TabIndex = 31;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(779, 27);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(356, 503);
            this.webBrowser1.TabIndex = 33;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem,
            this.sSDVToolStripMenuItem,
            this.testToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1035, 24);
            this.menuStrip1.TabIndex = 34;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked_1);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ccomportToolStripMenuItem,
            this.callsignToolStripMenuItem,
            this.rttyConfigToolStripMenuItem,
            this.loggingToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.testToolStripMenuItem.Text = "Setup";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // ccomportToolStripMenuItem
            // 
            this.ccomportToolStripMenuItem.Name = "ccomportToolStripMenuItem";
            this.ccomportToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.ccomportToolStripMenuItem.Text = "Communication";
            this.ccomportToolStripMenuItem.Click += new System.EventHandler(this.ccomportToolStripMenuItem_Click);
            // 
            // callsignToolStripMenuItem
            // 
            this.callsignToolStripMenuItem.Name = "callsignToolStripMenuItem";
            this.callsignToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.callsignToolStripMenuItem.Text = "Callsign";
            this.callsignToolStripMenuItem.Click += new System.EventHandler(this.callsignToolStripMenuItem_Click);
            // 
            // rttyConfigToolStripMenuItem
            // 
            this.rttyConfigToolStripMenuItem.Name = "rttyConfigToolStripMenuItem";
            this.rttyConfigToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.rttyConfigToolStripMenuItem.Text = "Rtty Config";
            this.rttyConfigToolStripMenuItem.Click += new System.EventHandler(this.rttyConfigToolStripMenuItem_Click);
            // 
            // loggingToolStripMenuItem
            // 
            this.loggingToolStripMenuItem.Name = "loggingToolStripMenuItem";
            this.loggingToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.loggingToolStripMenuItem.Text = "Logging";
            this.loggingToolStripMenuItem.Click += new System.EventHandler(this.loggingToolStripMenuItem_Click);
            // 
            // sSDVToolStripMenuItem
            // 
            this.sSDVToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sSDVEnableToolStripMenuItem,
            this.transmitPictureToolStripMenuItem});
            this.sSDVToolStripMenuItem.Name = "sSDVToolStripMenuItem";
            this.sSDVToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.sSDVToolStripMenuItem.Text = "Picture";
            this.sSDVToolStripMenuItem.Click += new System.EventHandler(this.sSDVToolStripMenuItem_Click);
            // 
            // sSDVEnableToolStripMenuItem
            // 
            this.sSDVEnableToolStripMenuItem.Name = "sSDVEnableToolStripMenuItem";
            this.sSDVEnableToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.sSDVEnableToolStripMenuItem.Text = "SSDV enable";
            this.sSDVEnableToolStripMenuItem.Click += new System.EventHandler(this.sSDVEnableToolStripMenuItem_Click);
            // 
            // transmitPictureToolStripMenuItem
            // 
            this.transmitPictureToolStripMenuItem.Name = "transmitPictureToolStripMenuItem";
            this.transmitPictureToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.transmitPictureToolStripMenuItem.Text = "Transmit Picture";
            this.transmitPictureToolStripMenuItem.Click += new System.EventHandler(this.transmitPictureToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem1
            // 
            this.testToolStripMenuItem1.Name = "testToolStripMenuItem1";
            this.testToolStripMenuItem1.Size = new System.Drawing.Size(41, 20);
            this.testToolStripMenuItem1.Text = "Test";
            this.testToolStripMenuItem1.Click += new System.EventHandler(this.testToolStripMenuItem1_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // debug
            // 
            this.debug.Location = new System.Drawing.Point(448, 273);
            this.debug.Name = "debug";
            this.debug.ReadOnly = true;
            this.debug.Size = new System.Drawing.Size(66, 20);
            this.debug.TabIndex = 36;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(437, 257);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Debug";
            // 
            // spectrumBox1
            // 
            this.spectrumBox1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.spectrumBox1.Location = new System.Drawing.Point(9, 27);
            this.spectrumBox1.Name = "spectrumBox1";
            this.spectrumBox1.Size = new System.Drawing.Size(286, 94);
            this.spectrumBox1.TabIndex = 39;
            this.spectrumBox1.TabStop = false;
            this.spectrumBox1.Click += new System.EventHandler(this.spectrumBox1_Click);
            this.spectrumBox1.MouseLeave += new System.EventHandler(this.spectrumBox1_MouseLeave);
            this.spectrumBox1.MouseHover += new System.EventHandler(this.spectrumBox1_MouseHover);
            this.spectrumBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spectrumBox1_MouseMove);
            // 
            // ballooncallsign
            // 
            this.ballooncallsign.Location = new System.Drawing.Point(16, 312);
            this.ballooncallsign.Name = "ballooncallsign";
            this.ballooncallsign.ReadOnly = true;
            this.ballooncallsign.Size = new System.Drawing.Size(67, 20);
            this.ballooncallsign.TabIndex = 41;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 297);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 42;
            this.label3.Text = "Balloon Id";
            // 
            // waterfallBox
            // 
            this.waterfallBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.waterfallBox.Location = new System.Drawing.Point(9, 125);
            this.waterfallBox.Name = "waterfallBox";
            this.waterfallBox.Size = new System.Drawing.Size(286, 75);
            this.waterfallBox.TabIndex = 43;
            this.waterfallBox.TabStop = false;
            this.waterfallBox.Click += new System.EventHandler(this.waterfallBox_Click);
            // 
            // NRcheckBox1
            // 
            this.NRcheckBox1.AutoSize = true;
            this.NRcheckBox1.Enabled = false;
            this.NRcheckBox1.Location = new System.Drawing.Point(535, 231);
            this.NRcheckBox1.Name = "NRcheckBox1";
            this.NRcheckBox1.Size = new System.Drawing.Size(42, 17);
            this.NRcheckBox1.TabIndex = 44;
            this.NRcheckBox1.Text = "NR";
            this.NRcheckBox1.UseVisualStyleBackColor = true;
            this.NRcheckBox1.Visible = false;
            this.NRcheckBox1.CheckedChanged += new System.EventHandler(this.NRcheckBox1_CheckedChanged);
            // 
            // LogginChkbox
            // 
            this.LogginChkbox.AutoSize = true;
            this.LogginChkbox.Location = new System.Drawing.Point(612, 208);
            this.LogginChkbox.Name = "LogginChkbox";
            this.LogginChkbox.Size = new System.Drawing.Size(64, 17);
            this.LogginChkbox.TabIndex = 46;
            this.LogginChkbox.Text = "Logging";
            this.LogginChkbox.UseVisualStyleBackColor = true;
            this.LogginChkbox.CheckedChanged += new System.EventHandler(this.LogginChkbox_CheckedChanged);
            // 
            // WebcheckBox1
            // 
            this.WebcheckBox1.AutoSize = true;
            this.WebcheckBox1.Location = new System.Drawing.Point(612, 231);
            this.WebcheckBox1.Name = "WebcheckBox1";
            this.WebcheckBox1.Size = new System.Drawing.Size(90, 17);
            this.WebcheckBox1.TabIndex = 47;
            this.WebcheckBox1.Text = "Web Logging";
            this.WebcheckBox1.UseVisualStyleBackColor = true;
            this.WebcheckBox1.CheckedChanged += new System.EventHandler(this.WebcheckBox1_CheckedChanged);
            // 
            // AudioTrackcheckBox1
            // 
            this.AudioTrackcheckBox1.AutoSize = true;
            this.AudioTrackcheckBox1.Location = new System.Drawing.Point(611, 253);
            this.AudioTrackcheckBox1.Name = "AudioTrackcheckBox1";
            this.AudioTrackcheckBox1.Size = new System.Drawing.Size(84, 17);
            this.AudioTrackcheckBox1.TabIndex = 48;
            this.AudioTrackcheckBox1.Text = "Audio Track";
            this.AudioTrackcheckBox1.UseVisualStyleBackColor = true;
            this.AudioTrackcheckBox1.CheckedChanged += new System.EventHandler(this.AudioTrackcheckBox1_CheckedChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.AccessibleName = "";
            this.trackBar1.BackColor = System.Drawing.SystemColors.Control;
            this.trackBar1.Location = new System.Drawing.Point(716, 222);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(0);
            this.trackBar1.Maximum = 40;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 96);
            this.trackBar1.TabIndex = 49;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // squelchBoxLed
            // 
            this.squelchBoxLed.Location = new System.Drawing.Point(723, 208);
            this.squelchBoxLed.Name = "squelchBoxLed";
            this.squelchBoxLed.Size = new System.Drawing.Size(23, 20);
            this.squelchBoxLed.TabIndex = 51;
            this.squelchBoxLed.TabStop = false;
            // 
            // statsrichTextBox2
            // 
            this.statsrichTextBox2.BackColor = System.Drawing.SystemColors.Menu;
            this.statsrichTextBox2.Location = new System.Drawing.Point(9, 497);
            this.statsrichTextBox2.Name = "statsrichTextBox2";
            this.statsrichTextBox2.ReadOnly = true;
            this.statsrichTextBox2.Size = new System.Drawing.Size(737, 20);
            this.statsrichTextBox2.TabIndex = 52;
            this.statsrichTextBox2.Text = "";
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // ALT_richTextBox
            // 
            this.ALT_richTextBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.ALT_richTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 60F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ALT_richTextBox.ForeColor = System.Drawing.Color.Lime;
            this.ALT_richTextBox.Location = new System.Drawing.Point(9, 368);
            this.ALT_richTextBox.Name = "ALT_richTextBox";
            this.ALT_richTextBox.ReadOnly = true;
            this.ALT_richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.ALT_richTextBox.Size = new System.Drawing.Size(736, 106);
            this.ALT_richTextBox.TabIndex = 53;
            this.ALT_richTextBox.Text = "";
            // 
            // softDCDBox
            // 
            this.softDCDBox.AutoSize = true;
            this.softDCDBox.Location = new System.Drawing.Point(535, 208);
            this.softDCDBox.Name = "softDCDBox";
            this.softDCDBox.Size = new System.Drawing.Size(71, 17);
            this.softDCDBox.TabIndex = 54;
            this.softDCDBox.Text = "Soft DCD";
            this.softDCDBox.UseVisualStyleBackColor = true;
            this.softDCDBox.CheckedChanged += new System.EventHandler(this.softDCDBox_CheckedChanged);
            // 
            // rangeBox
            // 
            this.rangeBox.Location = new System.Drawing.Point(104, 312);
            this.rangeBox.Name = "rangeBox";
            this.rangeBox.ReadOnly = true;
            this.rangeBox.Size = new System.Drawing.Size(70, 20);
            this.rangeBox.TabIndex = 56;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 296);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 57;
            this.label1.Text = "Range (KM)";
            // 
            // elevationtextBox
            // 
            this.elevationtextBox.Location = new System.Drawing.Point(190, 312);
            this.elevationtextBox.Name = "elevationtextBox";
            this.elevationtextBox.ReadOnly = true;
            this.elevationtextBox.Size = new System.Drawing.Size(70, 20);
            this.elevationtextBox.TabIndex = 58;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(187, 296);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 59;
            this.label4.Text = "Elevation";
            // 
            // bearingtextBox
            // 
            this.bearingtextBox.Location = new System.Drawing.Point(272, 312);
            this.bearingtextBox.Name = "bearingtextBox";
            this.bearingtextBox.ReadOnly = true;
            this.bearingtextBox.Size = new System.Drawing.Size(71, 20);
            this.bearingtextBox.TabIndex = 60;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(272, 296);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(43, 13);
            this.label17.TabIndex = 61;
            this.label17.Text = "Bearing";
            // 
            // serialPort2
            // 
            this.serialPort2.BaudRate = 4800;
            this.serialPort2.PortName = global::TNCAX25Emulator.Properties.Settings.Default.comport2;
            this.serialPort2.WriteTimeout = 2000;
            this.serialPort2.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort2_DataReceived);
            // 
            // checkBoxFastTelemetry
            // 
            this.checkBoxFastTelemetry.AutoSize = true;
            this.checkBoxFastTelemetry.Location = new System.Drawing.Point(612, 276);
            this.checkBoxFastTelemetry.Name = "checkBoxFastTelemetry";
            this.checkBoxFastTelemetry.Size = new System.Drawing.Size(95, 17);
            this.checkBoxFastTelemetry.TabIndex = 62;
            this.checkBoxFastTelemetry.Text = "Fast Telemetry";
            this.checkBoxFastTelemetry.UseVisualStyleBackColor = true;
            this.checkBoxFastTelemetry.CheckedChanged += new System.EventHandler(this.checkBoxFastTelemetry_CheckedChanged);
            // 
            // radioButton1200
            // 
            this.radioButton1200.AutoSize = true;
            this.radioButton1200.Enabled = false;
            this.radioButton1200.Location = new System.Drawing.Point(612, 301);
            this.radioButton1200.Name = "radioButton1200";
            this.radioButton1200.Size = new System.Drawing.Size(49, 17);
            this.radioButton1200.TabIndex = 63;
            this.radioButton1200.TabStop = true;
            this.radioButton1200.Text = "1200";
            this.radioButton1200.UseVisualStyleBackColor = true;
            this.radioButton1200.CheckedChanged += new System.EventHandler(this.radioButton1200_CheckedChanged);
            // 
            // radioButton4800
            // 
            this.radioButton4800.AutoSize = true;
            this.radioButton4800.Enabled = false;
            this.radioButton4800.Location = new System.Drawing.Point(612, 320);
            this.radioButton4800.Name = "radioButton4800";
            this.radioButton4800.Size = new System.Drawing.Size(49, 17);
            this.radioButton4800.TabIndex = 64;
            this.radioButton4800.TabStop = true;
            this.radioButton4800.Text = "4800";
            this.radioButton4800.UseVisualStyleBackColor = true;
            this.radioButton4800.CheckedChanged += new System.EventHandler(this.radioButton4800_CheckedChanged);
            // 
            // checkBoxReverse
            // 
            this.checkBoxReverse.AutoSize = true;
            this.checkBoxReverse.Enabled = false;
            this.checkBoxReverse.Location = new System.Drawing.Point(667, 321);
            this.checkBoxReverse.Name = "checkBoxReverse";
            this.checkBoxReverse.Size = new System.Drawing.Size(46, 17);
            this.checkBoxReverse.TabIndex = 65;
            this.checkBoxReverse.Text = "Rev";
            this.checkBoxReverse.UseVisualStyleBackColor = true;
            this.checkBoxReverse.CheckedChanged += new System.EventHandler(this.checkBoxReverse_CheckedChanged);
            // 
            // radioButton9600
            // 
            this.radioButton9600.AutoSize = true;
            this.radioButton9600.Enabled = false;
            this.radioButton9600.Location = new System.Drawing.Point(612, 338);
            this.radioButton9600.Name = "radioButton9600";
            this.radioButton9600.Size = new System.Drawing.Size(49, 17);
            this.radioButton9600.TabIndex = 66;
            this.radioButton9600.TabStop = true;
            this.radioButton9600.Text = "9600";
            this.radioButton9600.UseVisualStyleBackColor = true;
            this.radioButton9600.CheckedChanged += new System.EventHandler(this.radioButton9600_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1035, 529);
            this.Controls.Add(this.radioButton9600);
            this.Controls.Add(this.checkBoxReverse);
            this.Controls.Add(this.radioButton4800);
            this.Controls.Add(this.radioButton1200);
            this.Controls.Add(this.checkBoxFastTelemetry);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.bearingtextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.elevationtextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rangeBox);
            this.Controls.Add(this.softDCDBox);
            this.Controls.Add(this.ALT_richTextBox);
            this.Controls.Add(this.statsrichTextBox2);
            this.Controls.Add(this.squelchBoxLed);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.AudioTrackcheckBox1);
            this.Controls.Add(this.WebcheckBox1);
            this.Controls.Add(this.LogginChkbox);
            this.Controls.Add(this.NRcheckBox1);
            this.Controls.Add(this.waterfallBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ballooncallsign);
            this.Controls.Add(this.spectrumBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.debug);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.gpsfix);
            this.Controls.Add(this.numofsat);
            this.Controls.Add(this.voltage);
            this.Controls.Add(this.tempout);
            this.Controls.Add(this.tempin);
            this.Controls.Add(this.altitude);
            this.Controls.Add(this.sequenceno);
            this.Controls.Add(this.time);
            this.Controls.Add(this.latitude);
            this.Controls.Add(this.longitude);
            this.Controls.Add(this.speed);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "VK3TBC TNC Emualtor V3.13 15/01/2013";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spectrumBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.waterfallBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.squelchBoxLed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox speed;
        private System.Windows.Forms.TextBox longitude;
        private System.Windows.Forms.TextBox latitude;
        private System.Windows.Forms.TextBox time;
        private System.Windows.Forms.TextBox sequenceno;
        private System.Windows.Forms.TextBox altitude;
        private System.Windows.Forms.TextBox tempin;
        private System.Windows.Forms.TextBox tempout;
        private System.Windows.Forms.TextBox voltage;
        private System.Windows.Forms.TextBox numofsat;
        private System.Windows.Forms.TextBox gpsfix;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ccomportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem callsignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox debug;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox spectrumBox1;
        private System.Windows.Forms.ToolStripMenuItem rttyConfigToolStripMenuItem;
        private System.Windows.Forms.TextBox ballooncallsign;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem loggingToolStripMenuItem;
        private System.Windows.Forms.PictureBox waterfallBox;
        private System.Windows.Forms.CheckBox NRcheckBox1;
        private System.Windows.Forms.CheckBox LogginChkbox;
        private System.Windows.Forms.CheckBox WebcheckBox1;
        private System.Windows.Forms.CheckBox AudioTrackcheckBox1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.PictureBox squelchBoxLed;
        private System.Windows.Forms.RichTextBox statsrichTextBox2;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.RichTextBox ALT_richTextBox;
        private System.Windows.Forms.CheckBox softDCDBox;
        private System.Windows.Forms.TextBox rangeBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox elevationtextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox bearingtextBox;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ToolStripMenuItem sSDVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transmitPictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sSDVEnableToolStripMenuItem;
        private System.IO.Ports.SerialPort serialPort2;
        private System.Windows.Forms.CheckBox checkBoxFastTelemetry;
        private System.Windows.Forms.RadioButton radioButton1200;
        private System.Windows.Forms.RadioButton radioButton4800;
        private System.Windows.Forms.CheckBox checkBoxReverse;
        private System.Windows.Forms.RadioButton radioButton9600;
    }
}

