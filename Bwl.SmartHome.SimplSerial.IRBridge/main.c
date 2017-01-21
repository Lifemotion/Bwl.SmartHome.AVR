/*
 * IRBridge.c
 *
 * Created: 15.01.2017 11:17:09
 * Author : gus10
 */ 
#define F_CPU 12000000
#define BAUD 9600
#define DEVNAME "SmartHome IR control"

#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/delay.h>
#include <util/setbaud.h>

#include "lib/IRlib.h"
#include "lib/bwl_uart.h"
#include "lib/bwl_simplserial.h"

void var_delay_us(int us)
{
	for(int i=0;i<us;i++)_delay_us(1);
}

void var_delay_ms(int ms)
{
	for(int i=0;i<ms;i++)_delay_ms(1);
}

void IR_work(char proto, char device, char cmd, char single)
{
	switch(proto){
		case 1: RC5_send(device, cmd, single); break;
		default: break;
	}
}
void sserial_send_start()
{
	
}

void sserial_send_end()
{
	
}

void sserial_process_request()
{
	if (sserial_request.command==1)
	{
		if ((sserial_request.data[0]==12)&&(sserial_request.data[1]==47)&&(sserial_request.data[2]==55)&&(sserial_request.data[3]==100))
		{		
			sserial_response.datalength=4;
			sserial_response.data[0]=10;
			sserial_response.data[1]=20;
			sserial_response.data[2]=30;
			sserial_response.data[3]=40;
			sserial_send_response();
			IR_work(sserial_request.data[4], sserial_request.data[5], sserial_request.data[6], sserial_request.data[7]);
		}
	}
		if (sserial_request.command==2)
		{
			if ((sserial_request.data[0]==12)&&(sserial_request.data[1]==47)&&(sserial_request.data[2]==55)&&(sserial_request.data[3]==100))
			{
				sserial_response.datalength=4;
				sserial_response.data[0]=10;
				sserial_response.data[1]=20;
				sserial_response.data[2]=30;
				sserial_response.data[3]=40;
				sserial_send_response();
				uint16_t data = sserial_request.data[4]<<8|sserial_request.data[5];
				for(char i=13;i!=-1;i++)
				RC5_send_bit(data&(1<<i));
			}
		}
}

int main(void)
{
	IR_init();
	DDRA |= (1<<1);
	PORTA &= ~(1<<1);
	sserial_set_devname(DEVNAME);
	uart_init_withdivider(1, UBRR_VALUE);		
    while (1) 
    {
		sserial_poll_uart(1);
    }
}

