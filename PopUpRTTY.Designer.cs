namespace TNCAX25Emulator
{
    partial class PopUpRTTY
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
            this.baudBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.MarkBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.reverseBox = new System.Windows.Forms.CheckBox();
            this.offsetBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // baudBox
            // 
            this.baudBox.Location = new System.Drawing.Point(12, 28);
            this.baudBox.Name = "baudBox";
            this.baudBox.Size = new System.Drawing.Size(85, 20);
            this.baudBox.TabIndex = 0;
            this.baudBox.TextChanged += new System.EventHandler(this.baudBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Baud (45 - 300)";
            // 
            // MarkBox
            // 
            this.MarkBox.Location = new System.Drawing.Point(145, 28);
            this.MarkBox.Name = "MarkBox";
            this.MarkBox.Size = new System.Drawing.Size(82, 20);
            this.MarkBox.TabIndex = 2;
            this.MarkBox.TextChanged += new System.EventHandler(this.MarkBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(142, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Mark Freq (Hz)";
            // 
            // reverseBox
            // 
            this.reverseBox.AutoSize = true;
            this.reverseBox.Location = new System.Drawing.Point(145, 76);
            this.reverseBox.Name = "reverseBox";
            this.reverseBox.Size = new System.Drawing.Size(66, 17);
            this.reverseBox.TabIndex = 4;
            this.reverseBox.Text = "Reverse";
            this.reverseBox.UseVisualStyleBackColor = true;
            // 
            // offsetBox
            // 
            this.offsetBox.Location = new System.Drawing.Point(12, 76);
            this.offsetBox.Name = "offsetBox";
            this.offsetBox.Size = new System.Drawing.Size(85, 20);
            this.offsetBox.TabIndex = 5;
            this.offsetBox.TextChanged += new System.EventHandler(this.offsetBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Offset (Hz 100 - 500)";
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(12, 115);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 7;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.button1_Click);
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(136, 115);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 8;
            this.ok.Text = "Ok";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.button2_Click);
            // 
            // PopUpRTTY
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 152);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.offsetBox);
            this.Controls.Add(this.reverseBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MarkBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.baudBox);
            this.Name = "PopUpRTTY";
            this.Text = "RTTY Config";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox baudBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MarkBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox reverseBox;
        private System.Windows.Forms.TextBox offsetBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button ok;
    }
}