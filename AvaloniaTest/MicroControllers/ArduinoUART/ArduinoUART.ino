//PIN TX DO PINU RX W RPI+ WSPÓLNA MASA
#include <Wire.h>
#include <Adafruit_BMP280.h>
#include <AHT20.h>
//TEMPERATURA + WYLGOTNOSC
AHT20 aht20;

//Cisnienie
Adafruit_BMP280 bmp; 
#define INDOOR_CO_SENSOR A0  //niebieski - output,   czerwony-5v,  czarny-gnd
#define coefficient_A 19.32
#define coefficient_B -0.64
#define R_Load 10.0
//#define INDOOR_CO_SENSOR_OUTPUT 2
class MQ7 {
	private:
		uint8_t analogPin;
		float v_in;
		float voltageConversion(int);
	public:
		MQ7(uint8_t, float);
		float getPPM();
		float getSensorResistance();
		float getRatio();
};

MQ7 mq7(A0,5.0);

int sensor_co_indoor; 
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
  pinMode(INDOOR_CO_SENSOR, INPUT);

   
   sensor_co_indoor = analogRead(INDOOR_CO_SENSOR);
  // pinMode(INDOOR_CO_SENSOR_OUTPUT, OUTPUT);

}

void loop() {
float aht20temperature = 0.0;
float bmeTemperature = 0.0;
float humidity = 0.0;
float preassure = 0.0;
float altitude = 0.0;
float co = 0.0;
  if (aht20.available() == true)
  {
  aht20temperature = aht20.getTemperature();
  bmeTemperature = bmp.readTemperature();
  humidity = aht20.getHumidity();
  preassure = bmp.readPressure()/100.00;
  altitude = bmp.readAltitude(1013.25);
  co = mq7.getPPM();
  float avgTemperature = (aht20temperature + bmeTemperature)/2;  
  String temperatureString =String(avgTemperature) + "-C";
  Serial.println(temperatureString +"<"+ String(humidity)+"-%<" + String(preassure)+"-hPa<" + String(altitude)+"-m<" + String(co)+"-ppm");
  }
  // put your main code here, to run repeatedly:
  sensor_co_indoor = analogRead(INDOOR_CO_SENSOR);
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
//  CZYTANIE CO
//Serial.println(mq7.getPPM());

delay(5000);
}



MQ7::MQ7(uint8_t pin, float v_input){
  analogPin = pin;
  v_in = v_input;
}
float MQ7::getPPM(){
  return (float)(coefficient_A * pow(getRatio(), coefficient_B));
}
float MQ7::voltageConversion(int value){
  return (float) value * (v_in / 1023.0);
}
float MQ7::getRatio(){
  int value = analogRead(analogPin);
  float v_out = voltageConversion(value);
  return (v_in - v_out) / v_out;
}
float MQ7::getSensorResistance(){
  return R_Load * getRatio();
}