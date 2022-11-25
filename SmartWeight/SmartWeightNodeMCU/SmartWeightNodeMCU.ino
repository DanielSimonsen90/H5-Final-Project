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
bool ready = false;

// WiFi modules
#include <ESP8266HTTPClient.h>
#include <ESP8266WiFi.h>
#include <WifiClient.h>

WiFiClient client;
HTTPClient http;
const String API_URL = "http://192.168.1.24:45455/api/measurements/partials";

// Date & time modules
#include <chrono>
#include <Ticker.h>
#include <ctime>

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
const uint8_t BUTTON = D6;

const uint8_t WeightId = 1;
const int WaitTimeMS = 2000;

//const String Wifi_SSID = "h5pd091122";
//const String Wifi_Password = "h5pd091122_styrer";
const String Wifi_SSID = "Zyxel_6551";
const String Wifi_Password = "FUXXUAKMCG";

// TODO: Save all weights in String[] to then later post as collcetion

void HandleExternalInterrupt() {
    if (!ready) {
        ready = true;
        return;
    }

}

void ConnectToWifi() {
    WiFi.begin(Wifi_SSID, Wifi_Password);

    printToDisplay("Connecting to WiFi");
    while (WiFi.status() != WL_CONNECTED) // Wait for WiFI connection
    {
        delay(500);
        //Serial.print(".");
        Serial.println(WiFi.status());
    }
	printToDisplay("Connected to " + WiFi.SSID());
}
float GetRandomWeight() {
	return random(50, 90);
}

void setup() {
    Serial.begin(115200);

    pinMode(BUTTON, INPUT);
    attachInterrupt(digitalPinToInterrupt(BUTTON), HandleExternalInterrupt, RISING);

    delay(200);

    ConnectToWifi();
    reset();
}
void loop() {
	// Waiting for scale to be ready and for User to initiate default weight
	if (!ready) return;

	// User is using weight
    double value = GetRandomWeight();

	printToDisplay(String(value));

	if (WiFi.status() == WL_CONNECTED) PostWeight(value);
	else Serial.println("Error in WiFi connection. Status is " + String(WiFi.status()));

    reset();
}

void reset() {
    Serial.println("Resetting");
    ready = false;
	
    delay(1000);
    printToDisplay("SmartWeight");
}

void PostWeight(double value) {
    Serial.println("Posting value: " + String(value));

    http.begin(client, API_URL); // Request destination
    http.addHeader("Content-Type", "application/json"); // Request content-type header
    String content = (
    "{\n" +
        String("   \"WeightId\": ") + String(WeightId) + ",\n" +
		String("   \"Value\": ") + String(value) + "\n"
    "}");
    Serial.println(content);
    delay(200);

    printToDisplay("Posting..");
    int httpCode = http.POST(content); // Send the request
    String payload = http.getString(); // Get response
    
    printToDisplay(String(httpCode));
	Serial.println("[" + String(httpCode) + "] " + payload);
	
    http.end(); // Close connection
}

void printToDisplay(const String value) {
    Serial.println("Display: " + value);
    display.begin(SSD1306_SWITCHCAPVCC, DISPLAY_I2CBUS_ADDRESS);
    display.clearDisplay();
    display.setCursor(4, 16);
    display.setTextSize(2);
    display.setTextColor(WHITE);
	
    display.println(value);
    display.display();
}
