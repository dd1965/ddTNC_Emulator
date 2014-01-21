namespace TNCAX25Emulator
{
    partial class PopUpcallsign
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Longitude = new System.Windows.Forms.Label();
            this.longitudetextBox1 = new System.Windows.Forms.TextBox();
            this.latitudeTextBox1 = new System.Windows.Forms.TextBox();
            this.path = new System.Windows.Forms.TextBox();
            this.callsign = new System.Windows.Forms.TextBox();
            this.heighttextBox1 = new System.Windows.Forms.TextBox();
            this.Height = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Callsign";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Path";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(153, 195);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 195);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(150, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Latitude";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // Longitude
            // 
            this.Longitude.AutoSize = true;
            this.Longitude.Location = new System.Drawing.Point(150, 68);
            this.Longitude.Name = "Longitude";
            this.Longitude.Size = new System.Drawing.Size(54, 13);
            this.Longitude.TabIndex = 9;
            this.Longitude.Text = "Longitude";
            // 
            // longitudetextBox1
            // 
            this.longitudetextBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "longitude", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.longitudetextBox1.Location = new System.Drawing.Point(153, 84);
            this.longitudetextBox1.Name = "longitudetextBox1";
            this.longitudetextBox1.Size = new System.Drawing.Size(100, 20);
            this.longitudetextBox1.TabIndex = 8;
            this.longitudetextBox1.Text = global::TNCAX25Emulator.Properties.Settings.Default.longitude;
            // 
            // latitudeTextBox1
            // 
            this.latitudeTextBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "latitude", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.latitudeTextBox1.Location = new System.Drawing.Point(153, 29);
            this.latitudeTextBox1.Name = "latitudeTextBox1";
            this.latitudeTextBox1.Size = new System.Drawing.Size(100, 20);
            this.latitudeTextBox1.TabIndex = 7;
            this.latitudeTextBox1.Text = global::TNCAX25Emulator.Properties.Settings.Default.latitude;
            // 
            // path
            // 
            this.path.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "path", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.path.Location = new System.Drawing.Point(19, 84);
            this.path.Name = "path";
            this.path.Size = new System.Drawing.Size(100, 20);
            this.path.TabIndex = 1;
            this.path.Text = global::TNCAX25Emulator.Properties.Settings.Default.path;
            this.path.TextChanged += new System.EventHandler(this.path_TextChanged);
            // 
            // callsign
            // 
            this.callsign.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "callsign", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.callsign.Location = new System.Drawing.Point(19, 29);
            this.callsign.Name = "callsign";
            this.callsign.Size = new System.Drawing.Size(100, 20);
            this.callsign.TabIndex = 0;
            this.callsign.Text = global::TNCAX25Emulator.Properties.Settings.Default.callsign;
            this.callsign.TextChanged += new System.EventHandler(this.callsign_TextChanged);
            // 
            // heighttextBox1
            // 
            this.heighttextBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TNCAX25Emulator.Properties.Settings.Default, "Height", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.heighttextBox1.Location = new System.Drawing.Point(153, 142);
            this.heighttextBox1.Name = "heighttextBox1";
            this.heighttextBox1.Size = new System.Drawing.Size(100, 20);
            this.heighttextBox1.TabIndex = 10;
            this.heighttextBox1.Text = global::TNCAX25Emulator.Properties.Settings.Default.Height;
            // 
            // Height
            // 
            this.Height.AutoSize = true;
            this.Height.Location = new System.Drawing.Point(150, 126);
            this.Height.Name = "Height";
            this.Height.Size = new System.Drawing.Size(38, 13);
            this.Height.TabIndex = 11;
            this.Height.Text = "Height";
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // PopUpcallsign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 230);
            this.Controls.Add(this.Height);
            this.Controls.Add(this.heighttextBox1);
            this.Controls.Add(this.Longitude);
            this.Controls.Add(this.longitudetextBox1);
            this.Controls.Add(this.latitudeTextBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.path);
            this.Controls.Add(this.callsign);
            this.Name = "PopUpcallsign";
            this.Text = "Enter station details";
            this.Load += new System.EventHandler(this.PopUpcallsign_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox callsign;
        private System.Windows.Forms.TextBox path;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox latitudeTextBox1;
        private System.Windows.Forms.TextBox longitudetextBox1;
        private System.Windows.Forms.Label Longitude;
        private System.Windows.Forms.TextBox heighttextBox1;
        private System.Windows.Forms.Label Height;
        private System.Windows.Forms.ToolTip toolTip1;

    }
}