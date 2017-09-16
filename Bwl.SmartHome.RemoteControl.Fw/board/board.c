/*
 * board.c
 *
 * Created: 26.08.2017 12:08:31
 *  Author: gus10
 */ 

#include "board.h"
#include "../guid"
unsigned char dev_guid[16] = DEV_GUID;

void var_delay_ms(int ms)
{
	for(int i=0;i<ms;i++)_delay_ms(1.0);
}


void sserial_send_start(unsigned char portindex)
{
	if (portindex==UART_485){
		DDRC  |= (1<<7);
		PORTC |= (1<<7);
	}
}

void sserial_send_end(unsigned char portindex)
{
	if (portindex==UART_485)
	{
		DDRC  |= (1<<7);
		PORTC &= (~(1<<7));
	}
}


void board_init()
{	
	uart_init_withdivider(UART_485, UBRR_VALUE);
	sserial_set_devname(DEVNAME);
	sserial_find_bootloader();
	for (byte i=0; i<16; i++)
	{
		sserial_devguid[i]=dev_guid[i];
	}
	DDRC  |=  (1<<6);
	PORTC &= ~(1<<6);
	DDRD  |=  (1<<5);
	PORTD &= ~(1<<5);
	DDRD   |=  (1<<5);
	DDRD   &= ~(1<<2);
	PORTD  |=  (1<<2);
	TCCR0A |= (1<<WGM00);
	OCR0A   = 180;
	TIMSK0 |= (1<<OCIE0A);
	
	MCUCR = 1<<ISC01;
	EIMSK = 1<<INT0;

	wdt_enable(WDTO_8S);
	sei();
}