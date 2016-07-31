/*
 * Bwl UART lib for AVR: mega48/88/168/328
 *
 * Author: Igor Koshelev and others
 * Licensed: open-source Apache license
 *
 * Version: 29.07.2015
 */ 

#include <avr/io.h>
#include "bwl_uart.h"

void uart_init_withdivider(unsigned int ubrr)
{
	UBRR0H = (unsigned char)(ubrr>>8);
	UBRR0L = (unsigned char)ubrr;
	UCSR0A = (0<<U2X0);
	UCSR0B = (1<<RXEN0)|(1<<TXEN0);
	UCSR0C = (1<<USBS0)|(3<<UCSZ00);
}

void uart_disable()
{
	UCSR0B = (0<<RXEN0)|(0<<TXEN0);	
}

void uart_send( unsigned char byte )
{
	while ( !( UCSR0A & (1<<UDRE0)) );
	UDR0 = byte;
}

 unsigned char uart_received( void )
{
	return (UCSR0A & (1<<RXC0));
}

unsigned char uart_get( void )
{
	return UDR0;
}
