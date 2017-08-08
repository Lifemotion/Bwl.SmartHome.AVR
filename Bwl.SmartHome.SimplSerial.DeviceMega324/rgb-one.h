//FW for RGB One 1.0 (pcb: MultiSwitch v.2)

#define BAUD 9600
#define F_CPU 12000000UL

#include <avr/io.h>
#include <util/setbaud.h>
#include <util/delay.h>

#define DEVNAME "BwlSH.SS.RGB-1.0"
#define getbit(port, bit)		((port) &   (1 << (bit)))
#define setbit(port,bit,val)	{if ((val)) {(port)|= (1 << (bit));} else {(port) &= ~(1 << (bit));}}
#warning "Build for RGB-1.0"

byte get_buttons()
{
	byte result=0;
	DDRA=0;
	PORTA=255;
	if (getbit(PINA,0)==0) {result+=1;}
	if (getbit(PINA,1)==0) {result+=2;}
	if (getbit(PINA,2)==0) {result+=4;}
	if (getbit(PINA,3)==0) {result+=8;}
	if (getbit(PINA,4)==0) {result+=16;}
	if (getbit(PINA,5)==0) {result+=32;}
	if (getbit(PINA,6)==0) {result+=64;}
	if (getbit(PINA,7)==0) {result+=128;}
	return result;
}

void set_rgb1(byte r, byte g, byte b)
{
	setbit(DDRC,0,1);
	setbit(DDRC,1,1);
	setbit(DDRC,6,1);
	setbit (PORTC,0,r);
	setbit (PORTC,1,g);
	setbit (PORTC,6,b);
}

void sserial_send_start()
{
	DDRA|=(1<<7);PORTA|=(1<<7);
}

void sserial_send_end()
{
	DDRA|=(1<<7);PORTA&=(~(1<<7));
}

char r1,g1,b1;
char r2,g2,b2;

void sserial_process_request()
{
	if ((sserial_request.data[0]==52)&&(sserial_request.data[1]==135)&&(sserial_request.data[2]==17)&&(sserial_request.data[3]==245))
	{
		sserial_response.result=sserial_request.command+1;
		sserial_response.data[0]=80;
		sserial_response.data[1]=36;
		sserial_response.data[2]=2;
		sserial_response.data[3]=224;
		//set rgbs
		if (sserial_request.command==1)
		{
			r1=sserial_request.data[4];
			g1=sserial_request.data[5];
			b1=sserial_request.data[6];
			r2=sserial_request.data[7];
			g2=sserial_request.data[8];
			b2=sserial_request.data[9];
			sserial_response.datalength=4;
			sserial_send_response();
		}
		//get rgbs
		if (sserial_request.command==2)
		{
			sserial_response.data[4]=r1;
			sserial_response.data[5]=g1;
			sserial_response.data[6]=b1;
			sserial_response.data[7]=r2;
			sserial_response.data[8]=g2;
			sserial_response.data[9]=b2;
			sserial_response.data[10]=get_buttons();
			sserial_response.datalength=12;
			sserial_send_response();		
		}		
	}
}


byte counter=0;

void device_work()
{
	if (counter==0)
	{
		set_rgb1(0, 0, 0);
		byte b=get_buttons();

		if (b&1){r1=255;}
		if (b&2){g1=255;}
		if (b&4){b1=255;}
		if (b&8){r1=0; g1=0; b1=0;}
		set_rgb1(r1>0, g1>0, b1>0);
	}
	set_rgb1(r1>counter, g1>counter, b1>counter);
	counter++;
};