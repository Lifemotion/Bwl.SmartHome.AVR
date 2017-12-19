/*
 * board.h
 *
 * Created: 12.12.2017 17:26:06
 *  Author: Nikolay Gusev, Andrey Blinov
 */ 
#include <avr/io.h>
#include "board/board.h"

int main(void)
{
	board_init();
    while (1) 
    {
		check_button();
		sserial_poll_uart(UART);
		wdt_reset();
    }
}

