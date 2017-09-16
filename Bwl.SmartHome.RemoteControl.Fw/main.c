/*
 * SmartDevices.RemoteControl.Fw.c
 *
 * Created: 26.08.2017 12:06:39
 * Author : gus10
 */ 

#include "board/board.h"

unsigned char cmd_array [255];
unsigned char play_array[255];
unsigned char play_flag          = 0;
unsigned char play_array_lenght  = 0;
unsigned char play_array_counter = 0;
unsigned char pointer			 = 0;
unsigned char cmd_captured_flag  = 0;
unsigned char current_pin_state  = 4;

void sserial_process_request(unsigned char portindex)
{
	if(sserial_request.command==1){
		float h, t;
		dht22_read(&t, &h);
		unsigned char *p = (unsigned char *) &t;
		sserial_response.data[0]  = *p++;
		sserial_response.data[1]  = *p++;
		sserial_response.data[2]  = *p++;
		sserial_response.data[3]  = *p++;
		p = (unsigned char *) &h;
		sserial_response.data[4]  = *p++;
		sserial_response.data[5]  = *p++;
		sserial_response.data[6]  = *p++;
		sserial_response.data[7]  = *p++;
		sserial_response.data[8]  = cmd_captured_flag;
		sserial_response.result = 129;
		sserial_response.datalength = 9;
		sserial_send_response();
	}

	if(sserial_request.command == 2){
		sserial_response.datalength = pointer;
		for(int i=1;i<pointer;i++){
			sserial_response.data[i-1] = cmd_array[i];
		}
		play_flag          = 0;
		play_array_lenght  = 0;
		play_array_counter = 0;
		pointer			   = 0;
		cmd_captured_flag  = 0;
		current_pin_state  = 4;
		for(unsigned char i =0;i<255;i++){
			cmd_array[i] = 0;
		}
		sserial_send_response();
	}

	if(sserial_request.command == 3){
		for(int i=0;i<sserial_request.datalength;i++){
			play_array[i] = sserial_request.data[i];
		}
		sserial_response.datalength = 0;
		sserial_response.result = 210;
		play_flag = 1;
		play_array_lenght  = sserial_request.datalength;
		play_array_counter = 0;
		EIMSK &= ~(1<<INT0);
		TCCR0B |= (1<<CS00);
		sserial_send_response();
	}
}

ISR(INT0_vect)
{
	EIMSK &= ~(1<<INT0);
	TCCR0B |= (1<<CS00);
}

ISR(TIMER0_COMPA_vect)
{
	TCNT0 = 0;
	if(play_flag == 0){
		unsigned char pin =  PIND & (1<<2);
		if(pin != current_pin_state){
			pointer++;
			current_pin_state = pin;
		}
		if(cmd_array[pointer]<255){
			cmd_array[pointer]++;
		}else{
			if(pointer!=0){
				cmd_captured_flag = 1;
				EIMSK  |=  (1<<INT0);
				TCCR0B &= ~(1<<CS00);
			}
		}
	}else{
		if(play_array_counter <= play_array_lenght){
			if(play_array_counter % 2){
				PORTD &= ~(1<<5);				
			}else{
				if(PORTD&(1<<5)) PORTD &= ~(1<<5); else PORTD |= (1<<5);
			}
			if(play_array[play_array_counter]-- == 0) play_array_counter++;
		}else{
			cli();
			play_flag = 0;
			TCCR0B &= ~(1<<CS00);
			EIMSK  |=  (1<<INT0);
			PORTD &= ~(1<<5);
			sei();
		}	
	}
}

int main(void)
{
	for(unsigned char i =0;i<255;i++){
		cmd_array[i] = 0;
	}
    board_init();
    while (1) 
    {
		sserial_poll_uart(UART_485);
		wdt_reset();
    }
}

