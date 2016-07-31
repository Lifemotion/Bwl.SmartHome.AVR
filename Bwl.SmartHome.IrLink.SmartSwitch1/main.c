#include "hal.h"

int main(void)
{
	wdt_enable(WDTO_4S);
	DDRB=0; PORTB=0xFF;
	DDRC=0; PORTC=0xFF;
	DDRD=0; PORTD=0xFF;
	PRR=(1<<PRTWI)||(1<<PRTIM0)||(1<<PRTIM1)||(1<<PRTIM2)||(1<<PRSPI)||(0<<PRUSART0)||(1<<PRADC);

	uart_init_withdivider(UBRR_VALUE);			
	hal_led(0);
	hal_tsop_power(1);
	hal_relay_1_off();
	hal_relay_2_off();
			
	while(1)
	{
		if (uart_received())
		{
			char val=uart_get();
			if (val=='1'){hal_relay_1_on();}
			if (val=='2'){hal_relay_1_off();}
			if (val=='3'){hal_relay_2_on();}
			if (val=='4'){hal_relay_2_off();}
			hal_led(1);
			if ((val>'1')&&(val<'9'))
			{
				_delay_ms(100);
				hal_uart_send(val+10);
			}
			hal_led(0);		
		}
		if (hal_button())
		{
			_delay_ms(100);
			hal_uart_send('*');	
			hal_led(0);
			hal_irmod(1);
			uart_disable();
			hal_tsop_power(0);
			PRR=(1<<PRTWI)||(1<<PRTIM0)||(1<<PRTIM1)||(1<<PRTIM2)||(1<<PRSPI)||(1<<PRUSART0)||(1<<PRADC);
			
			clock_prescale_set(clock_div_128);		
		}
		if (hal_inp_1())
		{
			_delay_ms(100);
			hal_uart_send('A');
		}		
		if (hal_inp_2())
		{
			_delay_ms(100);
			hal_uart_send('B');
		}		
		hal_led(1);
		_delay_ms(10);
		wdt_reset();
	}
}