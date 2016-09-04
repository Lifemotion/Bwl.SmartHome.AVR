#include <avr/io.h>

#define DEVNAME "BwlSH.SS.MultiSwitch-1.0"
#define getbit(port, bit)		((port) &   (1 << (bit)))
#define setbit(port,bit,val)	{if ((val)) {(port)|= (1 << (bit));} else {(port) &= ~(1 << (bit));}}

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

byte get_relays1()
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

byte get_relays2()
{
	byte result=0;
	if (getbit(PINC,0)) {result+=256;}
	if (getbit(PINC,1)) {result+=512;}
	if (getbit(PINC,6)) {result+=1024;}
	if (getbit(PINC,7)) {result+=2048;}
	return result;
}

void set_relays1(byte states)
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

void set_relays2(byte states)
{
	setbit(DDRC,0,1);
	setbit(DDRC,1,1);
	setbit(DDRC,6,1);
	setbit(DDRC,7,1);
	setbit (PORTC,0,(states&256));
	setbit (PORTC,1,(states&512));
	setbit (PORTC,6,(states&1024));
	setbit (PORTC,7,(states&2048));
}

void sserial_send_start()
{
	DDRD|=(1<<2);PORTD|=(1<<2);
}

void sserial_send_end()
{
	DDRD|=(1<<2);PORTD&=(~(1<<2));
}

void sserial_process_request()
{
	if ((sserial_request.data[0]==57)&&(sserial_request.data[1]==129)&&(sserial_request.data[2]==33)&&(sserial_request.data[3]==221))
	{
		if (sserial_request.command==1)
		{
			sserial_response.result=sserial_request.command+1;
			sserial_response.datalength=7;
			sserial_response.data[0]=78;
			sserial_response.data[1]=32;
			sserial_response.data[2]=1;
			sserial_response.data[3]=227;
			sserial_response.data[4]=get_relays1();
			sserial_response.data[5]=get_relays2();
			sserial_response.data[6]=get_buttons();
			sserial_send_response();
		}
		if (sserial_request.command==2)
		{
			set_relays1(sserial_request.data[4]);
			set_relays2(sserial_request.data[5]);
			sserial_response.result=sserial_request.command+1;
			sserial_response.datalength=4;
			sserial_response.data[0]=78;
			sserial_response.data[1]=32;
			sserial_response.data[2]=1;
			sserial_response.data[3]=227;
			sserial_send_response();
		}
	}
}

static byte last_buttons=0;
void device_work()
{
	byte buttons=get_buttons();
	if ((buttons&1)!=(last_buttons&1)) 
	{
		byte rel1=get_relays1();
		if (rel1==0)
		{
			set_relays1(255);
			set_relays2(255);
		}else
		{
			set_relays1(0);
			set_relays2(0);
		}
		var_delay_ms(300);
	}
	//switch_external_state=get_button();
	//set_relay(switch_external_state!=switch_internal_state);
	last_buttons=buttons;
};