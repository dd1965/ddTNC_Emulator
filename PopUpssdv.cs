using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace TNCAX25Emulator
{
    public partial class PopUpssdv : Form
    {
        Boolean textboxUpdated = false;
        public PopUpssdv()
        {
            InitializeComponent();
            
        }
        public void diplayImage(Image img,int width,int height)
        {
            if (this.pictureBox1.InvokeRequired)
            {
                MethodInvoker del = delegate
                {
                    diplayImage(img, width, height);
                };
                this.Invoke(del);
                return;
            }                              
            
            try
            {
                pictureBox1.Image = img;
                pictureBox1.Width = width;
                pictureBox1.Height = height;
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid file"+"PopUpssdv line 26");
            }
        }
        public void displayHeaderText(String text)
        {
            if (this.textBox1.InvokeRequired)
            {
                MethodInvoker del = delegate
                {
                    displayHeaderText(text);
                };
                this.Invoke(del);
                return;
            }

               
            textBox1.Text = text;
            textBox1.ForeColor = Color.Green;          
            textBox1.BackColor = Color.Black;
            textboxUpdated = true;

        }

        private void PopUpssdv_Load(object sender, EventArgs e)
        {
            if (Usersetting.highSpeed == 0)
            {
                radioButton1.Checked = true;
            }
            else if (Usersetting.highSpeed == 1)
            {
                radioButton2.Checked = true;
            }
            else if (Usersetting.highSpeed == 2)
            {
                radioButton3.Checked = true;
            }
            else if (Usersetting.highSpeed == 3)
            {
                radioButton4.Checked = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        

        private void PopUpssdv_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
          
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Usersetting.highSpeed = 0;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Usersetting.highSpeed = 1;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Usersetting.highSpeed = 2;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            Usersetting.highSpeed = 3;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (textboxUpdated)
            {
                textboxUpdated = false;
                textBox1.ForeColor = Color.Black;
                textBox1.BackColor = Color.White;
            }
        }
    }
}
