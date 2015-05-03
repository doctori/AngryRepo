/*
   March 2014 - TMRh20 - Updated along with High Speed RF24 Library fork
   Parts derived from examples by J. Coliz <maniacbug@ymail.com>
*/
/**
 * Example for efficient call-response using ack-payloads 
 * 
 * This example continues to make use of all the normal functionality of the radios including 
 * the auto-ack and auto-retry features, but allows ack-payloads to be written optionlly as well. 
 * This allows very fast call-response communication, with the responding radio never having to 
 * switch out of Primary Receiver mode to send back a payload, but having the option to switch to 
 * primary transmitter if wanting to initiate communication instead of respond to a commmunication. 
 */
 
#include <SPI.h>
#include "nRF24L01.h"
#include "RF24.h"
#include "printf.h"

// Hardware configuration: Set up nRF24L01 radio on SPI bus plus pins 7 & 8 
RF24 radio(9,10);
                                                                           // Topology
byte addresses[][6] = {"1Node","2Node"};
const int max_payload_size = 32;// Radio pipe addresses for the 2 nodes to communicate.
char receive_payload[max_payload_size+1];
byte counter = 1;                                                          // A single byte to keep track of the data being sent back and forth


void setup(){

  Serial.begin(57600);
  printf_begin();
  printf("\n\r First Try out with Raspberry on the other side \n\r");

  // Setup and configure radio

  radio.begin();
  radio.setAutoAck(1);                    // Ensure autoACK is enabled
  radio.enableAckPayload();               // Allow optional ack payloads
  radio.setRetries(0,15);                 // Smallest time between retries, max no. of retries
  radio.openWritingPipe(addresses[1]);        // Both radios listen on the same pipes by default, and switch when writing
  radio.openReadingPipe(1,addresses[0]);      // Open a reading pipe on address 0, pipe 1
  radio.startListening();                 // Start listening
  radio.powerUp();
  radio.printDetails();                   // Dump the configuration of the rf unit for debugging
}
void sendCallback(unsigned short callback){
   // First, stop listening so we can talk
      radio.stopListening();

      // Send the final one back.
      radio.write( &callback, sizeof(unsigned short) );
      printf("Sent response. [%d] \n\r", callback);

      // Now, resume listening so we catch the next packets.
      radio.startListening();
}
//Will fetch the ID (the First Part of the message)
unsigned short getId(char * rawMessage, unsigned short length){
    unsigned short i = 0;
    unsigned short id = 0;
    for( i=1; i< length; i++){
        id += digit_to_int(rawMessage[i])*pow( 10, i-1 );
    }
    return id;
}
int digit_to_int(char d)
{
 char str[2];

 str[0] = d;
 str[1] = '\0';
 return (int) strtol(str, NULL, 10);
}
unsigned short getMessage( char * rawMessage){
    unsigned short message = digit_to_int(rawMessage[0]);
    return (unsigned short)message;
}

int getState(unsigned short pin){
  boolean state = digitalRead(pin);
  return state == true ? 0 : 1;
}

void doAction(unsigned short id, unsigned short action){
    printf("The Id recieved is [%d] and the Action is : [%d] \n",id,action);
    if( action == 0 ){
        digitalWrite(id, HIGH);
    }else{
        digitalWrite(id, LOW);
    }
}
void performAction(char * received_payload, uint8_t payloadLength){
  unsigned short action, id, callback;
  char * castedMessage;
  printf("Received : [%s] ",received_payload);
  printf("\n\r TEST : [%c] \n\r",received_payload[0]);
  printf("With the Length : [%d]",payloadLength);
  action = getMessage(received_payload);
  id = getId(received_payload, payloadLength);
  printf("Action is : [%d], With Id : [%d] \n\r",action,id);
  if (action == 0 || action ==1){
      callback = action;
      doAction(id, action);
  }else if(action == 2){
      callback = getState(id);
  }
  sendCallback(callback);


 
}
void loop(void) {
  byte pipeNo;                          // Declare variables for the pipe and the byte received
  unsigned long rawMessage;
  while( radio.available(&pipeNo)){   
      // Fetch the payload, and see if this was the last one.
      uint8_t len = radio.getDynamicPayloadSize();
      
      // If a corrupt dynamic payload is received, it will be flushed
      if(!len){
        continue; 
      }
      radio.read( receive_payload,len);
      receive_payload[len] = 0;
      performAction(receive_payload,len);
      
 }
  if ( Serial.available() )
  {
      
       radio.openWritingPipe(addresses[1]);      // Since only two radios involved, both listen on the same addresses and pipe numbers in RX mode
       radio.openReadingPipe(1,addresses[0]);    // then switch pipes & addresses to transmit. 
       radio.startListening();                   // Need to start listening after opening new reading pipes
  }
}



