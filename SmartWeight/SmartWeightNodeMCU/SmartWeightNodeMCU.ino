/*
 Name:		SmartWeightNodeMCU.ino
 Created:	11/3/2022 8:59:03 AM
 Author:	dani146d
 Credit:    Lasse Lund Madsen
*/

/*
  Rui Santos
  Complete project details at https://RandomNerdTutorials.com/arduino-load-cell-hx711/

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files.

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.
*/

// Calibrating the load cell
#include "HX711.h"

#define SizeOfArray(arr)  (sizeof(arr)/sizeof(arr[0]))

const int LOADCELL_DOUT_PIN = D4;
const int LOADCELL_SCK_PIN = D5;

const int SCALE_INTERVAL = 5000;
const int CHECK_READY_AFTER_MS = 1000 * 10;
const int IN_USE_THRESHOLD = 200;
long readings[];
int index = 0;

HX711 scale;
bool initialized = false;
bool in_use = false;

void setup() {
    Serial.begin(115200);
    scale.begin(LOADCELL_DOUT_PIN, LOADCELL_SCK_PIN);
}

void original_loop() {
    if (scale.is_ready()) {
        scale.set_scale();
        Serial.println("Tare... remove any weights from the scale.");
        delay(SCALE_INTERVAL);
        scale.tare();
        Serial.println("Tare done...");
        Serial.print("Place a known weight on the scale...");
        delay(SCALE_INTERVAL);
        long reading = scale.get_units(10);
        Serial.print("Result: ");
        Serial.println(reading);
    }
    else {
        Serial.println("HX711 not found.");
    }
    delay(1000);
}
void loop() {
    if (!scale.is_ready()) {
		Serial.println("HX711 not found.");
		delay(CHECK_READY_AFTER_MS);
		return;
    }
	
    if (!initialized) initialize();
	
	long reading = scale.get_units(10);
	in_use = reading > IN_USE_THRESHOLD || reading < -IN_USE_THRESHOLD;
	
    in_use ? onInUse(reading) : onIdle(reading);
	
}

void initialize() {
    scale.set_scale();
    Serial.println("Remove any weight from the weight...");
    delay(SCALE_INTERVAL);
    scale.tare();
    Serial.println("Initializing complete. Weight is now ready for use.");
    initialized = true;
}
void onInUse(long reading) {
    readings[index] = reading;
    index++;
}
void onIdle(long reading) {
	
}

//calibration factor will be the (reading)/(known weight)