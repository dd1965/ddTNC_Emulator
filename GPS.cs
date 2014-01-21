
//-----------------------------------------------------------------------
// <copyright file="GPS" company="(none)">
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TNCAX25Emulator
{
    class GPS
    {
        Boolean validframe = false;
        Queue GPSreceiveddataQueue = new Queue();
        byte[] GPS_Rx;
        Kiss kiss;
        Serial serialporthandlerTNC;
       
        public void setSerialport(Serial serialporthandlerTNC)
        {
            this.serialporthandlerTNC = serialporthandlerTNC;
          
        }

        public  Boolean processGPSframe(byte[] GPSframe)
        {
            GPS_Rx = GPSframe;
            int checksum = 0;
            int decchksum = 0;
            for (int i = 1; i < GPSframe.Length; i++)//was zero
            {
                if (GPSframe[i] == 0x2A)
                {
                  
                    try{
                   
                    int value; 
                    char receivedchecksumlsb = (char) GPSframe[GPSframe.Length-2];
                    string hexascii = ""+ receivedchecksumlsb;
                    decchksum = Convert.ToInt32(hexascii, 16);
                    char receivedchecksummsb = (char)GPSframe[GPSframe.Length - 3];
                    hexascii = ""+ receivedchecksummsb;
                    value  = Convert.ToInt32(hexascii, 16);
                    decchksum = (value * 16) + decchksum;
                  
                  }catch(Exception e){
                      validframe = false;
                      return validframe;
                  }
                    if (checksum != decchksum) 
                    {
                        validframe = false;
                         //Silently discard
                         //Test frame
                        //$GPGGA,092750.000,5321.6802,N,00630.3372,W,1,8,1.03,61.7,M,55.2,M,,*76
                        //$GPGGA,113835.815,3740.8027,S,14452.5965,E,1,03,50.0,129.3,M,-3.9,M,0.0,0000*42

                    }
                    else
                    {
                        if (Properties.Settings.Default.localGPRSenabled&&Properties.Settings.Default.kissenabled)
                        {
                            kiss = new Kiss();
                             byte[] encodedkissbuffer = kiss.encodeKissFrame(getGPSframe());

                             try
                             {
                                 (serialporthandlerTNC.getSerialPortRef()).Write(encodedkissbuffer, 0, encodedkissbuffer.Length); //Send data to Kiss port GPS sentence
                             }
                             catch (Exception e)
                             {
                                 System.Console.WriteLine("Error in sending to Kiss");
                             }
                        }
                        
                        validframe = true;
                       
                        //Enque Here Copy a new frame across with the correct data ie *and checksum removed.TBC
                            string[] payLoad;
                            string stringValue = ASCIIEncoding.ASCII.GetString(GPSframe);
                       
                         try
                        {
                            payLoad = stringValue.Split(',');
                            if ((payLoad[0] == "$GPGGA")&&(payLoad[6]!="0"))
                            {
                                //Latitude
                                string latstr = payLoad[2];
                                string degrees = latstr.Substring(0, 2);
                                string minutes = latstr.Substring(2, 2);
                                string decpart = latstr.Substring(4, 5);
                                double degreesd = Convert.ToDouble(degrees);
                                double minutesd = Convert.ToDouble(minutes);
                                minutesd = minutesd / 60;
                                double decpartd = (Convert.ToDouble(decpart)) / 60;
                                double lat = degreesd + minutesd + decpartd;
                                latstr = lat.ToString();
                                if (payLoad[3] == "S") latstr = "-" + latstr;
                                lat = Convert.ToDouble(latstr);
                                Usersetting.latituded = lat;
                                Usersetting.latitude = latstr;

                                //Longitude
                                string longstr = payLoad[4];
                                degrees = longstr.Substring(0, 3);
                                minutes = longstr.Substring(3, 2);
                                decpart = longstr.Substring(5, 5);
                                degreesd = Convert.ToDouble(degrees);
                                minutesd = Convert.ToDouble(minutes);
                                minutesd = minutesd / 60;
                                decpartd =((Convert.ToDouble(decpart)) / 60);
                                double longitude = degreesd + minutesd + decpartd;
                                longstr = longitude.ToString();
                                if (payLoad[5] == "W") longstr = "-" + longstr;
                                longitude = Convert.ToDouble(longstr);
                                Usersetting.longitutuded = longitude;
                                Usersetting.longitude = longstr;

                                double height;
                                string heightstr = payLoad[9];
                                height=Convert.ToDouble(heightstr);
                                Usersetting.height = heightstr;
                                Usersetting.heightd = height;
                            }
                            else
                            {//Ignore GPS string
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    
                    
                    
                    
                    }
                    break;
                }
                else
                {
                    checksum ^= GPSframe[i];
                }

            }
          // squareToLatLong("QF22kh56");Just for testing... it works
          return validframe;
        } 
        public byte[] getGPSframe(){
            return GPS_Rx;
        }
        public string  squareToLatLong(string qths) {
            qths = qths.ToUpper();
            char[] qth = qths.ToCharArray();

           
            double longitude = (qth.ElementAt(0) - 65) * 20 - 180;
            double latitude = (qth.ElementAt(1) - 65) * 10 - 90;

           longitude = longitude + (qth.ElementAt(2)-48) * 2;

            double latitudeinterim = (qth.ElementAt(3)-48) ;
            latitude = latitude + latitudeinterim;
            double longitudeinterim = (qth.ElementAt(4) - 65) * (1.0 / 12.0);// +(1.0 / 24.0);/// 5/60
            longitude = longitude + longitudeinterim;
            latitudeinterim = (qth.ElementAt(5) - 65) * (1.0 / 24.0); //+(1.0 / 48.0);//2.5/60
            latitude = latitude + latitudeinterim;
             longitude = longitude + (qth.ElementAt(6) - 48)*((1.0/12.0)/10)+((1.0/12.0)/10)/2;
             latitude = latitude + (qth.ElementAt(7) - 48) * ((1.0 / 24.0) / 10) + ((1.0 / 24.0) / 10) / 2;
            //Next line would divide grid by 24 to get the next pair. Then add the centre of the next pair...
            if (longitude <= 0) {
                    longitude = Math.Abs(longitude) + 'W';
            } else {
                    longitude = longitude + 'E';
            }

            if (latitude <= 0) {
                    latitude = Math.Abs(latitude) + 'S';
            } else {
                    latitude = latitude + 'N';
            }

        return longitude + "," +  latitude + "  (" + qth + ")";
        }
        public void tbc_calculate_grid_square(double latin, double lonin)
        {
            double latremainder;
            double lonremainder;
            latin += 90;
            lonin +=180;
            char[] locator = new char[8];
            
            //Calculate lon part of grid square
            locator[0] = (char)('A' + ((char)((lonin/ 20))));
            lonremainder = lonin % 20;
            locator[2] = (char)((((lonremainder/ 2))+'0'));
            lonremainder = lonremainder % 2;
            locator[4]= (char)('A' + ((char)((lonremainder/ 0.083333))));
            lonremainder= lonremainder%0.083333;
            locator[6] = (char)((((lonremainder / 0.008333)) + '0'));
            //Calculate lat part of grid square
            locator[1] = (char)('A' + ((char)(((latin) / 10))));
            latremainder = latin % 10;
            locator[3] = (char)((((latremainder / 1)) + '0'));
            latremainder = latremainder % 1;
            locator[5] = (char)('A' + ((char)(((latremainder) / 0.0416665))));
            latremainder = latremainder % 0.0416665;
            locator[7] = (char)((((latremainder / 0.004166)) + '0'));
            string s = new string(locator);
            Console.WriteLine(s);
        }

       
     }
}
