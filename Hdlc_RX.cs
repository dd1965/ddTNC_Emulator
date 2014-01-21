using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNCAX25Emulator
{
    class Hdlc_RX
    {
        Boolean Flagstart = false;
        int last_inbyte;
        int oldstate;
        byte cbyte,cbyteax25 = 0;
        int cnt;
        byte toSend;
        int bitcnt,bitcntax25=8;
        int bitcntdetect = 64;
        UInt64 phasedetectbits;
        int ones = 0;
        int byteCnt,byteCntax25 = 0;
        Boolean test = false;
        byte[] rxByte = new byte[512];
        byte[] rxByteax25 = new byte[512];
        int syncmidbitcount = 20;// baud
        int N1200 = 20;
        int  N = 40;//for 9600 = 20. Interpolation to 192K
        UInt16 PHASE_INC;
        UInt16 PHASE_CORR;
        UInt16 bit_phase;
        int bitstoaligncnt = 0;
        uint shreg;
        uint txshreg;
        int synccnt;
        Goldcode rxGcode,rxGcodeTelemetry;
        int bytestoCount = 256;
        byte dbuffer;
        byte sample_char;
        object reference;
        Shuffle sh;
        byte[] byteArray;
        Serial serialporthandlerTNC;
        int baud;

        public Hdlc_RX(int baud,object reference){
            this.reference = reference;
            this.baud = baud;
            if (baud == 1200)
            {
                N = Config.samplingrate / baud;

            }
            else
            {
                N= (Config.samplingrate) * 8 / baud;//Upsampled by 8. TODO make this dynamic
            }

            rxGcode = new Goldcode();
            rxGcodeTelemetry =new Goldcode(1);

            PHASE_INC = (UInt16)(65536 / N);
            PHASE_CORR = (UInt16)(PHASE_INC / 2);
            sh = new Shuffle();

        }

        public enum State
        {
            IDLE, FLAGDETECTED, DATADECODE
        };
        public enum SyncState
        {
            DETECT,WAITFORHIGH,WAITFORLOW,DECODING
        };
        public enum goldcodestate
        {
            HUNT, DETECTED, DETECTEDTELEMETRYFRAME, ALIGNBYTE
        }
        goldcodestate CodeState = goldcodestate.HUNT;
        int phase;
        SyncState syncstate = SyncState.DETECT;
        State state = State.IDLE;
        
        public void bitin(int inbit)
        {  					//function to read a bit
            try
            {
                if (byteCntax25 == 511) //was 254
                {
                    System.Array.Clear(rxByteax25, 0, rxByteax25.Length);
                    byteCntax25 = 0;
                    syncstate = SyncState.DETECT;
                    state = State.IDLE;
                    cbyteax25 = 0;
                    bitcntax25 = 8;
                    test = false;
                }
              
                if (inbit != oldstate)
                {    	                //if state has changed
                                  //update oldstate		
                    toSend = 0;
                    //return 0 if state changed
                }
                else
                    toSend = 0x80;

                oldstate = inbit;  
                switch (state)
                {
                    case State.IDLE:
                        {
                            Flagstart = search_For_FLAG(toSend);
                            if (Flagstart) state = State.FLAGDETECTED;
                            return;
                            //break;
                        }

                    case State.FLAGDETECTED:
                        {

                            if (getData(toSend) == 2)
                            {
                                state = State.DATADECODE;
                                rxByteax25[0] = cbyteax25;
                                byteCntax25++;

                                //Console.WriteLine(byteCnt);
                            }

                            return;
                        }


                    case State.DATADECODE:
                        {
                           // Console.WriteLine(byteCnt);
                            if ((test) && (toSend == 0x80))
                            {


                                Console.WriteLine("ENDFLAG");
                                if (byteCntax25 > 17)
                                {
                                    
                                    ushort crc,crccalc;
                                    crccalc = CRC16(rxByteax25);
                                    crc = (ushort)(rxByteax25[byteCntax25 - 1]);
                                    crc =( ushort)(crc << 8);
                                    crc = (ushort)(crc + rxByteax25[byteCntax25-2]);

                                    if (crc == crccalc) 
                                    { decodeAX25();
                                        // decodeAX25(); while (L < byteCnt)
                                       /* for(int i=0;i<byteCnt;i++)
                                        {
                                            Form1.Self.SetTextRTTY(((char)(rxByte[i])).ToString());
                                           // Form1.Self.SetTextRTTY(" ");
                                         
                                        }
                                        Form1.Self.SetTextRTTY("\n");	*/
                                    if (Usersetting.kissenabled)
                                    {
                                        Kiss kiss = new Kiss();
                                        byte[] msgbuffer = new byte[byteCntax25-3];
                                        System.Array.Copy(rxByteax25, msgbuffer, byteCntax25-3);
                                        byte[] encodedkissbuffer = kiss.encodeKissFrame(msgbuffer);
                                        Serial tncref = (Serial)((Form1)(reference)).getserialTNCref();
                                        tncref.getSerialPortRef().Write(encodedkissbuffer, 0, encodedkissbuffer.Length);
                                    }
                                    //    if (rxByte[0] == 0x55)
                                     //   {
                                      //      MessageHandler.receivepicturequeue.Enqueue(msgbuffer);
                                     //   }
                                      //  else
                                       //     MessageHandler.receivequeue.Enqueue(msgbuffer);
                                    }
                                }
                                System.Array.Clear(rxByteax25, 0, rxByteax25.Length);
                                byteCntax25 = 0;
                                syncstate = SyncState.DETECT;
                                state = State.IDLE;
                                cbyteax25 = 0;
                                bitcntax25 = 8;
                                test = false;
                                //shreg = 0;
                                return;
                            }
                            else if ((test) && (toSend == 0))
                            {
                                test = false;
                                return;
                            }

                            if (toSend == 0x80) ones++;  		 //increment the ones counter
                            else ones = 0;			             //if bit is a zero, reset the ones counter
                            if (ones == 5)
                            {  			         //removes bit stuffing
                                test = true;	                     //get the next bit but don’t add it to cbyte
                                ones = 0; 			                 //reset the ones counter                       
                            }

                            int tres = getData(toSend);

                            if (tres == 2)
                            {
                                rxByteax25[byteCntax25++] = cbyteax25;
                               // Form1.Self.SetTextRTTY(((char)(cbyte)).ToString());
                               
                                //Console.WriteLine(cbyte);


                                cbyteax25 = 0;
                            }

                            return;
                        }

                    default: break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
        
        
        public Boolean search_For_FLAG(byte bitin)
        {
            //bit in must be on the MSB.
            cbyteax25 = (byte)(cbyteax25 >> 1);
            cbyteax25 = (byte)((int)cbyteax25 | (int)bitin);
              				//initialize
            if (cbyteax25 != 0x7e)
            {
                //find the first flag
                
                return false;
            }
            else
            {
                cbyteax25 = 0;
                //output_high(LED);  //turn on the DCD light
               // cnt++;
               // Console.WriteLine(cnt);
                return true;
            }

        }
        public int getData(byte bitin)
        {
            cbyteax25 = (byte)(cbyteax25 >> 1);
            cbyteax25 = (byte)((int)cbyteax25 | (int)bitin);
            //if (bitin == 0) Console.Write(0); else Console.Write(1);
            bitcntax25--;
            if (bitcntax25 == 0)
            {
                if (cbyteax25 == 0x7e)
                {


                    cbyteax25 = 0;
                        bitcntax25 = 8;
                        Console.WriteLine("FLAG");
                  
                    return 1;
                }
                else
                {
                    
                    bitcntax25 = 8;                                
                   // cbyte = 0;
                    return 2;
                }
            }
            return 3;                              
        }
        public int getData10(byte bitin)
        {
          //  if (bitin == 0) Console.Write(0); else Console.Write(1);
            if (bitin == 1) bitin = 0x80;
            cbyte = (byte)(cbyte >> 1);
            cbyte = (byte)((int)cbyte | (int)bitin);
            //Console.WriteLine(cbyte);
            bitcnt--;
            if (bitcnt == 0)
            {
                 //  Console.WriteLine("8 bits ");
                    bitcnt = 8;
                   // cbyte = 0;
                    return 2;
                
            }
            return 3;
        }
        public int getData_4bit(byte bitin)
        {
            //  if (bitin == 0) Console.Write(0); else Console.Write(1);
            if (bitin == 1) bitin = 0x80;
            cbyte = (byte)(cbyte >> 1);
            cbyte = (byte)((int)cbyte | (int)bitin);
            //Console.WriteLine(cbyte);
            bitcnt--;
            if (bitcnt == 0)
            {
                //  Console.WriteLine("8 bits ");
                bitcnt = 4;
                // cbyte = 0;
                return 2;

            }
            return 3;
        }
        public int getData1(byte bitin)
        {

            if (CodeState == goldcodestate.DETECTED)
            {
                return (getData10(bitin));
            }
            //  int bitcntdetect = 64;
            UInt64 bitin64=0;
            //   if (bitin == 0) Console.Write(0); else Console.Write(1);
            if (bitin == 1) bitin64 = 0x8000000000000000;
            phasedetectbits = (phasedetectbits >> 1);
            phasedetectbits = ((UInt64)phasedetectbits | (UInt64)bitin64);
            //Console.WriteLine(cbyte);
            int sum = rxGcode.decodeGold1(phasedetectbits); 
            if (sum > 40){
                   bitcnt = 8;
                 return 2;
            }
         /*   bitcntdetect--;
            if (bitcntdetect == 0)
            {
                //  Console.WriteLine("8 bits ");
                bitcntdetect = 1;
                byteArray = BitConverter.GetBytes(phasedetectbits);
                for (int i = 0; i < 8; i++)
                {
                    phase = rxGcode.decodeGold(byteArray[i], 40); 
                }
               // if (phase < 8) Console.WriteLine("Phase " +phase);
                if (phase == 0)
                {
                    bitcntdetect = 64;
                    bitcnt = 8;
                    return 2;
                }
                
                //phase = rxGcode.decodeGold(cbyte, 40);
                // cbyte = 0;
                return 0;

            }*/
            return 3;
        }
        public void decodeAX25(){
            int i, L, m, temp;
            Form1.Self.SetTextRTTY("\n");
            for (m = 7; m < 13; m++)
            {					//print the source callsign
                if (rxByteax25[m] != 0x40) Form1.Self.SetTextRTTY(((char)(rxByteax25[m] >> 1)).ToString());			
            }
              Form1.Self.SetTextRTTY("-");
              Form1.Self.SetTextRTTY(((char)(rxByteax25[13] & 0x1F) >> 1).ToString());  				//print source SSID
              Form1.Self.SetTextRTTY(">");
              for (m = 0; m < 6; m++)
              {           					//print the destination callsign
                  if (rxByteax25[m] != 0x40) Form1.Self.SetTextRTTY(((char)(rxByteax25[m] >> 1)).ToString());
              }								
              Form1.Self.SetTextRTTY("-");
              Form1.Self.SetTextRTTY(((char)(rxByteax25[6] & 0x1F) >> 1).ToString());
              L = 7;

              if ((rxByteax25[13] & 0x01) != 1)
              {  					 //print any path that may exist
                  do
                  {
                      Form1.Self.SetTextRTTY(",");
                      L = L + 7;
                      for (m = L; m < (L + 6); m++)
                      {
                          if (rxByteax25[m] != 0x40)  Form1.Self.SetTextRTTY(((char)(rxByteax25[m] >> 1)).ToString());
                      }						
                        Form1.Self.SetTextRTTY("-");
                        Form1.Self.SetTextRTTY(((char)(rxByteax25[(L + 6)] & 0x0F) >> 1).ToString());
                  } while (((rxByteax25[L + 6] & 0x01) != 1)&&((L+6 < byteCntax25)));
              }								
                Form1.Self.SetTextRTTY(": ");
              
                L = L + 9;							
                while (L < byteCntax25-2)
                {  				 	
                    Form1.Self.SetTextRTTY(((char)(rxByteax25[L])).ToString());
                    L++;
                }
                Form1.Self.SetTextRTTY(" CRC->");	
                while (L < byteCntax25)
                {
                    Form1.Self.SetTextRTTY((rxByteax25[L]).ToString());
                    Form1.Self.SetTextRTTY(" ");	
                    L++;
                }				
                Form1.Self.SetTextRTTY("\n");						
        }
        public void decode(int inbyte)
        {
            if (last_inbyte != inbyte)
            {
                if (bit_phase < 0x8000)
                    bit_phase += PHASE_CORR;
                else
                    bit_phase -= PHASE_CORR;
            }
            last_inbyte = inbyte;
            int temp = bit_phase + PHASE_INC;
            bit_phase =(UInt16)(temp & 0xffff);
            if (temp > 0xffff)
            {
               /* txshreg <<= 1;
                if (inbyte == 1) txshreg |= 1;
                if ((txshreg & 0x20000) == 0x20000) txshreg ^= 0x0021;
                if ((txshreg & 0x40000) == 0x40000) inbyte = 1; else inbyte = 0;
                */
                shreg <<= 1;
                if (inbyte==1) shreg |= 1;
                int out1 = (int)((shreg ^ (shreg>>12)^(shreg>>17))&1);           
               
             // bitin(out1);
              //  if ((Usersetting.highSpeed == 2) || (Usersetting.highSpeed == 3))
                if ((baud == 4800) || (baud == 9600))
                {
                    
                    
                    bitinwithGC(out1);
                    bitin(out1);
                }
                else
                {
                    bitin(inbyte);
                    bitinwithGC(inbyte);
                }
            }


        }
        public void bitinwithGC(int bitin)
        {

        /*   if (CodeState == goldcodestate.HUNT)
            {

                if (getData_4bit((byte)bitin) == 2)
                {
                    alignbyte();
                }
                return;
            }*/
            if(getData1((byte)bitin) == 2){//0501vk3tbc
             
               
                /*  byte[] RxByte = new byte[1];
                 RxByte[0] = cbyte;
                String rxstring = ASCIIEncoding.ASCII.GetString(RxByte);
                ((Form1)reference).SetTextRTTY(rxstring);*/
                switch (CodeState)
                {
                  case goldcodestate.HUNT:
                        {
                          // alignbyte();
                             //return;



                            if (phase == 0)
                            {
                                bytestoCount = 256;
                                CodeState = goldcodestate.DETECTED;

                                cbyte = 0;
                                byteCnt = 0;
                            }
                                return;
                            
                        
                           
                        }
                  case goldcodestate.DETECTED:
                        {
                          
                            rxByte[byteCnt++] = cbyte;
                          
                            if (byteCnt == bytestoCount)
                            {
                                byte[] msgbuffer = new byte[256];
                                System.Array.Copy(rxByte, 0, msgbuffer, 0, 256);
                            
                            
                             
                                msgbuffer=sh.deshuffle(msgbuffer);
                              


                                MessageHandler.receivepicturequeue.Enqueue(msgbuffer);
                                byte[] RxByte = new byte[msgbuffer.Length];
                               
                                String rxstring = ASCIIEncoding.ASCII.GetString(msgbuffer);
                                ((Form1)reference).SetTextRTTY(rxstring);
                                System.Array.Clear(rxByte, 0, rxByte.Length);
                                byteCnt = 0;
                                CodeState = goldcodestate.HUNT;
                            }
                         //->   cbyte = 0;
                            return;
                        }
                  case goldcodestate.DETECTEDTELEMETRYFRAME:
                        {
                            rxByte[byteCnt++] = cbyte;
                           // System.Console.Write(" "+ byteCnt);
                            if (byteCnt == bytestoCount)
                            {
                                byte[] msgbuffer = new byte[256];
                                 if (phase == 0)
                                   System.Array.Copy(rxByte, 1, msgbuffer, 0, 256);
                                 else if (bytestoCount==256)
                                    System.Array.Copy(rxByte, 0, msgbuffer, 0, 256);
                                 else if (bytestoCount==258)
                                     System.Array.Copy(rxByte, 2, msgbuffer, 0, 256);
                              
                                // if(msgbuffer[0]==0x55)
                                //    MessageHandler.receivequeue.Enqueue(msgbuffer);
                               MessageHandler.receivequeueRSenc.Enqueue(msgbuffer);
                                System.Array.Clear(rxByte, 0, rxByte.Length);
                                byteCnt = 0;
                                for (int i = 0; i < 4; i++)
                                {
                                    System.Console.Write(msgbuffer[i]);
                                }
                                CodeState = goldcodestate.HUNT;
                                System.Console.WriteLine("HUNT STATE");
                            }
                         //->   cbyte = 0;
                            return;
                        }
                }
            }

        }
        public void detectPhase()
        {
            if (phase != 8)
            {
              
               // CodeState = goldcodestate.DETECTED;
              
                //System.Console.WriteLine("Found Test->" + phase);
               if (phase == 0)
                {

                    CodeState = goldcodestate.DETECTED;
                }
                else
                {
                    
                    bitcnt = phase;//Discard this to align
                    CodeState = goldcodestate.DETECTED;
                }            
            }
          
        }
        public void alignbyte()
        {
           // dbuffer = (byte) (dbuffer | (byte)(cbyte & 0xF0));
            Console.WriteLine(cbyte);
            // dbuffer = (byte)(dbuffer >> 4);
              // dbuffer  |=(byte)((cbyte<<4));
             
           phase=rxGcode.decodeGold((byte)(cbyte), 40);

           if (phase != 8)
           {
               bytestoCount = 256;
               byteCnt = 0;  // init buffer count
             
              // Console.WriteLine("Phase " + phase + " " + dbuffer); 

              
                
                    if ((phase > 0) && (phase < 5))
                    {
                        // rxByte[byteCnt++] = (byte)((dbuffer & 0x00FF));
                        // rxByte[byteCnt++] = (byte)((dbuffer & 0x00FF));
                        dbuffer = (byte)((dbuffer >> phase));
                        cbyte = (byte)((dbuffer >> 8) & 0x00FF);
                        cbyte = (byte)((cbyte << phase));
                        Console.WriteLine("Phase " + phase + " " + rxByte[0] + " " + cbyte + " Bit Cnt" + bitcnt);
                        bitcnt = phase;
                        cbyte = (byte)((dbuffer >> 8) & 0x00FF);
                        cbyte = (byte)((cbyte << phase));
                        Console.WriteLine("Phase " + phase + " " + rxByte[0] + " " + cbyte + " Bit Cnt" + bitcnt);
                        bitcnt = phase;
                    }
                    else
                    {
                        Console.WriteLine("DBuffer " + dbuffer);
                        dbuffer = (byte)((dbuffer >> phase));
                        Console.WriteLine("DBufferS " + dbuffer);
                        rxByte[byteCnt++] = (byte)((dbuffer & 0x00FF));

                        cbyte = (byte)((dbuffer >> 8) & 0x00FF);
                        cbyte = (byte)((cbyte << phase));
                        Console.WriteLine("Phase " + phase + " " + rxByte[0] + " " + cbyte + " Bit Cnt" + bitcnt);
                        bitcnt = phase;
                    }
                
                
               CodeState = goldcodestate.DETECTED;
           }
          // dbuffer = (byte)(dbuffer >> 4);
        //   cbyte = 0;
				
        }
        public void decode1(int inbyte)
        {
            if (state == State.IDLE)
            {
                switch (syncstate)
                {

                    case SyncState.DETECT:
                        {
                            if (inbyte == 1) syncstate = SyncState.WAITFORLOW; else syncstate = SyncState.WAITFORHIGH;
                            return;
                        }
                    case SyncState.WAITFORLOW:
                        {
                            if (inbyte == 0)
                            {
                                syncstate = SyncState.DECODING;
                                synccnt = syncmidbitcount / 2;
                            }
                            return;
                        }
                    case SyncState.WAITFORHIGH:
                        {
                            if (inbyte == 1)
                            {
                                syncstate = SyncState.DECODING;
                                synccnt = syncmidbitcount / 2;
                            }
                            return;
                        }
                    case SyncState.DECODING:
                        {
                            synccnt--;
                            if (synccnt == 0)
                            {
                                bitin(inbyte);
                                synccnt = syncmidbitcount;
                            }
                            return;
                        }

                }

            }
            else
            {
                syncstate = SyncState.DETECT;
                synccnt--;
                if (synccnt == 0)
                {
                    bitin(inbyte);
                    synccnt = syncmidbitcount;
                }
                return;
            }
        }
        private ushort CRC16(byte[] bytes)
        {
            /* This was a pain in the arse. Not documented at all! */
            /* This is what is used for AX25                      */
            ushort crc = 0xFFFF; //(ushort.maxvalue, 65535)

            for (int j = 0; j < byteCntax25-2; j++)
            {
                crc = (ushort)(crc ^ bytes[j]);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x0001) == 1)
                        crc = (ushort)((crc >> 1) ^ 0x8408);
                    else
                        crc >>= 1;
                }
            }
            return (ushort)~(uint)crc;    //A neat way to invert a byte.
        }
        

    }
}
