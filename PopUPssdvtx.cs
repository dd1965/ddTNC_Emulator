using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

namespace TNCAX25Emulator
{
   partial class PopUPssdvtx : Form
    {
        Image txImage;
        String filenameSelected;
        String filenameout;
        String filenametx;
        Bitmap myBitmap;
        SSDV ssdv;
        public PopUPssdvtx(SSDV ssdv)
        {
            InitializeComponent();
            transmit.Enabled = false;
            this.ssdv = ssdv;
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
            this.ShowDialog();
          
            
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
      
        private void transmit_Click(object sender, EventArgs e)
        {
             String day = DateTime.Now.ToString("dMMyyyy");
             filenametx = Logging.getMydirectory()+ "\\SSDV_PIC_ID_TX" + Usersetting.ImgId + "_" + day + ".out";
             byte[] fileout = Encoding.ASCII.GetBytes(filenametx);//This will be the out file to be loaded into the sender
             byte[] filein = Encoding.ASCII.GetBytes(filenameout);//Input JPEG file

          
            String callsigns;
            byte SSID;
            if (Usersetting.callsign.Contains('-'))
             {
                 string[] cs = Usersetting.callsign.Split('-');
                 callsigns = cs[0];
                 string ssid = cs[1];
                 SSID = Convert.ToByte(ssid);
             }
             else
            {
                SSID = 0;
                callsigns = Usersetting.callsign;
             }
            
             byte[] tcallsign = new byte[7];
             byte[] callsign = new byte[6];
             callsign = Encoding.ASCII.GetBytes(callsigns);
             tcallsign[6] = SSID;
             for (int i = 0; i < callsign.Length; i++)
             {
                 tcallsign[i] = callsign[i];                
             }

             int result=SSDV.ssdvTrial.encodeImage((byte)Usersetting.ImgId,tcallsign, filein, fileout);
            try
            {
               // File.Delete(filenameout);
            }
            catch (Exception er)
            {

            }
           
            
            Usersetting.ImgId++;
            transmit.Enabled = false; 
              
             Thread t = new Thread( NewThread );
             t.IsBackground=true;
              t.Start();

        }
        void NewThread()
        {
            
            ssdv.transmitSSDV(filenametx);
        }
        private void loadpicturebutton_Click(object sender, EventArgs e)
        {
            try
            {
                myBitmap = new Bitmap(320,
                240,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                Graphics drawingGraphics = Graphics.FromImage(myBitmap);
                Font myFont = new Font("Arial", 12, FontStyle.Bold);
                Color color = Color.Black;
                color = Color.FromArgb(127, 0, 0, 0);
                
                SolidBrush myBrush = new SolidBrush(color);


                String freetext = "Transmission de " + Usersetting.callsign;
                drawingGraphics.DrawString(freetext, myFont, Brushes.Aqua, new Point(1, 220));

                OpenFileDialog open = new OpenFileDialog();

                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";

                if (open.ShowDialog() == DialogResult.OK)
                {

                    Image myImage = new Bitmap(open.FileName);
                    Image process = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Size sz = new Size();
                    sz.Height = 240;
                    sz.Width = 320;
                    process = resizeImage(myImage, sz, myBitmap);

                 /*   if (freetext != "")
                    {
                        drawingGraphics.FillRectangle(myBrush, 1, 240, 256, 256);
                        drawingGraphics.DrawString(freetext, myFont, Brushes.Yellow, new Point(1, 240));
                    }*/
                    filenameSelected = open.FileName;
                    txImage = new Bitmap(process);
                   // oldimage = (Image)SSTVtx.Image.Clone();
                    pictureBox1.Image = txImage;
                    //SSTVtx.Image = new Bitmap(imagePlot.resizeImage(myImage, sz, myBitmap));
                    transmit.Enabled = true;
                    //pictureBox2.Image= new Bitmap((Image)(myBitmap));
                }


                imageSave();
                open.Dispose();
                myBitmap.Dispose();
                drawingGraphics.Dispose();


            }

            catch (Exception)
            {

                //throw new ApplicationException("Failed loading image");
                //Cannot load image.

            }

        }
        public static Image resizeImage(Image imgToResize, Size size, Bitmap myBitmap)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            //Bitmap b = new Bitmap(destWidth, destHeight);
            //Graphics g = Graphics.FromImage((Image)b);
            Graphics g = Graphics.FromImage((Image)myBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);//was 16


            g.Dispose();

            return (Image)myBitmap;
        }
        public void imageSave()
        {
            using (Bitmap blankImage = new Bitmap(myBitmap.Width, myBitmap.Height, PixelFormat.Format24bppRgb))
            {

                using (Graphics g1 = Graphics.FromImage(blankImage))
                {
                    g1.DrawImageUnscaledAndClipped(myBitmap, new Rectangle(Point.Empty, myBitmap.Size));
                }
                String day = DateTime.Now.ToString("dMMyyyy");
                ImageCodecInfo bmpCodec = GetImageDecoder(ImageFormat.Jpeg);
                filenameout = Logging.getMydirectory();
                filenameout = filenameout + "\\SSDV_PIC_ID_TX" + Usersetting.ImgId + "_" + day + ".jpeg";
                blankImage.Save(filenameout, bmpCodec, null);
                

            }
        }

        public static ImageCodecInfo GetImageEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageEncoders().ToList().Find(delegate(ImageCodecInfo codec)
            {
                return codec.FormatID == format.Guid;
            });
        }

        public static ImageCodecInfo GetImageDecoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().ToList().Find(delegate(ImageCodecInfo codec)
            {
                return codec.FormatID == format.Guid;
            });
        }

        ImageCodecInfo gifEncoder = GetImageEncoder(ImageFormat.Gif);
        ImageCodecInfo bmpDecoder = GetImageDecoder(ImageFormat.Bmp);
        ImageCodecInfo jpegDecoder = GetImageDecoder(ImageFormat.Jpeg);
        ImageCodecInfo jpegEncoder = GetImageEncoder(ImageFormat.Jpeg);

        private void button1_Click(object sender, EventArgs e)
        {
            ssdv.abort();
        }

        private void PopUPssdvtx_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                File.Delete(filenameout);
            }
            catch (Exception err)
            {
            }
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
    }
}
