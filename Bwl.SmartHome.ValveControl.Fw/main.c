/*
 * SmardDevices.ValveControl
 * Author: Nikcolay Gusev, Blinov Andrey 
 * Licensed: open-source Apache license
 * Version: 01.05.2016 V1.5.0
 */ 

#include "board/board.h"

void sserial_process_request()
{
	LED_ON;
	if (sserial_request.command==1)
	{
		sserial_response.result = 128;
		sserial_response.datalength = 0;
		sserial_send_response();	
		set_valve_state(sserial_request.data[0]);		
	}

	if (sserial_request.command==2)
	{
		sserial_response.result = 128;
		sserial_response.data[0] = current_valve_state;
		sserial_response.datalength = 1;
		sserial_send_response();
	}
	LED_OFF;
}

int main(void)
{
	board_init();
	sserial_send_end();
	while(1)
	{
		wdt_reset();
		sserial_poll_uart(RS485);
		if(get_button()){
			toggle_valve_state();
			var_delay_ms(30);
		}
	}
}