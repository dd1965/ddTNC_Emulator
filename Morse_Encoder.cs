using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TNCAX25Emulator
{
    class Morse_Encoder:IDisposable
    {
        /* A DOT = 1200/W where W is words per minute.
         * A DASH = 3 DOTS
         * Interword = 7 DOTS
         * Inter symbol = 1 DOT
         * Inter character = 3 DOTS
         * A space is a the seperator for a WORD
         */
        private static int W = 25;
        private static float DOT = 1200 / W;
        private static float DASH = 3 * DOT;
        private static float InterSymbol = DOT;
        private static float InterCharacter = 3 * DOT;
        private static float InterWord = 3 * DOT;
        private Dictionary<string, Object> morseMap = new Dictionary<string, Object>();
        private static Morse_Encoder instance;
        public Morse_Encoder()
        {
            /* SET UP Morse alphabet. Tedious, but required... Could do this by making a routine were you 
             * just specify what the number of DOTS and DAHs are needed as strings, will do to reduce code size... TODO
             */
            // A = DIT DAH
            morseCharacter ISO_A_Morse = new morseCharacter(4);
            Morse_Encoder.waveformmap ISO_Abit0 = new  Morse_Encoder.waveformmap();
            ISO_Abit0.toneHz = 1000;
            ISO_Abit0.timingms = DOT;
            ISO_A_Morse.characterbits[0] = ISO_Abit0;
            ISO_A_Morse.characterbits[1] = intersymbol();
            Morse_Encoder.waveformmap ISO_Abit1 = new Morse_Encoder.waveformmap();
            ISO_Abit1.toneHz = 1000;
            ISO_Abit1.timingms = DASH;
            ISO_A_Morse.characterbits[2] = ISO_Abit1;
            ISO_A_Morse.characterbits[3] = interCharacter();
            morseMap.Add("A", ISO_A_Morse);
           // End A

           // B = DAH DIT DIT DIT 
            morseCharacter ISO_B_Morse = new morseCharacter(8);
            ISO_B_Morse.characterbits[0] = dash();
            ISO_B_Morse.characterbits[1] = intersymbol();
            ISO_B_Morse.characterbits[2] = dot();
            ISO_B_Morse.characterbits[3] = intersymbol();
            ISO_B_Morse.characterbits[4] = dot();
            ISO_B_Morse.characterbits[5] = intersymbol();
            ISO_B_Morse.characterbits[6] = dot();
            ISO_B_Morse.characterbits[7] = interCharacter();           
            morseMap.Add("B", ISO_B_Morse);

            // C = DAH DIT DAH DIT 
            morseCharacter ISO_C_Morse = new morseCharacter(8);
            ISO_C_Morse.characterbits[0] = dash();
            ISO_C_Morse.characterbits[1] = intersymbol();
            ISO_C_Morse.characterbits[2] = dot();
            ISO_C_Morse.characterbits[3] = intersymbol();
            ISO_C_Morse.characterbits[4] = dash();
            ISO_C_Morse.characterbits[5] = intersymbol();
            ISO_C_Morse.characterbits[6] = dot();
            ISO_C_Morse.characterbits[7] = interCharacter();
            morseMap.Add("C", ISO_C_Morse);

            // D = DAH DIT DIT 
            morseCharacter ISO_D_Morse = new morseCharacter(6);
            ISO_D_Morse.characterbits[0] = dash();
            ISO_D_Morse.characterbits[1] = intersymbol();
            ISO_D_Morse.characterbits[2] = dot();
            ISO_D_Morse.characterbits[3] = intersymbol();
            ISO_D_Morse.characterbits[4] = dot();
            ISO_D_Morse.characterbits[5] = interCharacter();
            morseMap.Add("D", ISO_D_Morse);

            // E =DIT 
            morseCharacter ISO_E_Morse = new morseCharacter(2);
            ISO_E_Morse.characterbits[0] = dot();
            ISO_E_Morse.characterbits[1] = interCharacter();
            morseMap.Add("E", ISO_E_Morse);


            // F = DAH DAH DIT DAH 
            morseCharacter ISO_F_Morse = new morseCharacter(8);
            ISO_F_Morse.characterbits[0] = dash();
            ISO_F_Morse.characterbits[1] = intersymbol();
            ISO_F_Morse.characterbits[2] = dash();
            ISO_F_Morse.characterbits[3] = intersymbol();
            ISO_F_Morse.characterbits[4] = dot();
            ISO_F_Morse.characterbits[5] = intersymbol();
            ISO_F_Morse.characterbits[6] = dash();
            ISO_F_Morse.characterbits[7] = interCharacter();
            morseMap.Add("F", ISO_F_Morse);


            // G = DAH DAH DIT
            morseCharacter ISO_G_Morse = new morseCharacter(6);
            ISO_G_Morse.characterbits[0] = dash();
            ISO_G_Morse.characterbits[1] = intersymbol();
            ISO_G_Morse.characterbits[2] = dot();
            ISO_G_Morse.characterbits[3] = intersymbol();
            ISO_G_Morse.characterbits[4] = dot();
            ISO_G_Morse.characterbits[5] = interCharacter();
            morseMap.Add("G", ISO_G_Morse);

            // H = DIT DIT DIT DIT 
            morseCharacter ISO_H_Morse = new morseCharacter(8);
            ISO_H_Morse.characterbits[0] = dash();
            ISO_H_Morse.characterbits[1] = intersymbol();
            ISO_H_Morse.characterbits[2] = dash();
            ISO_H_Morse.characterbits[3] = intersymbol();
            ISO_H_Morse.characterbits[4] = dot();
            ISO_H_Morse.characterbits[5] = intersymbol();
            ISO_H_Morse.characterbits[6] = dash();
            ISO_H_Morse.characterbits[7] = interCharacter();
            morseMap.Add("H", ISO_H_Morse);


            // I = DIT DIT 
            morseCharacter ISO_I_Morse = new morseCharacter(4);
            ISO_I_Morse.characterbits[0] = dot();
            ISO_I_Morse.characterbits[1] = intersymbol();
            ISO_I_Morse.characterbits[2] = dot();
            ISO_I_Morse.characterbits[3] = interCharacter();
            morseMap.Add("I", ISO_I_Morse);

            // J = DIT DAH DAH DAH
            morseCharacter ISO_J_Morse = new morseCharacter(8);
            ISO_J_Morse.characterbits[0] = dot();
            ISO_J_Morse.characterbits[1] = intersymbol();
            ISO_J_Morse.characterbits[2] = dash();
            ISO_J_Morse.characterbits[3] = intersymbol();
            ISO_J_Morse.characterbits[4] = dash();
            ISO_J_Morse.characterbits[5] = intersymbol();
            ISO_J_Morse.characterbits[6] = dash();
            ISO_J_Morse.characterbits[7] = interCharacter();
            morseMap.Add("J", ISO_J_Morse);

            // K = DAH DIT DAH
            morseCharacter ISO_K_Morse = new morseCharacter(6);
            ISO_K_Morse.characterbits[0] = dash();
            ISO_K_Morse.characterbits[1] = intersymbol();
            ISO_K_Morse.characterbits[2] = dot();
            ISO_K_Morse.characterbits[3] = intersymbol();
            ISO_K_Morse.characterbits[4] = dash();
            ISO_K_Morse.characterbits[5] = interCharacter();
            morseMap.Add("K", ISO_K_Morse);

          

            // L = DIT DAH DIT DIT
            morseCharacter ISO_L_Morse = new morseCharacter(8);
            ISO_L_Morse.characterbits[0] = dot();
            ISO_L_Morse.characterbits[1] = intersymbol();
            ISO_L_Morse.characterbits[2] = dash();
            ISO_L_Morse.characterbits[3] = intersymbol();
            ISO_L_Morse.characterbits[4] = dot();
            ISO_L_Morse.characterbits[5] = intersymbol();
            ISO_L_Morse.characterbits[6] = dot();
            ISO_L_Morse.characterbits[7] = interCharacter();
            morseMap.Add("L", ISO_L_Morse);

            // M = DAH DAH
            morseCharacter ISO_M_Morse = new morseCharacter(4);
            ISO_M_Morse.characterbits[0] = dash();
            ISO_M_Morse.characterbits[1] = intersymbol();
            ISO_M_Morse.characterbits[2] = dash();
            ISO_M_Morse.characterbits[3] = interCharacter();
            morseMap.Add("M", ISO_M_Morse);

            // N = DAH DIT
            morseCharacter ISO_N_Morse = new morseCharacter(4);
            ISO_N_Morse.characterbits[0] = dash();
            ISO_N_Morse.characterbits[1] = intersymbol();
            ISO_N_Morse.characterbits[2] = dot();
            ISO_N_Morse.characterbits[3] = interCharacter();
            morseMap.Add("N", ISO_N_Morse);


            // O = DAH DAH DAH
            morseCharacter ISO_O_Morse = new morseCharacter(6);
            ISO_O_Morse.characterbits[0] = dash();
            ISO_O_Morse.characterbits[1] = intersymbol();
            ISO_O_Morse.characterbits[2] = dash();
            ISO_O_Morse.characterbits[3] = intersymbol();
            ISO_O_Morse.characterbits[4] = dash();
            ISO_O_Morse.characterbits[5] = interCharacter();
            morseMap.Add("O", ISO_O_Morse);

            // P = DIT DAH DAH DIT
            morseCharacter ISO_P_Morse = new morseCharacter(8);
            ISO_P_Morse.characterbits[0] = dot();
            ISO_P_Morse.characterbits[1] = intersymbol();
            ISO_P_Morse.characterbits[2] = dash();
            ISO_P_Morse.characterbits[3] = intersymbol();
            ISO_P_Morse.characterbits[4] = dash();
            ISO_P_Morse.characterbits[5] = intersymbol();
            ISO_P_Morse.characterbits[6] = dot();
            ISO_P_Morse.characterbits[7] = interCharacter();
            morseMap.Add("P", ISO_P_Morse);

            // Q =  DAH DAH DIT DAH
            morseCharacter ISO_Q_Morse = new morseCharacter(8);
            ISO_Q_Morse.characterbits[0] = dash();
            ISO_Q_Morse.characterbits[1] = intersymbol();
            ISO_Q_Morse.characterbits[2] = dash();
            ISO_Q_Morse.characterbits[3] = intersymbol();
            ISO_Q_Morse.characterbits[4] = dot();
            ISO_Q_Morse.characterbits[5] = intersymbol();
            ISO_Q_Morse.characterbits[6] = dash();
            ISO_Q_Morse.characterbits[7] = interCharacter();
            morseMap.Add("Q", ISO_Q_Morse);

            // R = DIT DAH DIT
            morseCharacter ISO_R_Morse = new morseCharacter(6);
            ISO_R_Morse.characterbits[0] = dot();
            ISO_R_Morse.characterbits[1] = intersymbol();
            ISO_R_Morse.characterbits[2] = dash();
            ISO_R_Morse.characterbits[3] = intersymbol();
            ISO_R_Morse.characterbits[4] = dot();
            ISO_R_Morse.characterbits[5] = interCharacter();
            morseMap.Add("R", ISO_R_Morse);

            // S = DIT DIT DIT
            morseCharacter ISO_S_Morse = new morseCharacter(6);
            ISO_S_Morse.characterbits[0] = dot();
            ISO_S_Morse.characterbits[1] = intersymbol();
            ISO_S_Morse.characterbits[2] = dot();
            ISO_S_Morse.characterbits[3] = intersymbol();
            ISO_S_Morse.characterbits[4] = dot();
            ISO_S_Morse.characterbits[5] = interCharacter();
            morseMap.Add("S", ISO_S_Morse);

            // T =DIT 
            morseCharacter ISO_T_Morse = new morseCharacter(2);
            ISO_T_Morse.characterbits[0] = dash();
            ISO_T_Morse.characterbits[1] = interCharacter();
            morseMap.Add("T", ISO_T_Morse);

            // U = DIT DIT DASH
            morseCharacter ISO_U_Morse = new morseCharacter(6);
            ISO_U_Morse.characterbits[0] = dot();
            ISO_U_Morse.characterbits[1] = intersymbol();
            ISO_U_Morse.characterbits[2] = dot();
            ISO_U_Morse.characterbits[3] = intersymbol();
            ISO_U_Morse.characterbits[4] = dash();
            ISO_U_Morse.characterbits[5] = interCharacter();
            morseMap.Add("U", ISO_U_Morse);

            // V =  DIT DIT DIT DAH
            morseCharacter ISO_V_Morse = new morseCharacter(8);
            ISO_V_Morse.characterbits[0] = dot();
            ISO_V_Morse.characterbits[1] = intersymbol();
            ISO_V_Morse.characterbits[2] = dot();
            ISO_V_Morse.characterbits[3] = intersymbol();
            ISO_V_Morse.characterbits[4] = dot();
            ISO_V_Morse.characterbits[5] = intersymbol();
            ISO_V_Morse.characterbits[6] = dash();
            ISO_V_Morse.characterbits[7] = interCharacter();
            morseMap.Add("V", ISO_V_Morse);

            // W = DIT DASH DASH
            morseCharacter ISO_W_Morse = new morseCharacter(6);
            ISO_W_Morse.characterbits[0] = dot();
            ISO_W_Morse.characterbits[1] = intersymbol();
            ISO_W_Morse.characterbits[2] = dash();
            ISO_W_Morse.characterbits[3] = intersymbol();
            ISO_W_Morse.characterbits[4] = dash();
            ISO_W_Morse.characterbits[5] = interCharacter();
            morseMap.Add("W", ISO_W_Morse);


            // X =  DAH DIT DIT DAH
            morseCharacter ISO_X_Morse = new morseCharacter(8);
            ISO_X_Morse.characterbits[0] = dash();
            ISO_X_Morse.characterbits[1] = intersymbol();
            ISO_X_Morse.characterbits[2] = dot();
            ISO_X_Morse.characterbits[3] = intersymbol();
            ISO_X_Morse.characterbits[4] = dot();
            ISO_X_Morse.characterbits[5] = intersymbol();
            ISO_X_Morse.characterbits[6] = dash();
            ISO_X_Morse.characterbits[7] = interCharacter();
            morseMap.Add("X", ISO_X_Morse);


            // Y =  DAH DIT DAH DAH
            morseCharacter ISO_Y_Morse = new morseCharacter(8);
            ISO_Y_Morse.characterbits[0] = dash();
            ISO_Y_Morse.characterbits[1] = intersymbol();
            ISO_Y_Morse.characterbits[2] = dot();
            ISO_Y_Morse.characterbits[3] = intersymbol();
            ISO_Y_Morse.characterbits[4] = dash();
            ISO_Y_Morse.characterbits[5] = intersymbol();
            ISO_Y_Morse.characterbits[6] = dash();
            ISO_Y_Morse.characterbits[7] = interCharacter();
            morseMap.Add("Y", ISO_Y_Morse);

            // Z =  DAH DAH DIT DIT
            morseCharacter ISO_Z_Morse = new morseCharacter(8);
            ISO_Z_Morse.characterbits[0] = dash();
            ISO_Z_Morse.characterbits[1] = intersymbol();
            ISO_Z_Morse.characterbits[2] = dash();
            ISO_Z_Morse.characterbits[3] = intersymbol();
            ISO_Z_Morse.characterbits[4] = dot();
            ISO_Z_Morse.characterbits[5] = intersymbol();
            ISO_Z_Morse.characterbits[6] = dot();
            ISO_Z_Morse.characterbits[7] = interCharacter();
            morseMap.Add("Z", ISO_Z_Morse);


            // 0 =  DAH DAH DAH DAH DAH
            morseCharacter ISO_Zero_Morse = new morseCharacter(10);
            ISO_Zero_Morse.characterbits[0] = dash();
            ISO_Zero_Morse.characterbits[1] = intersymbol();
            ISO_Zero_Morse.characterbits[2] = dash();
            ISO_Zero_Morse.characterbits[3] = intersymbol();
            ISO_Zero_Morse.characterbits[4] = dash();
            ISO_Zero_Morse.characterbits[5] = intersymbol();
            ISO_Zero_Morse.characterbits[6] = dash();
            ISO_Zero_Morse.characterbits[7] = intersymbol();
            ISO_Zero_Morse.characterbits[8] = dash();
            ISO_Zero_Morse.characterbits[9] = interCharacter();
            morseMap.Add("0", ISO_Zero_Morse);

            // 1 =  DIT DAH DAH DAH DAH
            morseCharacter ISO_One_Morse = new morseCharacter(10);
            ISO_One_Morse.characterbits[0] = dot();
            ISO_One_Morse.characterbits[1] = intersymbol();
            ISO_One_Morse.characterbits[2] = dash();
            ISO_One_Morse.characterbits[3] = intersymbol();
            ISO_One_Morse.characterbits[4] = dash();
            ISO_One_Morse.characterbits[5] = intersymbol();
            ISO_One_Morse.characterbits[6] = dash();
            ISO_One_Morse.characterbits[7] = intersymbol();
            ISO_One_Morse.characterbits[8] = dash();
            ISO_One_Morse.characterbits[9] = interCharacter();
            morseMap.Add("1", ISO_One_Morse);

            // 2 =  DIT DIT DAH DAH DAH
            morseCharacter ISO_Two_Morse = new morseCharacter(10);
            ISO_Two_Morse.characterbits[0] = dot();
            ISO_Two_Morse.characterbits[1] = intersymbol();
            ISO_Two_Morse.characterbits[2] = dot();
            ISO_Two_Morse.characterbits[3] = intersymbol();
            ISO_Two_Morse.characterbits[4] = dash();
            ISO_Two_Morse.characterbits[5] = intersymbol();
            ISO_Two_Morse.characterbits[6] = dash();
            ISO_Two_Morse.characterbits[7] = intersymbol();
            ISO_Two_Morse.characterbits[8] = dash();
            ISO_Two_Morse.characterbits[9] = interCharacter();
            morseMap.Add("2", ISO_Two_Morse);

            // 3 =  DIT DIT DIT DAH DAH
            morseCharacter ISO_Three_Morse = new morseCharacter(10);
           ISO_Three_Morse.characterbits[0] = dot();
           ISO_Three_Morse.characterbits[1] = intersymbol();
           ISO_Three_Morse.characterbits[2] = dot();
           ISO_Three_Morse.characterbits[3] = intersymbol();
           ISO_Three_Morse.characterbits[4] = dot();
           ISO_Three_Morse.characterbits[5] = intersymbol();
           ISO_Three_Morse.characterbits[6] = dash();
           ISO_Three_Morse.characterbits[7] = intersymbol();
           ISO_Three_Morse.characterbits[8] = dash();
           ISO_Three_Morse.characterbits[9] = interCharacter();
           morseMap.Add("3",ISO_Three_Morse);

           // 4 =  DIT DIT DIT DIT DAH
           morseCharacter ISO_Four_Morse = new morseCharacter(10);
           ISO_Four_Morse.characterbits[0] = dot();
           ISO_Four_Morse.characterbits[1] = intersymbol();
           ISO_Four_Morse.characterbits[2] = dot();
           ISO_Four_Morse.characterbits[3] = intersymbol();
           ISO_Four_Morse.characterbits[4] = dot();
           ISO_Four_Morse.characterbits[5] = intersymbol();
           ISO_Four_Morse.characterbits[6] = dot();
           ISO_Four_Morse.characterbits[7] = intersymbol();
           ISO_Four_Morse.characterbits[8] = dash();
           ISO_Four_Morse.characterbits[9] = interCharacter();
           morseMap.Add("4", ISO_Four_Morse);

           // 5 =  DIT DIT DIT DIT DIT
           morseCharacter ISO_Five_Morse = new morseCharacter(10);
           ISO_Five_Morse.characterbits[0] = dot();
           ISO_Five_Morse.characterbits[1] = intersymbol();
           ISO_Five_Morse.characterbits[2] = dot();
           ISO_Five_Morse.characterbits[3] = intersymbol();
           ISO_Five_Morse.characterbits[4] = dot();
           ISO_Five_Morse.characterbits[5] = intersymbol();
           ISO_Five_Morse.characterbits[6] = dot();
           ISO_Five_Morse.characterbits[7] = intersymbol();
           ISO_Five_Morse.characterbits[8] = dot();
           ISO_Five_Morse.characterbits[9] = interCharacter();
           morseMap.Add("5", ISO_Five_Morse);

           // 6 =  DAH DIT DIT DIT DIT
           morseCharacter ISO_Six_Morse = new morseCharacter(10);
           ISO_Six_Morse.characterbits[0] = dash();
           ISO_Six_Morse.characterbits[1] = intersymbol();
           ISO_Six_Morse.characterbits[2] = dot();
           ISO_Six_Morse.characterbits[3] = intersymbol();
           ISO_Six_Morse.characterbits[4] = dot();
           ISO_Six_Morse.characterbits[5] = intersymbol();
           ISO_Six_Morse.characterbits[6] = dot();
           ISO_Six_Morse.characterbits[7] = intersymbol();
           ISO_Six_Morse.characterbits[8] = dot();
           ISO_Six_Morse.characterbits[9] = interCharacter();
           morseMap.Add("6", ISO_Six_Morse);

           // 7 =  DAH DAH DIT DIT DIT
           morseCharacter ISO_Seven_Morse = new morseCharacter(10);
           ISO_Seven_Morse.characterbits[0] = dash();
           ISO_Seven_Morse.characterbits[1] = intersymbol();
           ISO_Seven_Morse.characterbits[2] = dash();
           ISO_Seven_Morse.characterbits[3] = intersymbol();
           ISO_Seven_Morse.characterbits[4] = dot();
           ISO_Seven_Morse.characterbits[5] = intersymbol();
           ISO_Seven_Morse.characterbits[6] = dot();
           ISO_Seven_Morse.characterbits[7] = intersymbol();
           ISO_Seven_Morse.characterbits[8] = dot();
           ISO_Seven_Morse.characterbits[9] = interCharacter();
           morseMap.Add("7", ISO_Seven_Morse);

           // 8 =  DAH DAH DAH DIT DIT
           morseCharacter ISO_Eight_Morse = new morseCharacter(10);
           ISO_Eight_Morse.characterbits[0] = dash();
           ISO_Eight_Morse.characterbits[1] = intersymbol();
           ISO_Eight_Morse.characterbits[2] = dash();
           ISO_Eight_Morse.characterbits[3] = intersymbol();
           ISO_Eight_Morse.characterbits[4] = dash();
           ISO_Eight_Morse.characterbits[5] = intersymbol();
           ISO_Eight_Morse.characterbits[6] = dot();
           ISO_Eight_Morse.characterbits[7] = intersymbol();
           ISO_Eight_Morse.characterbits[8] = dot();
           ISO_Eight_Morse.characterbits[9] = interCharacter();
           morseMap.Add("8", ISO_Eight_Morse);
           
            // 9 =  DAH DAH DAH DIT DIT
           morseCharacter ISO_Nine_Morse = new morseCharacter(10);
           ISO_Nine_Morse.characterbits[0] = dash();
           ISO_Nine_Morse.characterbits[1] = intersymbol();
           ISO_Nine_Morse.characterbits[2] = dash();
           ISO_Nine_Morse.characterbits[3] = intersymbol();
           ISO_Nine_Morse.characterbits[4] = dash();
           ISO_Nine_Morse.characterbits[5] = intersymbol();
           ISO_Nine_Morse.characterbits[6] = dash();
           ISO_Nine_Morse.characterbits[7] = intersymbol();
           ISO_Nine_Morse.characterbits[8] = dot();
           ISO_Nine_Morse.characterbits[9] = interCharacter();
           morseMap.Add("9", ISO_Nine_Morse);




        }
        private Morse_Encoder.waveformmap intersymbol()
        {
            Morse_Encoder.waveformmap intersymbolquiet = new Morse_Encoder.waveformmap();
            intersymbolquiet.toneHz = 0;
            intersymbolquiet.timingms = DOT;
            return intersymbolquiet;
        }
        private Morse_Encoder.waveformmap interCharacter()
        {
            Morse_Encoder.waveformmap intercharquiet = new Morse_Encoder.waveformmap();
            intercharquiet.toneHz = 0;
            intercharquiet.timingms = InterCharacter;
            return intercharquiet;
        }




        private Morse_Encoder.waveformmap interword()
        {
            Morse_Encoder.waveformmap interwordquiet = new Morse_Encoder.waveformmap();
            interwordquiet.toneHz = 0;
            interwordquiet.timingms = InterWord;
            return interwordquiet;
        }
        private Morse_Encoder.waveformmap dot()
        {
            Morse_Encoder.waveformmap dotsymbol = new Morse_Encoder.waveformmap();
            dotsymbol.toneHz = 1000;
            dotsymbol.timingms = DOT;
            return dotsymbol;
        }
        private Morse_Encoder.waveformmap dash()
        {
            Morse_Encoder.waveformmap dashsymbol = new Morse_Encoder.waveformmap();
            dashsymbol.toneHz = 1000;
            dashsymbol.timingms = DASH;
            return dashsymbol;
        }
        public Morse_Encoder.waveformmap[] getMorseChar(String str)
        {

            Object MorseObj;
            if (morseMap.TryGetValue(str, out MorseObj))
            {
                return ((morseCharacter)MorseObj).characterbits;
            }else

            return null;
        }
        public Morse_Encoder.waveformmap[] getCWtransmitsequence(String str)
        {
            List<Morse_Encoder.waveformmap> totalSequence = new List<Morse_Encoder.waveformmap>();          
            char character;
            Morse_Encoder.waveformmap[] sequence; //= new Morse_Encoder.waveformmap[1]
            Morse_Encoder.waveformmap[] space = new Morse_Encoder.waveformmap[1];
            str = str.ToUpper();
            for(int i=0;i < str.Length;i++){

                character=str.ElementAt(i);
                
               // if (String.IsNullOrWhiteSpace(character.ToString()))
                if (character.ToString() == " ")
                {

                    space[0] = interword();
                    totalSequence.AddRange(space);
                }
                else
                {
                    sequence = getMorseChar(character.ToString());
                    if(sequence != null)
                    totalSequence.AddRange(sequence);
                }
            }

            return totalSequence.ToArray();
        }

        public void Dispose(){
            morseMap.Clear();
            instance = null;

        }
        public static Morse_Encoder Instance
       {
           get
           {
               if (instance == null)
               {
                   instance = new Morse_Encoder();
               }
               return instance;
           }
       }
    
        class morseCharacter
        {
           public  morseCharacter(int size)
            {
                characterbits = new Morse_Encoder.waveformmap[size];
            }
            public Morse_Encoder.waveformmap[] characterbits;
        }

        public class waveformmap
        {
            public float timingms { get; set; }
            public int toneHz { get; set; }
           

        }


    }
}
