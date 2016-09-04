#define BAUD 9600
#define F_CPU 8000000UL
#include <util/setbaud.h>
#include <util/delay.h>

	
#include "libs/bwl_uart.h"
#include "libs/bwl_simplserial.h"

#include "multiswitch-one.h"

#include "../guid"
unsigned char dev_guid[16]=DEV_GUID;

void var_delay_ms(int ms)
{
	for (int i=0; i<ms; i++)_delay_ms(1.0);
}

int main(void)
{
	wdt_enable(WDTO_4S);
	sserial_set_devname(DEVNAME);
	for (byte i=0; i<16; i++)
	{
		sserial_devguid[i]=dev_guid[i];
	}
	uart_init_withdivider(0,UBRR_VALUE);
	while(1)
	{
		sserial_poll_uart(0);
		device_work();
		wdt_reset();
	}
}