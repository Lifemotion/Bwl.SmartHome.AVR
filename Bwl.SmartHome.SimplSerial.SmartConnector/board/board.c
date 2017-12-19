 #include "board.h"

 unsigned int  wdt_counter = 0;
 unsigned char power_mode = 1;

 void var_delay_ms(int ms)
 {
	for(int i=0;i<ms;i++)_delay_ms(1.0);
 }

 void sserial_send_start(unsigned char portindex){}
 void sserial_send_end(unsigned char portindex){}

 void sserial_process_request(unsigned char portindex)
 {
	if(sserial_request.command == 1)
	{
		wdt_counter = 600;
		sserial_response.result = 128;
		sserial_response.datalength = 0;
		sserial_send_response();
	}
 }

 void board_init()
 {
	wdt_enable(WDTO_8S);
	pin_set_dir(LED, 1);
	pin_set_dir(SWITCH, 1);
	if(eeprom_read_byte((uint8_t*)0) == 0xF1)power_mode = 1;
	if(eeprom_read_byte((uint8_t*)0) == 0xF0)power_mode = 0;
	if(power_mode==0){pin_low(SWITCH); pin_high(LED)}else{pin_high(SWITCH);}
	uart_init_withdivider_x2(UART, GET_UBRR_X2(F_CPU, BAUD));
	TCCR1B |= (1<<CS12 | 1<<CS10);
	TIMSK1 |= (1<<TOIE1);
	sserial_address = 100;
	sei(); 
 }
 void check_button()
 {
	pin_input_pullup(BUTTON);
	if(pin_get_in(BUTTON) == 0){
		var_delay_ms(100);
		unsigned int button_counter = 0;
		cli();
		while(pin_get_in(BUTTON) == 0){
			_delay_ms(100);
			button_counter++;
			if(button_counter>50){
				pin_toggle_out(LED);
			}
			wdt_reset();
		}
		sei();
		if(button_counter>50){
			if(power_mode == 1){
				power_mode = 0;
				eeprom_write_byte((uint8_t*)0,0xF0);
			}else{
				power_mode = 1;
				eeprom_write_byte((uint8_t*)0,0xF1);
			}
		}else{
			pin_toggle_out(SWITCH);
		}
	}
}

ISR(TIMER1_OVF_vect){
	cli();
	TCNT1 = 57722;	
	if(power_mode == 1){
		if(pin_get_out(SWITCH)>0){
			pin_low(LED);
			}else{
			pin_high(LED);
		}
		return;
	}
	if(wdt_counter>0)
	{
		wdt_counter--;
		pin_toggle_out(LED);
	} else {
		pin_high(SWITCH);
		for(int i=0;i<20;i++){
			pin_toggle_out(LED);
			_delay_ms(50);
		}
		pin_low(SWITCH);
		wdt_counter = 600;
	}
	sei();
 }
 
