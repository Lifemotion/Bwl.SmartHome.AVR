/*
 * board.h
 *
 * Created: 27.08.2017 12:49:04
 * Author: Nickolay Gusev, Andrey Blinov
 */ 

 #define F_CPU				8000000
 #define BAUD				9600
 #define RS485				0

 #define LED				B,0
 #define BUTTON				B,2
 #define BUZZER				B,1
 #define RS485_DIR			D,2
 #define STEP_UP            D,3

 #define DRV1_P             C,0
 #define DRV1_N             C,3
 #define DRV2_P             C,1
 #define DRV2_N             C,2

 #define ADC_TRESHOLD_VALUE 500
 #define DEVNAME            "EasyLeak.Controller v1.0     "
 #include <avr/io.h>
 #include <avr/wdt.h>
 #include <util/delay.h>
 #include <avr/interrupt.h>
 #include <util/setbaud.h>

 #include "../libs/bwl_pins.h"
 #include "../libs/bwl_simplserial.h"
 #include "../libs/bwl_uart.h"
 #include "../libs/bwl_adc.h"

 void			valve_set_state(unsigned char state);
 void			stepup_set_state(unsigned char state);
 void			board_init();
 void			var_delay_ms(int ms);
 void			beep(unsigned int t);
 unsigned char  sensor_state();
 void			beep_enable(unsigned char state);