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

// Weight module
#include "HX711.h"

const double CALIBRATION_FACTOR = -459.542;

HX711 scale;
bool shouldInitialize = false;
bool initialized = false;
bool inUse = false;

// WiFi modules
#include <ESP8266HTTPClient.h>
#include <ESP8266WiFi.h>
#include <WifiClient.h>

WiFiClient client;
HTTPClient http;
const String API_URL = "https://192.168.1.104:45455/api/measurements/partials";

// Date & time modules
#include <chrono>
#include <Ticker.h>

Ticker initializeTimer;

// Hardware setup
#define SizeOfArray(arr)  (sizeof(arr)/sizeof(arr[0]))
void ICACHE_RAM_ATTR HandleExternalInterrupt();

const int LOADCELL_DOUT_PIN = D4;
const int LOADCELL_SCK_PIN = D5;
const int BUTTON = D7;
const int WeightId = 1;

// TODO: Save all weights in String[] to then later post as collcetion

void HandleExternalInterrupt() {
    Serial.println("Pressed");
    
    if (!initialized && !shouldInitialize) {
        shouldInitialize = true;
        return;
    }

    inUse = true;
}

void setup() {
    scale.set_scale(CALIBRATION_FACTOR);
	
    pinMode(BUTTON, INPUT);
    attachInterrupt(digitalPinToInterrupt(BUTTON), HandleExternalInterrupt, RISING);

    Serial.begin(115200);
    Serial.println("Booting");
	
    scale.begin(LOADCELL_DOUT_PIN, LOADCELL_SCK_PIN);
    Serial.println("Scale began");
    
    WiFi.begin("h5pd091122", "h5pd091122_styrer");
    Serial.println("Wifi began");

    Serial.print(F("Connecting to WiFi"));
    while (WiFi.status() != WL_CONNECTED) // Wait for WiFI connection
    {
        delay(500);
        Serial.print(F("."));
    }
	Serial.println(F("\nConnected to the WiFi network"));
}

void loop() {
    if (!scale.is_ready() // Waiting for scale to be ready
        || !shouldInitialize) return; // Waiting for User to initialize weight
	
	else if (!initialized && shouldInitialize) { // Initialize weight and return out of loop
        initialize();
        return;
    }
	
    // Waiting for user to use weight and press button
    if (!inUse) return;

    Serial.println("Now in use");

	// User is using weight
	float units = scale.get_units(10);
    double value = scale.get_value(10);
    long read = scale.read();
    long read_average = scale.read_average(10);
    
	Serial.println(
        "Units: " + String(units) + 
        "\nValue: " + String(value) + 
        "\nRead: " + String(read) + 
        "\nAverage: " + String(read_average)
    );

    if (WiFi.status() == WL_CONNECTED) PostWeight(units);
    else Serial.println("Error in WiFi connection. Status is " + String(WiFi.status()));

    initialized = false;
    shouldInitialize = false;
    inUse = false;
}

void initialize() {
    scale.set_scale();
    //Serial.println("Remove any weight from the weight...");
	Serial.println("Initializing scale...");
    //delay(SCALE_INTERVAL);
    scale.tare();
    Serial.println("Initializing complete. Weight is now ready for use.");
	
    initialized = true;
    //delay(SCALE_INTERVAL);
}

String GetDate() {
    time_t timestamp = time(0) * 1000;
    tm* ltm = localtime(&timestamp);
    String date = (
        String(ltm->tm_year) + "-" +
        String(ltm->tm_mon) + "-" +
        String(ltm->tm_mday) + "T" +
        String(ltm->tm_hour) + ":" +
        String(ltm->tm_min) + ":" +
        String(ltm->tm_sec) + "." +
        String(millis()) + "Z"
    );
	return date;
}
void PostWeight(int value) {
    http.begin(client, API_URL); // Request destination
    http.addHeader("Content-Type", "application/json"); // Request content-type header
    String content = (
    "{" +
        String("\"WeightId\": ") + String(WeightId) + ", " +
		//String("\"UserId\": ") + String("null") + ", " +
		String("\"Value\": \"") + String(value) + "\", " +
        String("\"Date\": ") + GetDate() +
    "}");
    Serial.println(content);

    int httpCode = http.POST(content); // Send the request
    String payload = http.getString(); // Get response
    http.end(); // Close connection
	
	Serial.println("[" + String(httpCode) + "] " + payload);
}

//calibration factor will be the (reading)/(known weight)