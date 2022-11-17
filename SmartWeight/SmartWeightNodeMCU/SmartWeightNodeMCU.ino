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

Ticker displayResetTimer;

// Display
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <Wire.h>

constexpr auto DISPLAY_I2CBUS_ADDRESS = 0x3C;
constexpr auto SCREEN_WIDTH = 128; // OLED display width, in pixels;
constexpr auto SCREEN_HEIGHT = 64; // OLED display height, in pixels;
#define OLED_RESET    LED_BUILTIN // Reset pin # (or -1 if sharing Arduino reset pin)

Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);

// Hardware setup
void IRAM_ATTR HandleExternalInterrupt();

const uint8_t LOADCELL_DOUT_PIN = D4;
const uint8_t LOADCELL_SCK_PIN = D5;
const uint8_t BUTTON = D6;
const uint8_t RED_LED = D7;
const uint8_t GREEN_LED = D8;

const uint8_t WeightId = 1;

// TODO: Save all weights in String[] to then later post as collcetion

void HandleExternalInterrupt() {
    Serial.println("States:\n\t" + 
        String("shouldInitialize: ") + String(shouldInitialize) + 
        "\n\tinitialized: " + String(initialized) + 
        "\n\tinUse: " + String(inUse)
    );
    
    if (!initialized) {
        Serial.println("shouldInitialize: true");
        shouldInitialize = true;
        return;
    }

    Serial.println("inUse: true");
    inUse = true;
}
void HandleDisplayResetTimer() {
    display.begin(SSD1306_SWITCHCAPVCC, DISPLAY_I2CBUS_ADDRESS);
	display.clearDisplay();
    display.setCursor(30, 16);
    display.println("SmartWeight");
	display.display();
}

void ConnectToWifi() {
    WiFi.begin("h5pd091122", "h5pd091122_styrer");

    printToDisplay("Connecting to WiFi");
    while (WiFi.status() != WL_CONNECTED) // Wait for WiFI connection
    {
        delay(500);
        Serial.print(".");
    }
    //Serial.println("\nConnected to the WiFi network");
	printToDisplay("\nConnected to " + WiFi.SSID());
}

void setup() {
    Serial.begin(115200);

    scale.set_scale(CALIBRATION_FACTOR);
    scale.begin(LOADCELL_DOUT_PIN, LOADCELL_SCK_PIN);

	pinMode(RED_LED, OUTPUT);
	pinMode(GREEN_LED, OUTPUT);
	
    pinMode(BUTTON, INPUT);
    attachInterrupt(digitalPinToInterrupt(BUTTON), HandleExternalInterrupt, RISING);
    
    printToDisplay("Booting...");
    delay(200);

    ConnectToWifi();
    reset();
}

void loop() {
    // Waiting for scale to be ready and for User to initiate default weight
    if (!shouldInitialize) return;
    
    // Initialize weight and return out of loop
	if (!initialized && shouldInitialize) { 
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
    printToDisplay(String(units));

    if (WiFi.status() == WL_CONNECTED) PostWeight(units);
    else Serial.println("Error in WiFi connection. Status is " + String(WiFi.status()));

    reset();
}

void initialize() {
    scale.begin(LOADCELL_DOUT_PIN, LOADCELL_SCK_PIN);

    if (!scale.is_ready()) {
        printToDisplay("Scale is not ready yet.");
        shouldInitialize = false;
        return;
    }

    printToDisplay("Initializing scale...");
    scale.set_scale(CALIBRATION_FACTOR);
	
    printToDisplay("Waiting to tare...");
    delay(5000);
	printToDisplay("Taring scale...");
    scale.tare();
    printToDisplay("Scale tared, continuing...");
	
    initialized = true;
	
    digitalWrite(RED_LED, LOW);
    digitalWrite(GREEN_LED, HIGH);
    printToDisplay("Ready");
	
    //delay(SCALE_INTERVAL);
}
void reset() {
    Serial.println("Resetting");
    initialized = false;
    shouldInitialize = false;
    inUse = false;
	
    digitalWrite(RED_LED, HIGH);
    digitalWrite(GREEN_LED, LOW);
    printToDisplay("SmartWeight");
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
    //displayResetTimer.once(1, HandleDisplayResetTimer);
}

void printToDisplay(const String value) {
    Serial.println("Display: " + value);
    display.begin(SSD1306_SWITCHCAPVCC, DISPLAY_I2CBUS_ADDRESS);
    display.clearDisplay();
    display.setCursor(8, 8);
    display.setTextSize(2);
    display.setTextColor(WHITE);
	
	//display.setCursor(SCREEN_WIDTH / 2 - value.length() * 2.5, SCREEN_HEIGHT / 4);
    display.println(value);
    display.display();
}
//calibration factor will be the (reading)/(known weight)