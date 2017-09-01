#include "board.h"
#include "../guid"
unsigned char dev_guid[16]   = DEV_GUID;
char	 current_valve_state = 1;

char get_button()
{
	setbit(DDRA,2,0);
	setbit(PUEA,2,1);
	return (getbit(PINA,2)==0);
}

void toggle_valve_state()
{
	set_valve_state((current_valve_state==0 ? 1:0));
}

void set_valve_state(char valve_state)
{
	unsigned char direct_one = valve_state==0 ? 0:1;
	unsigned char direct_two = valve_state==0 ? 1:0;
	setbit(DDRA,0,1);
	setbit(DDRA,1,1);
	setbit(PORTA,0,direct_one);
	setbit(PORTA,1,direct_two);
	current_valve_state = valve_state;
}

void reset_valve_power()
{
	setbit(DDRA,0,1);
	setbit(DDRA,1,1);
	setbit(PORTA,0,0);
	setbit(PORTA,1,0);
}

void sserial_send_start()
{
	DDRA|=(1<<3);PORTA|=(1<<3);
	var_delay_ms(1);
}

void sserial_send_end()
{
	var_delay_ms(1);
	DDRA|=(1<<3);
	PORTA&=(~(1<<3));
}

void var_delay_ms(int ms)
{
	for (int i=0; i<ms; i++)_delay_ms(1.0);
}

void board_init()
{
	wdt_enable(WDTO_8S);
	for (byte i=0; i<16; i++)
	{
		sserial_devguid[i]=dev_guid[i];
	}
	reset_valve_power();
	sserial_set_devname(DEVNAME);
	uart_init_withdivider(0,UBRR_VALUE);
}
