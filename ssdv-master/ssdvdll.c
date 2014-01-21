

/* SSDV - Slow Scan Digital Video                                        */
/*=======================================================================*/
/* Copyright 2011 Philip Heron <phil@sanslogic.co.uk                     */
/*                                                                       */
/* This program is free software: you can redistribute it and/or modify  */
/* it under the terms of the GNU General Public License as published by  */
/* the Free Software Foundation, either version 3 of the License, or     */
/* (at your option) any later version.                                   */
/*                                                                       */
/* This program is distributed in the hope that it will be useful,       */
/* but WITHOUT ANY WARRANTY; without even the implied warranty of        */
/* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         */
/* GNU General Public License for more details.                          */
/*                                                                       */
/* You should have received a copy of the GNU General Public License     */
/* along with this program.  If not, see <http://www.gnu.org/licenses/>. */
/* Modified by David Dessardo VK3TBC 1/09/2013  to work as a Windows DLL */ 
/* so that it can be called up by C# It works but is WIP and needs a     */
/* little refactoring of the API calls                                   */


#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
//#include <unistd.h>
#include <string.h>
#include "ssdv.h"
#include "rs8.h"
void exit_usage()
{
	fprintf(stderr, "Usage: ssdv [-e|-d] [-t <percentage>] [-c <callsign>] [-i <id>] [<in file>] [<out file>]\n");
	exit(-1);
}
extern __declspec(dllexport) void initialise(){
	 
	
	 hptr = malloc(sizeof(header));
     ssdv_dec_init(&ssdv);		
     jpeg_length = 1024 * 64;
     jpeg = malloc(jpeg_length);		
	 ssdv_dec_set_buffer(&ssdv, jpeg, jpeg_length);
	

}
extern __declspec(dllexport) int output(char *fileout){
	   
	    FILE *fout = stdout;
	    int x;
	    fout = fopen(fileout, "wb");
	
			
	      x =  ssdv_dec_get_jpeg(&ssdv, &jpeg, &jpeg_length);		
		 fwrite(jpeg, 1, jpeg_length, fout);				   	 
	    if(fout != stdout) fclose(fout);
		return x;
}
/*Return the decoded SSDV buffer*/
/*Must call intialise first, followed by a call to decodeImage and then FREEMEM*/
extern __declspec(dllexport) uint8_t* output_buffer(int* x){
	  
	
	 ssdv_dec_get_jpeg(&ssdv, &jpeg, &jpeg_length);
	  *x = jpeg_length;
	return jpeg;
}


extern __declspec(dllexport) header* getFrameHeader()
{
	
	/*char callsign[7];
	callsign[0]= hptr->lostpacket[0];
	//callsign[0]=&ssdv.lostpacket[0];
	callsign[1]=hptr->lostpacket[1];
	callsign[2]=hptr->lostpacket[2];;*/
	return hptr;
}
extern __declspec(dllexport) void freeMem(){
	free(hptr);
	//free(&ssdv);
	free(jpeg);
}


extern __declspec(dllexport) int decodeImageData(uint8_t *decpkt )

{
	char code;
	uint8_t pkt[SSDV_PKT_SIZE], b[128];				
	/* Decode */				
		
		//Starts here
	
		code= ssdv_dec_is_packet(decpkt, NULL);
		if(code==0){
	        code= ssdv_dec_feed(&ssdv, decpkt);
		}
	    return(code);
		
}
/*Encode the telemetry data with Reed Solomon error correction*/
extern __declspec(dllexport) uint8_t* encodeTelemetry(uint8_t *encpkt )
{
	uint8_t pkt[SSDV_PKT_SIZE];

	encode_rs_8(&encpkt[0], &encpkt[223], 0);
	return encpkt;

}

extern __declspec(dllexport) int decodeTelemetry(uint8_t *decpkt )
{
	uint8_t pkt[SSDV_PKT_SIZE];
	int i;
	
	/* Testing is destructive, work on a copy */
	memcpy(pkt,decpkt, SSDV_PKT_SIZE);
	i = decode_rs_8(&pkt[0], 0, 0, 0);
	memcpy(decpkt, pkt, SSDV_PKT_SIZE);
	if(i < 0) return(-1); 
	return i ;
}
extern __declspec(dllexport) int decodeSSDV(uint8_t *decpkt )
{
	/*Added so that we can easily do logging for SSDV pictures*/
	uint8_t pkt[SSDV_PKT_SIZE];
	int i;
	
	/* Testing is destructive, work on a copy */
	memcpy(pkt,decpkt, SSDV_PKT_SIZE);
	i = decode_rs_8(&pkt[1], 0, 0, 0);
	memcpy(decpkt, pkt, SSDV_PKT_SIZE);
	if(i < 0) return(-1); 
	return i ;
}

extern __declspec(dllexport) int encodeImage(char imgid,char Callsign[],char *filein,char *fileout)		
{
		int c, i;
	    char callsign[7];
		uint8_t pkt[SSDV_PKT_SIZE], b[128];	
	  
		uint8_t image_id = 0;
		
		FILE *fin = stdin;
	    FILE *fout = stdout;
		
		strncpy(callsign, Callsign, 6);
		image_id=imgid;
	    fin = fopen(filein, "rb");
	    fout = fopen(fileout, "wb");


	   /* Encode */

		ssdv_enc_init(&ssdv, callsign, image_id);
		ssdv_enc_set_buffer(&ssdv, pkt);
		
		i = 0;
		
		while(1)
		{
			while((c = ssdv_enc_get_packet(&ssdv)) == SSDV_FEED_ME)
			{
			  size_t r = fread(b, 1, 128, fin);
				
				if(r <= 0)
				{
					fprintf(stderr, "Premature end of file\n");
					break;
				}
			    ssdv_enc_feed(&ssdv, b, r);
			}
			
			if(c == SSDV_EOI)
			{
				fprintf(stderr, "ssdv_enc_get_packet said EOI\n");
				break;
			}
			else if(c != SSDV_OK)
			{
				fprintf(stderr, "ssdv_enc_get_packet failed: %i\n", c);
				return(-1);
			}
			
			fwrite(pkt, 1, SSDV_PKT_SIZE, fout);
			i++;
		}
		
		fprintf(stderr, "Wrote %i packets\n", i);
		
		
	if(fin != stdin) fclose(fin);
	if(fout != stdout) fclose(fout);
	
	return(0);
}

