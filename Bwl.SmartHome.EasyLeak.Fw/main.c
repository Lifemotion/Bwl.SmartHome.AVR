
#include "board/board.h"

unsigned char current_valve_state=0; 
unsigned char leak=0;

void sserial_process_request(unsigned char portindex)
{
	if (sserial_request.command==1)
	{
		sserial_response.result		 = 128;
		sserial_response.data[0]     = sensor_state();
		sserial_response.data[1]     = current_valve_state;
		sserial_response.datalength	 = 2;
		sserial_send_response();
	}

	if (sserial_request.command==2)
	{
		sserial_response.result		 = 128;
		sserial_response.data[0]     = sensor_state();
		sserial_response.data[1]     = pin_get_in(BUTTON);
		current_valve_state = sserial_request.data[0];
		sserial_response.datalength	 = 2;
		sserial_send_response();
	}

	if (sserial_request.command==3)
	{
		sserial_response.result		 = 128;
		sserial_response.datalength	 = 0;
		beep(sserial_request.data[0]);
		sserial_send_response();
	}
}

ISR(USART_RX_vect)
{
	cli();
	sserial_poll_uart(RS485);
	sei();
}

int main(void)
{
	unsigned char delay_counter=0;
	board_init();
	while (1)
	{
		wdt_reset();
		if(pin_get_in(BUTTON) == 0) 
		{
			pin_high(LED);
			var_delay_ms(100);
			pin_low(LED);
			if(current_valve_state == 0)
			{
	          	current_valve_state = 1;
			}else{
			    current_valve_state = 0;
			}
		}
		if(sensor_state()){
			current_valve_state = 1;
			delay_counter+=5;
			leak=1;
		}else{
	        leak=0;
	    }
		beep_enable(leak);
	    valve_set_state(current_valve_state);
		_delay_ms(300);
		if(delay_counter++>10)
		{
			delay_counter=0;
			pin_high(LED);
			var_delay_ms(10+leak*500);
			pin_low(LED);
		}
	}

}

