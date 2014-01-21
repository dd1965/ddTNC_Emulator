using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNCAX25Emulator
{
   
    
    static class Range_Elevation
    {
        static double COSD = 0;
        static double COSC = 0;
        static double earthradius= 6371.00;
        static double oneoverk = 0;
        static string range = "0";
        static string elevation = "0";    
        public static void CalcRangeElevationBearting(double lat, double longitude,double height)
        /* Assumes user lat and long have been set in user setting!*/
        {
            oneoverk=earthradius/((height/1000)-(Usersetting.heightd/1000)+earthradius);
            COSD = Math.Sin(Usersetting.latituded * (Math.PI / 180)) * Math.Sin(lat * (Math.PI / 180)) + Math.Cos(Usersetting.latituded * (Math.PI / 180)) * Math.Cos(lat * (Math.PI / 180)) * Math.Cos((Usersetting.longitutuded-longitude) * (Math.PI / 180));
        
            COSC = (Math.Sin(lat* (Math.PI / 180))-Math.Sin(Usersetting.latituded* (Math.PI / 180))*COSD)/(Math.Cos(Usersetting.latituded* (Math.PI / 180))*Math.Sin(Math.Acos(COSD)));
            //Add bearing code correction depending on long.
          //  COSC= (-Math.Tan(Usersetting.latituded* (Math.PI / 180)))/(Math.Tan((Usersetting.longitutuded-longitude) * (Math.PI / 180)));
          
        }
        public static String getRange()
        {
            double ranged = earthradius*Math.Sqrt(1-2*COSD*oneoverk+oneoverk*oneoverk);
            return string.Format("{0:0.00}", ranged);
        }
        public static String getElevation()
        {
            double elevationd = (COSD - oneoverk) / Math.Sqrt(1-COSD * COSD);
            elevationd = Math.Atan(elevationd)*(180/Math.PI);
            if (double.IsNaN(elevationd)) elevationd = 0;
            String fstring = string.Format("{0:0.00}",elevationd);
            return fstring;
        }
        public static String getBearing()
        {
            double bearing = Math.Acos(COSC) * (180 / Math.PI);
             if(( Usersetting.longitutuded-Receivedparameters.longituded)>0) bearing= 360-bearing;
             if (double.IsNaN(bearing)) bearing = 0;
             return string.Format("{0:0.00}", bearing);
        }
    }
}
