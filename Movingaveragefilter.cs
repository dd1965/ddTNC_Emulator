//-----------------------------------------------------------------------
//Adapted from c code from part of FLDIGI and converted to C# by VK3TBC


namespace TNCAX25Emulator
{
    
    class Movingaveragefilter
    {
        int length;
        int j = 0;
       
        double[] input;
        double output;

       public  Movingaveragefilter(int length){
           this.length = length;
	      input = new double[length];      
        }

       public double runFiltAverager(double datain){	    
	     output = output - input[j] + datain;
	     input[j] = datain;
	     if (++j >= length) j = 0;
	     return (output / length);
        }
    }
}
