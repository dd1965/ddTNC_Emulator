using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TNCAX25Emulator
{
    class  Aprs 
    {
        object reference;
        string speed;
        string satellites;
        string GPSfix;
        string tempin;
        string tempout;
        string Vin;
        string alt;
        string longitude;
        string time;
        string sequence;
        string lat;

        public Aprs(object reference){
            this.reference = reference;
        }
        
        public byte[] constructAX25APRS(byte[] aprstosend,string callsign,string path,string info){
            
            int payloadlength=aprstosend.Length;
            byte[] pathdaddress=null;
            byte[] formattedpayload = constructPayload(aprstosend,info);
            if (formattedpayload == null) return null;
            int pathbytecnt = 0;
           
            if ((callsign==null) | (callsign=="")) return null; //Don't send the APRS frame without a call.
            callsign = padAddress(callsign);
            byte[] arraycallsignsaddress = Encoding.ASCII.GetBytes(callsign);
            byte[] arraycallsigndaddress = Encoding.ASCII.GetBytes("APRS  ");
            if (path != "")
            {

                path = padAddress(path);
                pathdaddress = Encoding.ASCII.GetBytes(path);
            }
            
            byte[] destaddress = new byte[7];
            byte[] sourceaddress = new byte[7];
            byte[] pathaddress = new byte[7];
            byte control= 0x03;
            byte pid = 0xF0;
            destaddress = buildaddress(arraycallsigndaddress,0);
            sourceaddress = buildaddress(arraycallsignsaddress,checkandcreateSSID(arraycallsignsaddress));
            //TO DO add code so that path does not need to be there
            if (path != "")
            {
                pathaddress = buildaddress(pathdaddress, checkandcreateSSID(pathdaddress));
                uint finalpathaddressbit = pathaddress[6];
                finalpathaddressbit = finalpathaddressbit | 1;
                pathaddress[6] = (byte)finalpathaddressbit;
                pathbytecnt = 7;
            }
            else
            {
                uint finalpathaddressbit = sourceaddress[6];
                finalpathaddressbit = finalpathaddressbit | 1;
                sourceaddress[6] = (byte)finalpathaddressbit;
            }


            byte[] AX25aprsframe = new byte[16+pathbytecnt + formattedpayload.Length];  //Lets assume destination (7), source (7) and path (7) control (1) and pid (1) and payload.
            //Array.Copy(source array, sourceindex, destination arrary, destination index,Length);
            Array.Copy(destaddress, 0, AX25aprsframe, 0, 7);
            Array.Copy(sourceaddress, 0, AX25aprsframe, 7, 7);
            //TO DO change code here for multiple paths.
            if (pathbytecnt > 0)
            {
                Array.Copy(pathaddress, 0, AX25aprsframe, 14, 7);
            }
            AX25aprsframe[14+pathbytecnt] = control;
            AX25aprsframe[15+pathbytecnt] = pid;
            Array.Copy(formattedpayload, 0, AX25aprsframe, 16+pathbytecnt, formattedpayload.Length);
           
            return AX25aprsframe;
        }
        private byte[] buildaddress(byte[] address,byte ssid){
             byte[] finaladdress = new byte[6];
             byte[] totaladdress = new byte[7];

             //Array.Copy(source array, sourceindex, destination arrary, destination index,Length);
             Array.Copy(address, 0, finaladdress, 0, 6);
             finaladdress = rotateAddress(finaladdress);
             Array.Copy(finaladdress, 0, totaladdress, 0, 6);
             if (ssid == 0)
             {
                 totaladdress[6] = 0;
             }
             else
             {
                 totaladdress[6] = Rotate.RotateLeft(ssid, 1);
             }
             return totaladdress;

        }
        private byte[] rotateAddress(byte[] address)
        {
            byte[] raddress = new byte[address.Length];
            for (int i = 0; i < address.Length; i++)
            {
                raddress[i] = Rotate.RotateLeft(address[i],1);
            }
            return raddress;
        }

        private byte checkandcreateSSID(byte[] callsigntocheck)
        {
            byte SSIDtoreturn=0;
            try
            {
                if (callsigntocheck.Length > 6) //Must have SSID > 0
                {
                    if (callsigntocheck.Length == 8)
                    {
                        if (callsigntocheck[6] == 0x2D)                //It should be a valid callsign TODO move this to textbox entry
                        {

                            SSIDtoreturn = callsigntocheck[7];
                            char ssid = (char)SSIDtoreturn;
                            string ascii = "" + ssid;
                            SSIDtoreturn = (byte)Convert.ToInt32(ascii, 10);


                        }
                       
                    }
                    else if (callsigntocheck.Length == 9)
                    {
                        if (callsigntocheck[6] == 0x2D)                //It should be a valid callsign TODO move this to textbox entry
                        {
                            SSIDtoreturn = callsigntocheck[8];
                            char ssid = (char)SSIDtoreturn;
                            string ascii = "" + ssid;
                            SSIDtoreturn = (byte)Convert.ToInt32(ascii, 10);
                            int SSIDx10 = callsigntocheck[7];
                            ssid = (char)SSIDx10;
                            ascii = "" + ssid;
                            SSIDx10 = (byte)(Convert.ToInt32(ascii, 10) * 10);
                            SSIDtoreturn = (byte)(SSIDtoreturn + SSIDx10);
                        }                      
                    }                 
                }               
            }
            catch (Exception e)
            {
                return SSIDtoreturn=0;//If we detect an illegal SSID at this point, just set it to 0.
            }
            return SSIDtoreturn;
        }
        private string padAddress(string address)
        {
           
            /*Need to pad the address with spaces if the callsign is less than 6 characters*/
            string[] payLoad = address.Split('-');
            if (payLoad[0].Length < 6)
            {
                int tpathlength = payLoad[0].Length;
                for (int i = tpathlength; i < 6; i++)
                {
                    payLoad[0] = payLoad[0] + " ";
                }
                if (payLoad.Length > 1)
                {
                    address = payLoad[0] + "-" + payLoad[1];
                }
                else
                {
                    address = payLoad[0];
                }
            }
            return address;
        }

        private byte[] constructPayloadold(byte[] aprstosend, string info)
        {
            //$$PSB,sequence,time,lat,long,altitude,speed,satellites,temp_in,temp_out,Vin*CHECKSUM\n 
            //e.g.$$PSB,0164,233252,-37.8534,144.9265,6,1,4,1,-67,107,2719*516A
            //$$PSB,0110,024228,-37.7536,144.9264,46,0,7,1,-13,52,7125*F1DD
            //New format  17/03/2013
            //$$PSB,599,07:17:53,-36.2431,143.1503,431,5.12,-2.14,9,3,34,31,3032,_*BBD7
            //note the \n is already stripped
            //count, hour:minute:second, lat_str, lon_str, alt, gspeed, veld, sats, lock, Temp_in,Temp_out,Vbatt,debug*CHECKSUM
            string[] payLoad;
            string stringValue = ASCIIEncoding.ASCII.GetString(aprstosend);
            payLoad = stringValue.Split(',');
            try
            {


                sequence = payLoad[1];

                string timerx = payLoad[2];
                //timerx = "012345";
                time = timerx;
                string[] ctime = timerx.Split(':');
                if (ctime.Length == 3)
                    time = ctime[0] + ctime[1] + ctime[2];

                //Need to parse time now

                lat = payLoad[3];
               
                longitude = payLoad[4];

               // longitude = getlongitude(longitude);
                // lat = "38.0000";
                // longitude = "145.01";
                if ((!lat.StartsWith("0")) && (!longitude.StartsWith("0")))
                {

                    string sign = "+";
                    if (lat.StartsWith("-")) { sign = "-"; }
                    double value = Convert.ToDouble(lat);
                    value = Math.Abs(value);
                    int degrees = (int)value;
                    string deg = degrees.ToString();
                    double minutes = ((value - degrees) * 60);
                    minutes = Math.Round(minutes, 2);
                    string min = minutes.ToString();
                    string[] rem;
                    string sec;
                    if ((min == "0") && (min.Length == 1))
                    {
                        min = "0";
                        sec = "0";
                    }
                    else
                    {
                        rem = min.Split('.');

                        min = rem[0];
                        sec = rem[1];
                    }

                    if (sec.Length < 2) sec = sec + "0";

                    int minValue = Convert.ToInt16(min);
                    if (minValue < 10)
                    {
                        min = "0" + min;
                    }
                    lat = "" + deg + min + "." + sec;

                    if (sign == "-")
                    {
                        lat = lat + "S";
                    }
                    else
                    {
                        lat = lat + "N";
                    }
                    if (lat.Length < 8)
                    {
                        for (int i = lat.Length; i < 8; i++)
                        {
                            lat = "0" + lat;
                        }
                    }

                    // longitude = payLoad[4];
                    sign = "+";
                    if (longitude.StartsWith("-")) { sign = "-"; }

                    value = Convert.ToDouble(longitude);
                    value = Math.Abs(value);
                    degrees = (int)value;
                    minutes = ((value - degrees) * 60);
                    minutes = Math.Round(minutes, 2);
                    min = minutes.ToString();
                    rem = min.Split('.');

                    if ((min == "0") && (min.Length == 1))
                    {
                        min = "0";
                        sec = "0";
                    }
                    else
                    {
                        rem = min.Split('.');

                        min = rem[0];
                        sec = rem[1];
                    }


                    if (sec.Length < 2) sec = sec + "0";
                    minValue = Convert.ToInt16(min);
                    if (minValue < 10)
                    {
                        min = "0" + min;
                    }
                    longitude = "" + degrees + min + "." + sec;


                    if (longitude.StartsWith("-"))
                    {
                        longitude = longitude + "W";
                    }
                    else
                    {
                        longitude = longitude + "E";
                    }
                    if (longitude.Length < 9)
                    {
                        for (int i = longitude.Length; i < 9; i++)
                        {
                            longitude = "0" + longitude;
                        }
                    }
                }
                alt = payLoad[5];//In feet
                // alt = "0";
                int altitudemtr = Convert.ToInt32(alt);
                double altitduefeet = 3.2808399 * altitudemtr;//APRS protocol is in feet.
                altitduefeet = Math.Round(altitduefeet, 0);
                int altft = (int)(altitduefeet);
                alt = altft.ToString();
                for (int i = alt.Length; i < 6; i++)
                {
                    alt = "0" + alt;
                }

                speed = payLoad[6];
                string veld = payLoad[7];
                satellites = payLoad[8];
                GPSfix = payLoad[9];
                tempin = payLoad[10];
                tempout = payLoad[11];
                Vin = payLoad[12];
                string debug = payLoad[13];
                string[] debugx = debug.Split('*');//Need to separate out the * and CRC
                debug = debugx[0];
                string icon = "O";
                string symtableID = "/";
                string AltitudeExtend = " /A=";
                TNCAX25Emulator.Receivedparameters.sequence = sequence;
                TNCAX25Emulator.Receivedparameters.time = timerx;
                TNCAX25Emulator.Receivedparameters.lat = lat;
                TNCAX25Emulator.Receivedparameters.longitude = longitude;
                TNCAX25Emulator.Receivedparameters.speed = speed;
                TNCAX25Emulator.Receivedparameters.tin = tempin;
                TNCAX25Emulator.Receivedparameters.tout = tempout;
                TNCAX25Emulator.Receivedparameters.satno = satellites;
                TNCAX25Emulator.Receivedparameters.gpsfix = GPSfix;
                TNCAX25Emulator.Receivedparameters.volts = Vin;
                TNCAX25Emulator.Receivedparameters.altiude = altitudemtr.ToString();//alt now displays in meters
               // TNCAX25Emulator.Receivedparameters.veld = veld;
                TNCAX25Emulator.Receivedparameters.debug = debug;
                // time = "024228";
                //  alt = "00000";
                string stringtosend = "/" + time + "h" + lat + symtableID + longitude + icon + AltitudeExtend + alt + " " + " Sq" + sequence + " " + "S" + speed + " " + "Ti" + tempin + "C " + "To" + tempout + "C " + Vin + "mV " + "Sa" + satellites + " GF" + GPSfix + " De" + debug + " " + info;
                byte[] payload = Encoding.ASCII.GetBytes(stringtosend);
                return payload;
            }
            catch (Exception e)
            {
                TNCAX25Emulator.Receivedparameters.sequence = payLoad[1];
                TNCAX25Emulator.Receivedparameters.time = payLoad[2];
                TNCAX25Emulator.Receivedparameters.lat = payLoad[3];
                TNCAX25Emulator.Receivedparameters.longitude = payLoad[4];
                TNCAX25Emulator.Receivedparameters.speed = payLoad[6];
                TNCAX25Emulator.Receivedparameters.tin = payLoad[9];
                TNCAX25Emulator.Receivedparameters.tout = payLoad[10];
                TNCAX25Emulator.Receivedparameters.satno = payLoad[7];
                TNCAX25Emulator.Receivedparameters.gpsfix = payLoad[8];
                TNCAX25Emulator.Receivedparameters.volts = payLoad[11];
                TNCAX25Emulator.Receivedparameters.altiude = payLoad[5]; //alt now displays in meters
              //  TNCAX25Emulator.Receivedparameters.veld = payLoad[7]; ;
                TNCAX25Emulator.Receivedparameters.debug = payLoad[12];

                string icon = "O";
                string symtableID = "/";
                string AltitudeExtend = " /A=";
                string stringtosend = "/" + time + "h" + payLoad[3] + symtableID + payLoad[4] + icon + AltitudeExtend + payLoad[5] + " " + " Sq" + payLoad[1];
                byte[] payload = Encoding.ASCII.GetBytes(stringtosend);
                return payload;
            }
        }


 private byte[] constructPayload(byte[] aprstosend,string info){
           //$$PSB,sequence,time,lat,long,altitude,speed,satellites,temp_in,temp_out,Vin*CHECKSUM\n 
           //e.g.$$PSB,0164,233252,-37.8534,144.9265,6,1,4,1,-67,107,2719*516A
           //$$PSB,0110,024228,-37.7536,144.9264,46,0,7,1,-13,52,7125*F1DD
           //New format  17/03/2013
           //$$PSB,599,07:17:53,-36.2431,143.1503,431,5.12,-2.14,9,3,34,31,3032,_*BBD7
           //note the \n is already stripped
           //count, hour:minute:second, lat_str, lon_str, alt, gspeed, veld, sats, lock, Temp_in,Temp_out,Vbatt,debug*CHECKSUM
           string[] payLoad;
           string stringValue = ASCIIEncoding.ASCII.GetString(aprstosend);
           payLoad = stringValue.Split(',');
           try
           {    
               sequence = payLoad[1];
                //Need to parse time now
               string timerx = payLoad[2];
               time = timerx;
               string[] ctime = timerx.Split(':');
               if (ctime.Length==3)
               time = ctime[0]+ctime[1]+ctime[2];           
               //Calc lat and long
               lat = payLoad[3];
               longitude = payLoad[4];           
               lat = getlat(lat);
               longitude = getlongitude(longitude);  
               //Cal altitude in feet
               alt = payLoad[5];        
               double altitudemtr = Convert.ToDouble(alt);
               TNCAX25Emulator.Receivedparameters.altituded = altitudemtr;
               double altitduefeet = 3.2808399 * altitudemtr;//APRS protocol is in feet.
               altitduefeet = Math.Round(altitduefeet, 0);
               int altft = (int)(altitduefeet);              
               alt = String.Format("{0:000000}", altft);           
               //Add rest of parameters
               speed = payLoad[6];
              // string veld = payLoad[7];
               satellites = payLoad[7];
               GPSfix = payLoad[8];
               tempin = payLoad[9];
               tempout = payLoad[10];
               Vin = payLoad[11];
               string[] Vinx = Vin.Split('*');
               Vin = Vinx[0];
              // string debug = payLoad[12];
             //  string[] debugx = debug.Split('*');//Need to separate out the * and CRC
             //  debug = debugx[0];
               string icon = "O";
               string symtableID = "/";
               string AltitudeExtend = " /A=";
               TNCAX25Emulator.Receivedparameters.psbcallsign = payLoad[0];
               TNCAX25Emulator.Receivedparameters.sequence = sequence;
               TNCAX25Emulator.Receivedparameters.time = timerx;
               TNCAX25Emulator.Receivedparameters.lat = lat;
               TNCAX25Emulator.Receivedparameters.longitude = longitude;
               TNCAX25Emulator.Receivedparameters.speed = speed;
               TNCAX25Emulator.Receivedparameters.tin = tempin;
               TNCAX25Emulator.Receivedparameters.tout = tempout;
               TNCAX25Emulator.Receivedparameters.satno = satellites;
               TNCAX25Emulator.Receivedparameters.gpsfix = GPSfix;
               TNCAX25Emulator.Receivedparameters.volts = Vin;
               TNCAX25Emulator.Receivedparameters.altiude = altitudemtr.ToString();//alt now displays in meters
              // TNCAX25Emulator.Receivedparameters.veld = veld;
              // TNCAX25Emulator.Receivedparameters.debug=debug;
              // time = "024228";
             //  alt = "00000";
               string stringtosend = "/" + time + "h" + lat + symtableID + longitude + icon + AltitudeExtend + alt+" "+ " Sq" + sequence + " " + "S" + speed + " "  + "Ti" + tempin + "C " + "To" + tempout + "C " + Vin + "mV " + "Sa" + satellites + " GF" + GPSfix + " " + info;
               byte[] payload = Encoding.ASCII.GetBytes(stringtosend);
               return payload;
           }
           catch (Exception e)
           {
               try
               {
                   TNCAX25Emulator.Receivedparameters.psbcallsign = payLoad[0];
                   TNCAX25Emulator.Receivedparameters.sequence = payLoad[1];
                   TNCAX25Emulator.Receivedparameters.time = time;
                   TNCAX25Emulator.Receivedparameters.lat = lat;
                   TNCAX25Emulator.Receivedparameters.longitude = longitude;
                   TNCAX25Emulator.Receivedparameters.altiude = payLoad[5];
                   TNCAX25Emulator.Receivedparameters.speed = payLoad[6];
                   // TNCAX25Emulator.Receivedparameters.veld = payLoad[7];
                   TNCAX25Emulator.Receivedparameters.satno = payLoad[7];
                   TNCAX25Emulator.Receivedparameters.gpsfix = payLoad[8];
                   TNCAX25Emulator.Receivedparameters.tin = payLoad[9];
                   TNCAX25Emulator.Receivedparameters.tout = payLoad[10];
               }
               catch (Exception calcTel)
               {
                   System.Console.WriteLine("Error in APRS. Construct payload()");
               }
               try
               {
                   if (payLoad.Length >= 12)
                   {
                       Vin = payLoad[11];
                       string[] Vinx = Vin.Split('*');
                       Vin = Vinx[0];
                       TNCAX25Emulator.Receivedparameters.volts = Vin;
                   }
               }
               catch (Exception iex) { }
               try{
               if (payLoad.Length == 13) TNCAX25Emulator.Receivedparameters.debug = payLoad[12];                                     
               string icon = "O";
               string symtableID = "/";
               string AltitudeExtend = " /A=";
               string stringtosend = "/" + time + "h" + lat + symtableID + longitude + icon + AltitudeExtend + alt+" "+ " Sq"+ payLoad[1]; 
               byte[] payload = Encoding.ASCII.GetBytes(stringtosend);
             
               return payload;
               }
               catch (Exception eex) {
                   byte[] payload = Encoding.ASCII.GetBytes("Null");
                   return payload; 
               }
           }
 }
 
     private string getlat(String lat){
         string sign = "+";
         if (lat.StartsWith("-")) { sign = "-"; }
         double value = Convert.ToDouble(lat);
         TNCAX25Emulator.Receivedparameters.latituded = value;
         value = Math.Abs(value);
         int degrees = (int)value;
         double minutesandfracminutes = (value - degrees) * 60;
         degrees = degrees * 100;
         double convertedlat = degrees + minutesandfracminutes;
        
         lat = String.Format("{0:0000.00}",convertedlat);
         if (sign == "+") lat = lat + "N"; else lat = lat + "S";              
         return lat;

     }

       private string getlongitude(String longitude){
           string sign = "+";
           if (longitude.StartsWith("-")) { sign = "-"; }
           double value = Convert.ToDouble(longitude);
           TNCAX25Emulator.Receivedparameters.longituded = value;
           value = Math.Abs(value);
           int degrees = (int)value;
           double minutesandfracminutes = (value - degrees) * 60;
           degrees = degrees * 100;
           double convertedlong = degrees + minutesandfracminutes;
           longitude= String.Format("{0:00000.00}", convertedlong);
           if (sign == "+") longitude = longitude + "E"; else longitude = longitude + "W";
           return longitude;
     }

       private string truncate(string value, int maxLength)
       {
           if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
           {
               return value.Substring(0, maxLength);
           }

           return value;
       }
    }
    
}
