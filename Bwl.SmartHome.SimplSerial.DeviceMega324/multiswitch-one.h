//FW for MultiSwitch One 1.0 (pcb: MultiSwitch v.1)

#define BAUD 9600
#define F_CPU 8000000UL

#include <avr/io.h>
#include <util/setbaud.h>
#include <util/delay.h>

#define DEVNAME "BwlSH.SS.MultiSwitch-1.0"
#define getbit(port, bit)		((port) &   (1 << (bit)))
#define setbit(port,bit,val)	{if ((val)) {(port)|= (1 << (bit));} else {(port) &= ~(1 << (bit));}}
#warning "Build for MultiSwitch-1.0"


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

byte get_relays_1()
{
	byte result=0;
	if (getbit(PINB,0)) {result+=1;}
	if (getbit(PINB,1)) {result+=2;}
	if (getbit(PINB,2)) {result+=4;}
	if (getbit(PINB,3)) {result+=8;}
	if (getbit(PINB,4)) {result+=16;}
	if (getbit(PINB,5)) {result+=32;}
	if (getbit(PINB,6)) {result+=64;}
	if (getbit(PINB,7)) {result+=128;}
	return result;
}

byte get_relays_2()
{
	byte result=0;
	if (getbit(PIND,4)) {result+=1;}
	if (getbit(PIND,5)) {result+=2;}
	if (getbit(PIND,6)) {result+=4;}
	if (getbit(PIND,7)) {result+=8;}
	return result;
}

void set_relays_1(byte states)
{
	DDRB=255;
	setbit (PORTB,0,(states&1));
	setbit (PORTB,1,(states&2));
	setbit (PORTB,2,(states&4));
	setbit (PORTB,3,(states&8));
	setbit (PORTB,4,(states&16));
	setbit (PORTB,5,(states&32));
	setbit (PORTB,6,(states&64));
	setbit (PORTB,7,(states&128));
}

void set_relays_2(byte states)
{
	setbit(DDRD,4,1);
	setbit(DDRD,5,1);
	setbit(DDRD,6,1);
	setbit(DDRD,7,1);
	setbit (PORTD,4,(states&1));
	setbit (PORTD,5,(states&2));
	setbit (PORTD,6,(states&4));
	setbit (PORTD,7,(states&8));
}

void sserial_send_start()
{
	DDRD|=(1<<2);PORTD|=(1<<2);
}

void sserial_send_end()
{
	DDRD|=(1<<2);PORTD&=(~(1<<2));
}

byte input_mode_table=0;
byte input_output_table_1[8]={1+2,4+8,16+32,64+128,0,0,0,0};
byte input_output_table_2[8]={0,0,0,0,1+2,4+8,0,0};

void sserial_process_request()
{
	if ((sserial_request.data[0]==57)&&(sserial_request.data[1]==129)&&(sserial_request.data[2]==33)&&(sserial_request.data[3]==221))
	{
		sserial_response.result=sserial_request.command+1;
		sserial_response.data[0]=78;
		sserial_response.data[1]=32;
		sserial_response.data[2]=1;
		sserial_response.data[3]=227;
		if (sserial_request.command==1)
		{
			sserial_response.data[4]=get_relays_1();
			sserial_response.data[5]=get_relays_2();
			sserial_response.data[6]=get_buttons();
			sserial_response.datalength=7;
			sserial_send_response();
		}
		if (sserial_request.command==2)
		{
			set_relays_1(sserial_request.data[4]);
			set_relays_2(sserial_request.data[5]);
			sserial_response.datalength=4;
			sserial_send_response();
		}
		if (sserial_request.command==3)
		{
			for (byte i=0; i<8; i++)
			{
				input_output_table_1[i]=sserial_request.data[4+i];
				input_output_table_2[i]=sserial_request.data[12+i];
			}
			input_mode_table=sserial_request.data[20];
			sserial_response.datalength=4;
			sserial_send_response();
		}
	}
}

void device_work()
{
	static byte last_buttons;
	byte buttons=get_buttons();
	if (buttons!=last_buttons)
	{
		byte changed_inputs=buttons^last_buttons;
		
		byte relay_1_change=0;
		byte relay_2_change=0;
		
		if (changed_inputs&1) 	relay_1_change |= input_output_table_1[0];
		if (changed_inputs&2) 	relay_1_change |= input_output_table_1[1];
		if (changed_inputs&4) 	relay_1_change |= input_output_table_1[2];
		if (changed_inputs&8) 	relay_1_change |= input_output_table_1[3];
		if (changed_inputs&16) 	relay_1_change |= input_output_table_1[4];
		if (changed_inputs&32) 	relay_1_change |= input_output_table_1[5];
		if (changed_inputs&64) 	relay_1_change |= input_output_table_1[6];
		if (changed_inputs&128)	relay_1_change |= input_output_table_1[7];
		
		if (changed_inputs&1) 	relay_2_change |= input_output_table_2[0];
		if (changed_inputs&2) 	relay_2_change |= input_output_table_2[1];
		if (changed_inputs&4) 	relay_2_change |= input_output_table_2[2];
		if (changed_inputs&8) 	relay_2_change |= input_output_table_2[3];
		if (changed_inputs&16) 	relay_2_change |= input_output_table_2[4];
		if (changed_inputs&32) 	relay_2_change |= input_output_table_2[5];
		if (changed_inputs&64) 	relay_2_change |= input_output_table_2[6];
		if (changed_inputs&128)	relay_2_change |= input_output_table_2[7];	
		
		byte relays_1=get_relays_1();
		byte relays_2=get_relays_2();	
		set_relays_1(relays_1^relay_1_change);
		set_relays_2(relays_2^relay_2_change);
		
		last_buttons=buttons;
		var_delay_ms(100);
	}

	last_buttons=buttons;
};