//PIN TX DO PINU RX W RPI+ WSPÓLNA MASA
#include <Wire.h>
#include <Adafruit_BMP280.h>
#include <AHT20.h>
//TEMPERATURA + WYLGOTNOSC
AHT20 aht20;

//Cisnienie
Adafruit_BMP280 bmp; 
//#define INDOOR_CO_SENSOR A0  //niebieski - output czerwony 5v czarny gnd
//#define INDOOR_CO_SENSOR_OUTPUT 2

//int sensor_co_indoor; 
bool aht20Work = true;
bool bmpWork = true;
//char dataString[50] = {0};
//int a = 0;
void setup() {
  
  // put your setup code here, to run once:
  Serial.begin(9600);
  Wire.begin();
  if (aht20.begin() == !aht20Work)
  {
    Serial.println("AHT20 not detected. Please check wiring. Freezing.");
    aht20Work = false;
  }
  Serial.println("AHT20 acknowledged.");

   
   bmpWork = bmp.begin();
    if (!bmpWork) {
    Serial.println(F("Could not find a valid BMP280 sensor, check wiring or "
                      "try a different address!"));
    Serial.print("SensorID was: 0x"); Serial.println(bmp.sensorID(),16);
    Serial.print("        ID of 0xFF probably means a bad address, a BMP 180 or BMP 085\n");
    Serial.print("   ID of 0x56-0x58 represents a BMP 280,\n");
    Serial.print("        ID of 0x60 represents a BME 280.\n");
    Serial.print("        ID of 0x61 represents a BME 680.\n");
    bmpWork = false;
  }
 bmp.setSampling(Adafruit_BMP280::MODE_NORMAL,     /* Operating Mode. */
                  Adafruit_BMP280::SAMPLING_X2,     /* Temp. oversampling */
                  Adafruit_BMP280::SAMPLING_X16,    /* Pressure oversampling */
                  Adafruit_BMP280::FILTER_X16,      /* Filtering. */
                  Adafruit_BMP280::STANDBY_MS_500); /* Standby time. */
 Serial.println("BMP280 acknowledged.");
 // pinMode(INDOOR_CO_SENSOR, INPUT);

   
  // sensor_co_indoor = analogRead(INDOOR_CO_SENSOR);
   //pinMode(INDOOR_CO_SENSOR_OUTPUT, OUTPUT);

}

void loop() {
float aht20temperature = 0.0;
float bmeTemperature = 0.0;
float humidity = 0.0;
float preassure = 0.0;
float altitude = 0.0;
  if (aht20.available() == true)
  {
  aht20temperature = aht20.getTemperature();
  bmeTemperature = bmp.readTemperature();
  humidity = aht20.getHumidity();
  preassure = bmp.readPressure()/100.00;
  altitude = bmp.readAltitude(1013.25);
  float avgTemperature = (aht20temperature + bmeTemperature)/2;  
  String temperatureString =String(avgTemperature) + "-C";
  Serial.println(temperatureString +"<"+ String(humidity)+"-%<" + String(preassure)+"-hPa<" + String(altitude)+"-m");
  }
  // put your main code here, to run repeatedly:
  //sensor_co_indoor = analogRead(INDOOR_CO_SENSOR);
 // Serial.println("");
   //Serial.println("Czujnik: 32.0C");
 //Serial.print(sensor_co_indoor);
 
//Serial.println(sensor_co_indoor, DEC); 
 /* if (sensor_co_indoor > 500) {
    digitalWrite(INDOOR_CO_SENSOR_OUTPUT, HIGH); // Ustawienie stanu wysokiego na wyjściu
  } else {
    digitalWrite(INDOOR_CO_SENSOR_OUTPUT, LOW); // Ustawienie stanu niskiego na wyjściu
  }

Serial.print(sensor_co_indoor);
Serial.print('\n');
*/

delay(5000);
}
