#define BAUD  9600
#define F_CPU 8000000UL
#define DEVNAME "SmartDevice.ValveControl v1.0"
#define RS485 0

#include <avr/io.h>
#include <util/setbaud.h>
#include <util/delay.h>
#include <avr/wdt.h>

#include "../libs/bwl_uart.h"
#include "../libs/bwl_simplserial.h"

#define getbit(port, bit)		((port) &   (1 << (bit)))
#define setbit(port,bit,val)	{if ((val)) {(port)|= (1 << (bit));} else {(port) &= ~(1 << (bit));}}
#define LED_ON  {setbit(DDRA,5,1);setbit(PORTA,5,1);}
#define LED_OFF {setbit(DDRA,5,1);setbit(PORTA,5,0);}

char current_valve_state;

char get_button();
void set_valve_state(char state);
void reset_valve_power();
void sserial_send_start();
void sserial_send_end();
void var_delay_ms(int ms);
void toggle_valve_state();
void board_init();