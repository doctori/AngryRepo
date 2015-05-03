#!/usr/bin/env python
import time
import getopt
import sys
from RF24 import *
radio = RF24(22,0)
pipes = [ "1Node","2Node"]
millis = lambda: int(round(time.time() * 1000))
payloadSize = 16

def main(argv):
	try:
		opts, args = getopt.getopt(argv, "hm:", ["help", "message="])
	except getopt.GetoptError:
		usage()
		sys.exit(2)
	for opt, arg in opts:
		if opt in ("-h", "--help"):
			usage()
			sys.exit()
		elif opt in ("-m", "--message"): 
			payload = arg
			print "Trying To Send ",payload,"\n\r"
			
	if not payload : 
		print "message not given"
		usage()
		sys.exit(2)

	print 'Trying To Communicate with the Arduino Nano'
	radio.begin()
	radio.setAutoAck(1);
	radio.enableAckPayload();
	radio.setRetries( 15, 15);
	
	radio.printDetails()

	radio.openWritingPipe(pipes[0])
	radio.openReadingPipe(1,pipes[1])

	while 1:
		radio.stopListening()
		print 'Now sending ', str(payload), ' ... ',
		radio.write(payload)
		radio.startListening()

		# Wait here until we get a response, or timeout
		started_waiting_at = millis()
		timeout = False
		while (not radio.available()) and (not timeout):
			if (millis() - started_waiting_at) > 1000:
				timeout = True

		# Describe the results
		if timeout:
			print 'failed, response timed out.'
		else:
			# Grab the response, compare, and send to debugging spew
			len = radio.getDynamicPayloadSize()
			receive_payload = radio.read(len)

			# Spew it
			print 'got response size=', len, ' value="', receive_payload, '"'
		time.sleep(5)

if __name__ == "__main__": main(sys.argv[1:])