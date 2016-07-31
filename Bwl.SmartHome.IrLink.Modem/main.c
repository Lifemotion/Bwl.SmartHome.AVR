#define BAUD 9600
#define F_CPU 8000000UL
#include <util/setbaud.h>
#include <util/delay.h>
#include <avr/wdt.h>
#include <avr/io.h>
#define getbit(port, bit)		((port) &   (1 << (bit)))
#define setbit(port,bit,val)	{if ((val)) {(port)|= (1 << (bit));} else {(port) &= ~(1 << (bit));}}
#define setbits(port1,port2,bit,val1,val2)	{if ((val1)) {(port1)|= (1 << (bit));} else {(port1) &= ~(1 << (bit));}; if ((val2)) {(port2)|= (1 << (bit));} else {(port2) &= ~(1 << (bit));} }
	
#include "libs/bwl_uart.h"

void var_delay_ms(int ms)
{
	for (int i=0; i<ms; i++)_delay_ms(1.0);
}

void hal_irmod(char state)
{
	setbits (DDRD,PORTD,3,1,state);
}

void hal_tsop_power(char state)
{
	setbits (DDRD,PORTD,4,1,state);
}

void hal_led(char state)
{
	setbits (DDRB,PORTB,0,1,state);
}

#define DELAY 11.3

int main(void)
{
	wdt_enable(WDTO_4S);
	hal_tsop_power(1);
	hal_led(1);
	//uart_init_withdivider(UBRR_VALUE);
	while(1)
	{
		setbits (DDRD,PORTD,3,1,1);
		_delay_us(DELAY);
		setbits (DDRD,PORTD,3,1,0);
		_delay_us(DELAY);
		wdt_reset();
		hal_led ((getbit(PIND,0)==0));
	}
}