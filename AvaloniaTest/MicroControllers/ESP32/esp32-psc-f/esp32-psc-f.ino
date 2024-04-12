#include <Wire.h>
#include <Adafruit_BME280.h>
#include <Adafruit_ADS1X15.h>
#include <Adafruit_I2CDevice.h>
#define SEALEVELPRESSURE_HPA (1013.25)
#define delayTime 3000
Adafruit_BME280 bme; 
Adafruit_ADS1015 ads;



void setup() {
TwoWire I2C = TwoWire(0);
    Serial.begin(9600);
    while(!Serial);   
    Serial.println(F("test enviro+"));

  int status1  = init_559ALS();
  int status2 = bme.begin(0x76);
  int status3 = ads.begin(0x49, &Wire);

    Serial.print("status czujnika swiatla: ");
    Serial.print(status1);
    Serial.print("\nstatus czujnika bme280: ");
    Serial.print(status2);  
    Serial.print("\nstatus czujnika ads1015: ");
    Serial.print(status3); 
    Serial.println();
  }


void loop() { 
    printValues();
    print_ALS();
    print_ADS();
    Serial.print("\n");

    delay(delayTime);
}

void print_ADS()
{
  for (int i =0; i<3;i+=1)
  print_normalized_ADS(i);
}

void print_normalized_ADS(uint8_t nr){//odczyt wartości MICS-6814

  int voltage;
  if(GAIN_TWOTHIRDS==ads.getGain()) voltage = 6144;
  if(GAIN_ONE==ads.getGain()) voltage = 4096;
  if(GAIN_TWO==ads.getGain()) voltage = 2048;
  if(GAIN_FOUR==ads.getGain()) voltage = 1024;
  if(GAIN_EIGHT==ads.getGain()) voltage = 512;
  if(GAIN_SIXTEEN==ads.getGain()) voltage = 256;
  
if (nr==0) Serial.print("\nNO2 value = ");
else if (nr==1) Serial.print("\nCO value = ");
else if (nr==2) Serial.print("\nNH3 value = ");
Serial.print((ads.readADC_SingleEnded(nr)/2047.0*voltage)/1000);
Serial.print("ppm");
}

int init_559ALS(){//inicjalizacja czujnika światła
   delay(200);
  Wire.begin();
  Wire.beginTransmission(0x23);
  Wire.write(0x80);
  Wire.write(0b00000001);
  delay(20); 
  return Wire.endTransmission();
}


void print_ALS(){//odczyt czujnika światła
  Wire.begin();
  Wire.beginTransmission(0x23);
  Wire.write(0x88);
  Wire.endTransmission(false);
  Wire.requestFrom(0x23, 1);
  int L_ch1 = Wire.read();//odczyt dolnego bajtu kanału 1 czujnika światła
  Wire.endTransmission();
  
  Wire.begin();
  Wire.beginTransmission(0x23);
  Wire.write(0x89);
  Wire.endTransmission(false);
  Wire.requestFrom(0x23, 1);
  int H_ch1 = Wire.read();//odczyt górnego bajtu kanału 1 czujnika światła
  Wire.endTransmission();

    Wire.begin();
    Wire.beginTransmission(0x23);
    Wire.write(0x8A);
    Wire.endTransmission(false);
    Wire.requestFrom(0x23, 1);
    int L_ch2 = Wire.read();//odczyt dolnego bajtu kanału 2 czujnika światła
    Wire.endTransmission();
    
    Wire.begin();
    Wire.beginTransmission(0x23);
    Wire.write(0x8B);
    Wire.endTransmission(false);
    Wire.requestFrom(0x23, 1);
    int H_ch2 = Wire.read();//odczyt górnego bajtu kanału 2 czujnika światła
    Wire.endTransmission();

  Serial.print("\nCH1 - iluminance = ");
  Serial.print(H_ch1<<8|L_ch1);//złączenie bajtów i wypisanie wyniku
  Serial.print(" LUX");
  Serial.print("\nCH2 - iluminance = ");
  Serial.print(H_ch2<<8|L_ch2);//złączenie bajtów i wypisanie wyniku
  Serial.print(" LUX");
}

void printValues() {//wypisanie wartości czujnika BME
    Serial.print("\nTemperature = ");
    Serial.print(bme.readTemperature());
    Serial.print(" °C");

    Serial.print("Pressure = ");

    Serial.print(bme.readPressure() / 100.0F);
    Serial.print(" hPa");

    Serial.print("\nApprox. Altitude = ");
    Serial.print(bme.readAltitude(SEALEVELPRESSURE_HPA));
    Serial.print(" m");

    Serial.print("\nHumidity = ");
    Serial.print(bme.readHumidity());
    Serial.print(" %");
}
