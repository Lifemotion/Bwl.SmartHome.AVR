/*
 * board.h
 *
 * Created: 26.08.2017 12:08:55
 *  Author: gus10
 */ 


#ifndef BOARD_H_
#define BOARD_H_
#define F_CPU		16000000
#define BAUD		9600
#define UART_485	0
#define DEVNAME		"SmartDevice.RemoteControl v1.0  "

#include <avr/io.h>
#include <util/delay.h>
#include <avr/interrupt.h>
#include <avr/wdt.h>
#include <util/setbaud.h>

#include "../libs/bwl_uart.h"
#include "../libs/bwl_simplserial.h"
#include "../libs/dht22.h"

void var_delay_ms(int ms);
void board_init();
#endif /* BOARD_H_ */