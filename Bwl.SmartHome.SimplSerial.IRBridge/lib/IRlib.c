/*
 * IRlib.c
 *
 * Created: 18.01.2017 23:22:24
 *  Author: gus10
 */ 

#include "IRlib.h"
#include <avr/io.h>
#define F_CPU 12000000
#include <util/delay.h>

char RC5_next = 1;
 void IR_init()
 {
	 IR_LED_DDR |= (1<<IR_LED_PIN);
	 IR_LED_PORT &= ~(1<<IR_LED_PIN);
 }

 void RC5_send_bit(char bit)
 {
	 char i = 0;
	 if(bit){
		 _delay_us(888);
		 for(i=0;i<32;i++){
			 PORTA |= (1<<IR_LED_PIN);
			 _delay_us(4);
			 PORTA &= ~(1<<IR_LED_PIN);
			 _delay_us(22);
		 }
		 }else{
		 for(i=0;i<32;i++){
			 PORTA |= (1<<IR_LED_PIN);
			 _delay_us(4);
			 PORTA &= ~(1<<IR_LED_PIN);
			 _delay_us(22);
		 }
		 _delay_us(888);
	 }
 }

 void RC5_send(char device, char cmd, char single)
 {
	 /*преамбула*/
	 if(single){
	 RC5_send_bit(1);
	 RC5_send_bit(1);
	 RC5_send_bit(0);
	  for(char i=0;i<5;i++){
		  RC5_send_bit(device&(1<<i));
	  }
	  for(char i=0;i<6;i++){
		  RC5_send_bit(cmd&(1<<i));
	  }
	 }else{
		RC5_send_bit(1);
		RC5_send_bit(0);
		RC5_send_bit(RC5_next);		
		for(char i=0;i<5;i++){
			 RC5_send_bit(device&(1<<i));
		}

		for(char i=0;i<6;i++){
			 RC5_send_bit(cmd&(1<<i));
		}
	 }
	if(RC5_next){ RC5_next=0; }else{ RC5_next = 1; }
	
 }