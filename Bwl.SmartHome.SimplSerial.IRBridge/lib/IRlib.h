/*
 * RC5cmd.h
 *
 * Created: 18.01.2017 17:12:19
 *  Author: gus10
 */ 


#ifndef RC5CMD_H_
#define RC5CMD_H_

#define IR_LED_DDR DDRA
#define IR_LED_PORT PORTA
#define IR_LED_PIN 7

void IR_init();
void RC5_send(char device, char cmd, char single);
void RC5_send_bit(char bit);
#endif /* RC5CMD_H_ */