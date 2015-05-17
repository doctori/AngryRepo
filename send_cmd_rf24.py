#!/usr/bin/env python
import time
import getopt
import sys
from RF24 import *
radio = RF24(22,0)
pipes = [ "1Node","2Node"]
millis = lambda: int(round(time.time() * 1000))
payloadSize = 16

def usage():
	print "Please use the -a and -d option"
	print "use the -i parameter in order to print the radio infos"
def printRadioInfo():
	radio.begin()
	radio.printDetails()

def main(argv):
	action = '0'
	device = False
	try:
		opts, args = getopt.getopt(argv, "hia:d:", ["help", "action=","device="])
	except getopt.GetoptError:
		usage()
		sys.exit(2)
	for opt, arg in opts:
		if opt in ("-h", "--help"):
			usage()
			sys.exit()
		elif opt in ("-a", "--action"): 
			action = arg
			print "Trying To Send The Action :",action,"\n\r"
		elif opt in ("-d", "--device"): 
			device = arg
			print "Trying To Send to ",device,"\n\r"
		elif opt in ("-i"):
			printRadioInfo()
	if not device : 
		print "device not given"
		usage()
		sys.exit(2)
	payload = device+"|"+action
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
			break
		time.sleep(1)

if __name__ == "__main__": main(sys.argv[1:])
