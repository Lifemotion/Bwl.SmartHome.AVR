#define DEVNAME "BwlSH.SS.SwitchOne-1.0"
#define getbit(port, bit)		((port) &   (1 << (bit)))
#define setbit(port,bit,val)	{if ((val)) {(port)|= (1 << (bit));} else {(port) &= ~(1 << (bit));}}

char switch_external_state=0;
char switch_internal_state=0;

char get_relay()
{
	return getbit(PORTB,0);
}

char get_button()
{
	setbit(DDRA,0,0);
	setbit(PUEA,0,1);

	return (getbit(PINA,0)==0);
}

void set_relay(char state)
{
	setbit(DDRB,0,1);
	setbit(PORTB,0,state);
	DDRB|=(1<<3);PORTA&=(~(1<<3));
}

void sserial_send_start()
{
	DDRA|=(1<<3);PORTA|=(1<<3);
}

void sserial_send_end()
{
	DDRA|=(1<<3);PORTA&=(~(1<<3));
}

void sserial_process_request()
{
	if (sserial_request.command==1)
	{
		if ((sserial_request.data[0]==124)&&(sserial_request.data[1]==45)&&(sserial_request.data[2]==67)&&(sserial_request.data[3]==251))
		{
			if (sserial_request.data[4]==1) {switch_internal_state=sserial_response.data[5];}		
			sserial_response.datalength=6;
			sserial_response.data[0]=12;
			sserial_response.data[1]=79;
			sserial_response.data[2]=36;
			sserial_response.data[3]=129;
			sserial_response.data[4]=switch_external_state;
			sserial_response.data[5]=switch_internal_state;
			sserial_send_response();
		}	
	}
}

void device_work()
{
	switch_external_state=get_button();
	set_relay(switch_external_state!=switch_internal_state);
};