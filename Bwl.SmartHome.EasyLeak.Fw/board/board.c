/*
 * board.c
 *
 * Created: 27.08.2017 12:48:49
 *  Author: gus10
 */ 
 #include "board.h"
 #include "../guid"
 unsigned char dev_guid[16]   = DEV_GUID;
void var_delay_ms(int ms)
{
	for(int i=0;i<ms;i++)_delay_ms(1.0);
}

void sserial_send_start(unsigned char portindex)
{
	pin_high(RS485_DIR);
	var_delay_ms(1);
}

void sserial_send_end(unsigned char portindex)
{
	var_delay_ms(1);
	pin_low(RS485_DIR);
}

void board_init(){
	pin_input_pullup(BUTTON);
	valve_set_state(1);
	wdt_enable(WDTO_8S);
	adc_init(ADC_MUX_ADC4, ADC_ADJUST_RIGHT, ADC_REFS_AVCC, ADC_PRESCALER_64);
	uart_init_withdivider(RS485, UBRR_VALUE);
	for (byte i=0; i<16; i++)
	{
		sserial_devguid[i]=dev_guid[i];
	}
	sserial_set_devname(DEVNAME);
	UCSR0B|=(1<<RXCIE0);
	sei();
}

unsigned char sensor_state()
{
	if(adc_read_average(5)<ADC_TRESHOLD_VALUE)return 1;
	return 0;
}

void stepup_set_state(unsigned char state)
{
	if(state){
		pin_high(STEP_UP);
	}else{
		pin_low(STEP_UP);
	}
}

void valve_set_state(unsigned char state)
{
	if(state){
		pin_low(DRV1_N);
		pin_high(DRV1_P);
		pin_low(DRV2_N);
		pin_high(DRV2_P)
	}else{
		pin_low(DRV1_P);
		pin_high(DRV1_N);
		pin_low(DRV2_P);
		pin_high(DRV2_N)
	}
}

void beep(unsigned int t)
{
	for(unsigned int i=0;i<t;i++){
		pin_high(BUZZER);
		_delay_ms(0.95);
		pin_low(BUZZER);
		_delay_ms(0.95);
	}
}

void beep_enable(unsigned char state)
{
	if(state)
	{
		pin_low(BUZZER);
		TCCR1B=0x5;
		OCR1AL=0xFF;
		TCCR1A=0x83;		
	}else{
	    TCCR1B=0;
		pin_low(BUZZER);
	}
}