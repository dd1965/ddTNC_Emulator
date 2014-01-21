
//-----------------------------------------------------------------------
// <copyright file="graph" company="(none)">
//  Copyright (c) 2013 VK3TBC
//
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
//
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
//  BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
//  ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE
// </copyright>
// <author>VK3TBC</author>
// Has a spectrum graph and a waterfall graph in the same class. TODO add a time graph


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;

namespace TNCAX25Emulator
{
    class graph
    {
        protected PictureBox pb;
        private  System.Windows.Forms.Timer timer;
        Point[] points;
      
        int y,y1,oldy;
        int max = 0xFFFFFFF;
        int xAxisPos; // x axis at bottom of plot
        int yScale;
        int xScale;
        float binSize;
        int binlow;
        int binhigh;
        int xstep;
        int mousePTR = 0;
        int mousePTRclick = 0;
        int bwoffset = 0;
        
        //Spectrum
        int cnt = 0;
        int yw;
        Boolean fillscreen = true;
        Rectangle oneline;
        Rectangle destoneline;
        Rectangle srcRec;
        Rectangle destRec;
        Bitmap tempbitmap;
        Bitmap scrollscreen;
        
        
        private  ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        Pen greenPen = new Pen(Color.SpringGreen, 4);
        SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
        SolidBrush myRedBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
        SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
        SolidBrush myWhiteBrush = new SolidBrush(Color.White);
        StringFormat drawFormat = new StringFormat();
        StringFormat drawFormat1 = new StringFormat();
        Font myFont = new Font("Arial", 10);
        Boolean state = true;
        BufferedGraphicsContext currentContext;
        protected BufferedGraphics myBuffer;
        int rmspower=0;
        int peakholdcnt = 0;

        public graph(PictureBox pb)
        {
            
            this.pb = pb;
            xScale = (int)(((Config.FFTRES / 2) + 1 )/ pb.Width);
             binSize = (float) Config.samplingrate / (float) Config.FFTRES;
          //  binlow = (int)( 1000 / binSize);
            binlow = 0;
            binhigh = (int)(3000 / binSize);
            xstep =  pb.Width/(binhigh - binlow);
            yScale = (int)max / pb.Height;
            xAxisPos = pb.Height-5;//-5
            rmspower = pb.Height;
            points = new Point[pb.Width-30];//Todo make this dependant on calculating the audio buffer.
            
            //Spectrum
            oneline = new Rectangle(0, 0, pb.Width, 1);
            destoneline = new Rectangle(0, 0, pb.Width, 1);
            srcRec = new Rectangle(0, 0, pb.Width, pb.Height - 1);
            destRec = new Rectangle(0, 1, pb.Width, pb.Height - 1);
            scrollscreen = new Bitmap(pb.Width, pb.Height);
            tempbitmap = new Bitmap(pb.Width, pb.Height);

            
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 5;//5
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

        
            currentContext = BufferedGraphicsManager.Current;
            drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            drawFormat1.FormatFlags = StringFormatFlags.NoWrap;
            myBuffer = currentContext.Allocate(pb.CreateGraphics(),
                new Rectangle(0, 0, Math.Max(pb.Width, 1), Math.Max(pb.Height, 1)));
            myBuffer = currentContext.Allocate(pb.CreateGraphics(),
               pb.DisplayRectangle);

                                 
        }
        public void Halt()
        {
            timer.Stop();
            state = false;
            
        }
        public void start()
        {
            state = true;
            timer.Start();
        }

       public void setscreenPTR(int mousePTR){
           if (mousePTR != -1)
           {
               int freqofmouseptr = (int)((mousePTR * 23.47 / 2) + 23.47); //+1000
               if ((freqofmouseptr <= (0 + Usersetting.offset-48))||( (freqofmouseptr >= (3000))))return ;//3652
           }
           else
           {
               mousePTR = 0;
           }
           this.mousePTR = mousePTR;
           
       }

       public Boolean setscreenPTRconform(int mousePTR)
       {
           int freqofmouseptr = (int)((mousePTR * 23.47 / 2) + 23.47); //1000
           if ((freqofmouseptr <= (0 + Usersetting.offset-48)) || ((freqofmouseptr >= (3000)))) return false;
           this.mousePTRclick = mousePTR;
           return true;
          
       }
       public void setScreenFreq(int freq)
       {
           int factor = (int)((float)((freq / (23.47 / 2))) - binlow*2); //-86
           this.mousePTRclick = factor;
       }
       public void filterBW(int bwoffset)
       {
           this.bwoffset = bwoffset* 2;//each bin takes 2 pixel position on the screen.
           //this.bwoffset = (int)((float)bwoffset*((float)(Config.FFTRES/2)/((float)pb.Width)));
       }
       public void clear(string toDisplay)
       {
           _lock.EnterReadLock();
           myBuffer.Graphics.Clear(Color.Black);
           _lock.ExitReadLock();
       }
       public void writeValue(float value)
       {
           _lock.EnterReadLock();         
            myBuffer.Graphics.DrawString(value.ToString(), myFont, yellowBrush, new Point(30, 30), drawFormat1);
           _lock.ExitReadLock();
       }

       public void storePointstoPlot(double[] values, double power, float filtercentre,int squelch)
       {
           try
           {
              // return;
               if (double.IsNaN(power)) return;
               if (state == true)
               {
               _lock.EnterReadLock();
              
                   myBuffer.Graphics.Clear(Color.Black);
                   int x = 0;
                   for (int i = 0; i < pb.Width - 30; )
                   {
                       y = xAxisPos - (int)(values[binlow + x] / yScale);
                       y1 = (int)(oldy + y) / 2;
                       if (y >= 160) y = 160;
                       if (y1 >= 160) y1 = 160;
                       oldy = y1;
                       Point point1 = new Point(i, y);
                       if (i >= points.Length) break;//Temporary fix to stop a crash with big font size.
                       points[i++] = point1;

                       if (i >= points.Length) break;//Temporary fix to stop a crash with big font size.
                       point1 = new Point(i, y1);
                       points[i++] = point1;


                       /*  point1 = new Point(i, y);
                         points[i++] = point1;

                         point1 = new Point(i, y);
                         points[i++] = point1;*/
                       x++;
                   }
            
                 //  int powscale = (int)((float)(((power / 9000000))) * 80);
                 //  powscale = 160 - powscale;
                   int powscale = pb.Height - (int)power*2;
                   //   rmspower = 0;
                   if (powscale <= 0) powscale = 0;
                   /*
                   {
                       peakholdcnt++;
                       if (rmspower >= powscale)
                       {
                           rmspower = powscale;
                           peakholdcnt = 0;
                       }
                       else
                       {
                           if (peakholdcnt > 25)
                           {
                               rmspower = powscale;
                               peakholdcnt = 0;
                           }
                       }
                   }*/
                   rmspower = powscale;
                    int factor;
                   if (Usersetting.highSpeed == 0)
                   {
                       bwoffset = (int)((float)Usersetting.offset / ((float)Config.samplingrate / (float)Config.FFTRES)) * 2;
                       factor = (int)((float)((filtercentre) / (23.47 / 2))) - binlow * 2;//86
                   
                      
                       myBuffer.Graphics.DrawLine(Pens.Yellow, mousePTR, 0,
                                  mousePTR, pb.Height - 5);
                       myBuffer.Graphics.DrawLine(Pens.Yellow, mousePTR - bwoffset, 0,
                                  mousePTR - bwoffset, pb.Height - 5);

                      
                   }
                   else
                   {
                       filtercentre = 1700;
                       factor = (int)((float)((filtercentre) / (23.47 / 2))) - binlow * 2;
                       bwoffset = (int)((float)1000 / ((float)Config.samplingrate / (float)Config.FFTRES)) *2;
                     
                   }
                   myBuffer.Graphics.DrawString(filtercentre.ToString() + "Hz", myFont, yellowBrush, new Point(factor, 10), drawFormat);
                   myBuffer.Graphics.DrawLine(Pens.Cyan, factor, 0,
                                   factor, pb.Height - 5);//998.
                   myBuffer.Graphics.DrawLine(Pens.Red, factor + bwoffset / 2, 0,
                                 factor + bwoffset / 2, pb.Height - 5);
                   myBuffer.Graphics.DrawLine(Pens.Red, factor - bwoffset / 2, 0,
                             factor - bwoffset / 2, pb.Height - 5);
                   //myBuffer.Graphics.FillEllipse(myBrush, 180, 6, 5, 5);
                   myBuffer.Graphics.FillRectangle(myRedBrush, new Rectangle(pb.Width - 15, rmspower, 10, pb.Width - 25));//-20 -35
                   myBuffer.Graphics.FillRectangle(myWhiteBrush, new Rectangle(pb.Width - 15, (int)(pb.Height-squelch*1.5-10), 10, 5));
                   myBuffer.Graphics.DrawLines(greenPen, points);
                 // myBuffer.Graphics.DrawImageUnscaled(display_graphics_spectrum(values), 0, 0);
               
                   _lock.ExitReadLock();
               }
           }
           catch (Exception e)
           {

               MessageBox.Show("Graph "+e.ToString(), "TNCAX25Emulator",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
           }
       
       }
       private Bitmap display_graphics_spectrum(double[] intensity)
       {
           Bitmap b = new Bitmap(256, 94);
           BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
           ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
           int stride = bmData.Stride;
           System.IntPtr Scan0 = bmData.Scan0;
           Color myColor = Color.LightGreen;
        

           unsafe
           {
               byte* p = (byte*)(void*)Scan0;
               int dataValue;
               int nOffset =  (stride - 256 * 3);
               byte* scan0 = (byte*)bmData.Scan0.ToPointer();
               int x = 0;
               for (int i = 0; i < pb.Width; i+=2)
               {
                  // dataValue = (int)(intensity[binlow + i] / yScale);
                   dataValue = xAxisPos - (int)(intensity[binlow + x] / yScale);
                  
                   if (dataValue >= 89) dataValue = 89;
                   if (dataValue < 0) dataValue = 0;
                   for (int j = dataValue; j <pb.Height; j++)
                   {
                       
                       byte* data = scan0 + j * stride + i * 24 / 8;
                       data[2] = (byte)(myColor.R); //Red
                       data[1] = (byte)(myColor.G);//Green
                       data[0] = (byte)(myColor.B);//Blue
                       data += 3;
                       data[2] = (byte)(myColor.R); //Red
                       data[1] = (byte)(myColor.G);//Green
                       data[0] = (byte)(myColor.B);//Blue
                    
                       //data is a pointer to the first byte of the 3-byte color data
                   }
                   x++;
               }
               
               
               
            /*   for (int x = 0; x < 128; x++)
               {
                   test = stride*hgt+x;
                   p = (byte*)test;
                   p[2] = (byte)(myColor.R); //Red
                   p[1] = (byte)(myColor.G);//Green
                   p[0] = (byte)(myColor.B);//Blue
                   p += 3;
                   p[2] = (byte)(myColor.R); //Red
                   p[1] = (byte)(myColor.G);//Green
                   p[0] = (byte)(myColor.B);//Blue
                   p += 3;

                    valueIntensity = intensity[x] / (yScale * 100);
                 
                    p[2] = (byte)(255 * GetRedValue( valueIntensity)); //Red
                    p[1] = (byte)(255 * GetGreenValue(valueIntensity));//Green
                    p[0] = (byte)(255 * GetBlueValue(valueIntensity));//Blue
                   
                    p += 3;
               

               }*/

           }

           b.UnlockBits(bmData);

           return b;
       }

       public void updateWaterFall(double[] values, double power, float filtercentre)
       {
           _lock.EnterReadLock();
           


           cnt++;
           if (cnt != 1)
           {
               _lock.ExitReadLock();
               return;
           }
          
           cnt = 0;
           System.Drawing.Bitmap bm;
           Size sz = new Size(pb.Width, pb.Height);
           Rectangle destt = new Rectangle(0, 0, pb.Width, pb.Height);
           bm = display_graphics(values);
           CopyRegionIntoImage(scrollscreen, srcRec, tempbitmap, destRec);
           CopyRegionIntoImage(bm, oneline, tempbitmap, oneline);
           myBuffer.Graphics.DrawImageUnscaled(tempbitmap, 0, 0);
           scrollscreen = tempbitmap;

           
           _lock.ExitReadLock();

       }
       private void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, Bitmap destBitmap, Rectangle destRegion)
       {
           using (Graphics grD = Graphics.FromImage(destBitmap))
           {
               grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            
           }
       }


       private Bitmap display_graphics(double[] intensity)
       {
           Bitmap b = new Bitmap(256, 1);
           BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
           ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
           int stride = bmData.Stride;
           System.IntPtr Scan0 = bmData.Scan0;
           Color myColor;
           unsafe
           {
               byte* p = (byte*)(void*)Scan0;

               int nOffset = stride - 256 * 3;
               for (int x = 0; x < 128;x++)
               {
                   myColor = getColor2(intensity[x]);
                   p[2] = (byte)(myColor.R); //Red
                   p[1] = (byte)(myColor.G);//Green
                   p[0] = (byte)(myColor.B);//Blue
                   p += 3;
                    p[2] = (byte)(myColor.R); //Red
                   p[1] = (byte)(myColor.G);//Green
                   p[0] = (byte)(myColor.B);//Blue
                   p += 3;

                   /*  valueIntensity = intensity[x] / (yScale * 100);
                 
                    p[2] = (byte)(255 * GetRedValue( valueIntensity)); //Red
                    p[1] = (byte)(255 * GetGreenValue(valueIntensity));//Green
                    p[0] = (byte)(255 * GetBlueValue(valueIntensity));//Blue
                   
                    p += 3;*/


               }

           }

           b.UnlockBits(bmData);

           return b;
       }



       private Color getColor2(double intensity)
       {
           int green = 0;
           int red = 0;
           int blue = 0;
           float degreesHue;
           Color mycolor;
           mycolor = Color.FromArgb(0, 0, 0);
           //  mycolor = Color.FromArgb(2, 0, 21);
           intensity = intensity / (1000);
           degreesHue = (int)(intensity / 300);
           if (intensity <= 300) return Color.FromArgb(255, mycolor.R, mycolor.G, mycolor.B);

           if (degreesHue > 300)
           {
               red = 255;
               blue = 255;
               green = 255;
           }
           else
           {
               degreesHue = 300 - degreesHue;

           }
           if (degreesHue <= 60)
           {
               /* blue = 0;
                red = 255;
                green =(int) ((degreesHue / 60.0f)*255);*/
               blue = 0;
               green = 0;
               mycolor = Color.Red;
               red = mycolor.R;

           }
           else if ((degreesHue > 60) && (degreesHue <= 120))
           {
               /*  blue = 0;
                 green = 255;
                 degreesHue = 60 - (120-degreesHue);
                 red = (int)((degreesHue / 60.0f) * 255);*/

               mycolor = Color.Yellow;
               blue = mycolor.B;
               green = mycolor.G;
               red = mycolor.R;
           }
           else if ((degreesHue > 120) && (degreesHue <= 180))
           {/*
                red = 0;
                green = 255;
                blue = (int)(((degreesHue-120) / 60.0f) * 255);*/

               mycolor = Color.Green;
               blue = mycolor.B;
               green = mycolor.G;
               red = mycolor.R;

           }

           else if ((degreesHue > 180) && (degreesHue <= 240))
           {/*
                red = 0;
                blue = 255;
                degreesHue = degreesHue - 180;
                green  = (int)(((60-degreesHue) / 60.0f) * 255);*/
               mycolor = Color.FromArgb(5, 1, 44);
               blue = mycolor.B;
               green = mycolor.G;
               red = mycolor.R;
           }
           else if ((degreesHue > 240) && (degreesHue <= 300))
           {
               /*
                 green = 0;
                 blue = 255;
                 red = (int)(((degreesHue-240) / 60.0f) * 255);*/
               //  mycolor = Color.FromArgb(2, 0, 21);
               mycolor = Color.FromArgb(0, 0, 0);
               blue = mycolor.B;
               green = mycolor.G;
               red = mycolor.R;
           }
           return Color.FromArgb(255,
               red,
               green,
               blue);
       }




        private void timer_Tick(object sender, EventArgs e) {
            try{
            _lock.EnterWriteLock();
             myBuffer.Render(pb.CreateGraphics());
            _lock.ExitWriteLock();
            }catch(Exception er) {}
          
        }   
      
    }
}
