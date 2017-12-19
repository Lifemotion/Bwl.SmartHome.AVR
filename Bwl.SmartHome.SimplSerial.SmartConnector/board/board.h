/*
 * board.h
 *
 * Created: 12.12.2017 17:26:06
 *  Author: Andrey Blinov, Nikolay Gusev
 */ 

#ifndef BOARD_H_
#define BOARD_H_

#define F_CPU  16000000
#define UART   0
#define BAUD   115200

#include <avr/eeprom.h>
#include <avr/wdt.h>
#include <util/delay.h>
#include <avr/interrupt.h>

#include "../refs/bwl_pins.h"
#include "../refs/bwl_simplserial.h"
#include "../refs/bwl_uart.h"

#define BUTTON D,6
#define SWITCH D,7
#define LED    B,0

void board_init();
void check_button();

#endif /* BOARD_H_ */