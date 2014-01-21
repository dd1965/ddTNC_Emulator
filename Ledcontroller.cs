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
    class Ledcontroller
    {

        private Boolean flash_led=false;
        private PictureBox pbled;
        private String imagefileon, imagefileoff;
        private System.IO.Stream fileimgon;
        private System.IO.Stream fileimgoff;
        private System.IO.Stream imgtodisplay;

        public Ledcontroller(PictureBox pb,String imagefileon,String imagefileoff)
            
        {
            this.pbled = pb;
            this.imagefileon = imagefileon;
            this.imagefileoff = imagefileoff;
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            fileimgon = thisExe.GetManifestResourceStream(imagefileon);
            fileimgoff = thisExe.GetManifestResourceStream(imagefileoff);
            this.pbled.Image = Image.FromStream(fileimgoff);
           // Thread thread = new Thread(new ThreadStart(LedThread));
           // thread.IsBackground = true;
           // thread.Start();
            
        }
        public delegate void Invokedelegate();
        private void LedThread()
        {
            while (true)
            {
                if (flash_led)
                {
                    flash_led = false;
                    imgtodisplay = fileimgon;
                    pbled.BeginInvoke(new Invokedelegate(updateLed));
                    Thread.Sleep(200);          
                    imgtodisplay = fileimgoff;
                    pbled.BeginInvoke(new Invokedelegate(updateLed));
                   
                }
                Thread.Sleep(20);
            }
        }
       

        private void updateLed()
        {
            this.pbled.Image = Image.FromStream(imgtodisplay);
            this.pbled.Refresh();
        }
        public void updateLedon()
        {
            this.pbled.Image = Image.FromStream(fileimgon);
            this.pbled.Refresh();
        }
        public void updateLedoff()
        {
            this.pbled.Image = Image.FromStream(fileimgoff);
            this.pbled.Refresh();
        }
        public void flashled(){
            flash_led = true;

        }
    }
}
