#ifndef HAL_H_
#define HAL_H_

#define BAUD 1200
#define F_CPU 1000000UL
#include <util/setbaud.h>
#include <util/delay.h>
#include <avr/wdt.h>
#include <avr/io.h>
#include <avr/power.h>
#include <avr/sleep.h>
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

#define DELAY 9.1

void hal_uart_send(char data)
{
	hal_irmod(0);
	uart_send(data);
	for (int i=0; i<20000; i++)
	{
		setbit (PORTD,3,0);
		_delay_us(DELAY);
		setbit (PORTD,3,1);
		_delay_us(DELAY);
	}
}

void hal_relay_1_on()
{
	setbits (DDRD,PORTD,5,1,1);
	setbits (DDRD,PORTD,6,1,0);
	setbits (DDRD,PORTD,7,0,0);
	_delay_ms(50);
	setbits (DDRD,PORTD,5,0,1);
	setbits (DDRD,PORTD,6,0,1);
	setbits (DDRD,PORTD,7,0,1);
}

void hal_relay_1_off()
{
	setbits (DDRD,PORTD,5,1,0);
	setbits (DDRD,PORTD,6,1,1);
	setbits (DDRD,PORTD,7,0,0);
	_delay_ms(50);
	setbits (DDRD,PORTD,5,0,1);
	setbits (DDRD,PORTD,6,0,1);
	setbits (DDRD,PORTD,7,0,1);
}

void hal_relay_2_on()
{
	setbits (DDRD,PORTD,5,1,1);
	setbits (DDRD,PORTD,6,0,0);
	setbits (DDRD,PORTD,7,1,0);
	_delay_ms(50);
	setbits (DDRD,PORTD,5,0,1);
	setbits (DDRD,PORTD,6,0,1);
	setbits (DDRD,PORTD,7,0,1);
}

void hal_relay_2_off()
{
	setbits (DDRD,PORTD,5,1,0);
	setbits (DDRD,PORTD,6,0,0);
	setbits (DDRD,PORTD,7,1,1);
	_delay_ms(50);
	setbits (DDRD,PORTD,5,0,1);
	setbits (DDRD,PORTD,6,0,1);
	setbits (DDRD,PORTD,7,0,1);
}

char hal_button()
{
	setbits (DDRB,PORTB,1,0,1);
	if (getbit(PINB,1)==0){return 1;}
	return 0;
}

char hal_inp_1()
{
	setbits (DDRC,PORTC,0,1,0);
	setbits (DDRC,PORTC,3,0,1);
	if (getbit(PINC,3)==0){return 1;}
	return 0;	
}

char hal_inp_2()
{
	setbits (DDRC,PORTC,0,1,0);
	setbits (DDRC,PORTC,2,0,1);
	if (getbit(PINC,2)==0){return 1;}
	return 0;
}

#endif /* HAL_H_ */