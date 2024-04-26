//INTERNET
#include <WiFi.h>
#include<WiFiClientSecure.h>
#include <PubSubClient.h>
#include <Wire.h>


//CZUJNIKI
#include <Adafruit_BME280.h>
#include <Adafruit_ADS1X15.h>
#include <Adafruit_I2CDevice.h>
#define SEALEVELPRESSURE_HPA (1013.25)
#define delayTime 30000
Adafruit_BME280 bme; 
Adafruit_ADS1015 ads;



//KONFIGURACJA INTERNETU/MQTT
const char* ssid = "TP-Link_0E21";
const char* password = "66275022";

//const char* mqtt_server = "192.168.0.91";
const char* mqtt_server = "7b21c793398043ab8fbde110f0ebc243.s1.eu.hivemq.cloud";
const char* mqtt_username = "espSensors";
const char* mqtt_password = "testuseR1";
const int mqtt_port = 8883;

WiFiClientSecure espClient;
PubSubClient client(espClient);
long lastMsg = 0;

char msg[50];
int value = 0;
int counter = 0;

static const char* root_ca = R"EOF(
  -----BEGIN CERTIFICATE-----
MIIFazCCA1OgAwIBAgIRAIIQz7DSQONZRGPgu2OCiwAwDQYJKoZIhvcNAQELBQAw
TzELMAkGA1UEBhMCVVMxKTAnBgNVBAoTIEludGVybmV0IFNlY3VyaXR5IFJlc2Vh
cmNoIEdyb3VwMRUwEwYDVQQDEwxJU1JHIFJvb3QgWDEwHhcNMTUwNjA0MTEwNDM4
WhcNMzUwNjA0MTEwNDM4WjBPMQswCQYDVQQGEwJVUzEpMCcGA1UEChMgSW50ZXJu
ZXQgU2VjdXJpdHkgUmVzZWFyY2ggR3JvdXAxFTATBgNVBAMTDElTUkcgUm9vdCBY
MTCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAK3oJHP0FDfzm54rVygc
h77ct984kIxuPOZXoHj3dcKi/vVqbvYATyjb3miGbESTtrFj/RQSa78f0uoxmyF+
0TM8ukj13Xnfs7j/EvEhmkvBioZxaUpmZmyPfjxwv60pIgbz5MDmgK7iS4+3mX6U
A5/TR5d8mUgjU+g4rk8Kb4Mu0UlXjIB0ttov0DiNewNwIRt18jA8+o+u3dpjq+sW
T8KOEUt+zwvo/7V3LvSye0rgTBIlDHCNAymg4VMk7BPZ7hm/ELNKjD+Jo2FR3qyH
B5T0Y3HsLuJvW5iB4YlcNHlsdu87kGJ55tukmi8mxdAQ4Q7e2RCOFvu396j3x+UC
B5iPNgiV5+I3lg02dZ77DnKxHZu8A/lJBdiB3QW0KtZB6awBdpUKD9jf1b0SHzUv
KBds0pjBqAlkd25HN7rOrFleaJ1/ctaJxQZBKT5ZPt0m9STJEadao0xAH0ahmbWn
OlFuhjuefXKnEgV4We0+UXgVCwOPjdAvBbI+e0ocS3MFEvzG6uBQE3xDk3SzynTn
jh8BCNAw1FtxNrQHusEwMFxIt4I7mKZ9YIqioymCzLq9gwQbooMDQaHWBfEbwrbw
qHyGO0aoSCqI3Haadr8faqU9GY/rOPNk3sgrDQoo//fb4hVC1CLQJ13hef4Y53CI
rU7m2Ys6xt0nUW7/vGT1M0NPAgMBAAGjQjBAMA4GA1UdDwEB/wQEAwIBBjAPBgNV
HRMBAf8EBTADAQH/MB0GA1UdDgQWBBR5tFnme7bl5AFzgAiIyBpY9umbbjANBgkq
hkiG9w0BAQsFAAOCAgEAVR9YqbyyqFDQDLHYGmkgJykIrGF1XIpu+ILlaS/V9lZL
ubhzEFnTIZd+50xx+7LSYK05qAvqFyFWhfFQDlnrzuBZ6brJFe+GnY+EgPbk6ZGQ
3BebYhtF8GaV0nxvwuo77x/Py9auJ/GpsMiu/X1+mvoiBOv/2X/qkSsisRcOj/KK
NFtY2PwByVS5uCbMiogziUwthDyC3+6WVwW6LLv3xLfHTjuCvjHIInNzktHCgKQ5
ORAzI4JMPJ+GslWYHb4phowim57iaztXOoJwTdwJx4nLCgdNbOhdjsnvzqvHu7Ur
TkXWStAmzOVyyghqpZXjFaH3pO3JLF+l+/+sKAIuvtd7u+Nxe5AW0wdeRlN8NwdC
jNPElpzVmbUq4JUagEiuTDkHzsxHpFKVK7q4+63SM1N95R1NbdWhscdCb+ZAJzVc
oyi3B43njTOQ5yOf+1CceWxG1bQVs5ZufpsMljq4Ui0/1lvh+wjChP4kqKOJ2qxq
4RgqsahDYVvTH9w7jXbyLeiNdd8XM2w9U/t7y0Ff/9yi0GE44Za4rF2LN9d11TPA
mRGunUHBcnWEvgJBQl9nJEiU0Zsnvgc/ubhPgXRR4Xq37Z0j4r7g1SgEEzwxA57d
emyPxgcYxn/eR44/KJ4EBs+lVDR3veyJm+kXQ99b21/+jh5Xos1AnX5iItreGCc=
-----END CERTIFICATE-----
)EOF";



void setup() {
  Serial.print("Setup ");
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

  setup_wifi();
  espClient.setCACert(root_ca);
  client.setServer(mqtt_server, mqtt_port);
  client.setCallback(callback);
}

void loop() {
   if (!client.connected()) 
   {
    reconnect();
   }
   client.loop();
  long now = millis();
  if (now - lastMsg > 5000) 
  {
    lastMsg = now;
    Serial.print("WYSYLANIE: ");
   // client.publish("testy", "drugi publish" );
    counter = counter +1;
  }

    printValues();
    print_ALS();
    print_ADS();
    Serial.print("\n");

    delay(delayTime);

}




void setup_wifi() {
  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

void callback(char* topic, byte* message, unsigned int length) {
  Serial.print("Message arrived on topic: ");
  Serial.print(topic);
  Serial.print(". Message: ");
  String messageTemp;
  
  for (int i = 0; i < length; i++) {
    Serial.print((char)message[i]);
    messageTemp += (char)message[i];
  }
  Serial.println();
}

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
     String clientId = "ESP32Client";
    Serial.print("Attempting MQTT connection...");
    // Attempt to connect
   // if (client.connect("ESP8266Client")) {
     if (client.connect("ESP32Client",mqtt_username, mqtt_password)) {
      Serial.println("connected");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}








void  print_ADS()
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
   

if (nr==0) 
{
  char tempChar[15];
  sprintf(tempChar, "%.2f", (ads.readADC_SingleEnded(nr)/2047.0*voltage)/1000);
  client.publish("outdoorno2", tempChar);
  Serial.print("\nNO2 value = ");
}
else if (nr==1) 
{
  char tempChar[15];
  sprintf(tempChar, "%.2f", (ads.readADC_SingleEnded(nr)/2047.0*voltage)/1000);
  client.publish("outdoorco", tempChar);
  Serial.print("\nCO value = ");
}
else if (nr==2)
{
  char tempChar[15];
  sprintf(tempChar, "%.2f", (ads.readADC_SingleEnded(nr)/2047.0*voltage)/1000);
  client.publish("outdoornh3", tempChar);
   Serial.print("\nNH3 value = ");
}
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

Serial.print("  Moja jasnosc:  ");
 Serial.print( ((H_ch2<<8|L_ch2) + (H_ch1<<8|L_ch1) )/ 2);
 float ilu = (   (H_ch2<<8|L_ch2) + (H_ch1<<8|L_ch1) )/ 2;
  Serial.print("----------");
 Serial.print(ilu);
   Serial.print("----------");
  char tempChar[15];
  sprintf(tempChar, "%.2f", ilu);
  client.publish("outdooriluminance", tempChar);
}

void printValues() {//wypisanie wartości czujnika BME
    Serial.print("\nTemperature = ");
    Serial.print(bme.readTemperature());
    Serial.print(" °C");

    char tempChar[15];
    sprintf(tempChar, "%.2f", bme.readTemperature());
    client.publish("outdoortemperature", tempChar);


    Serial.print("\nPressure = ");
    Serial.print(bme.readPressure() / 100.0F);
    Serial.print(" hPa");

    memset(tempChar, 0, sizeof(tempChar)); 
    sprintf(tempChar, "%.2f", bme.readPressure() / 100.0F);
    client.publish("outdoorpreasure", tempChar);

    
    Serial.print("\nApprox. Altitude = ");
    Serial.print(bme.readAltitude(SEALEVELPRESSURE_HPA));
    Serial.print(" m");

    memset(tempChar, 0, sizeof(tempChar)); 
    sprintf(tempChar, "%.2f", bme.readAltitude(SEALEVELPRESSURE_HPA));
    client.publish("outdooraltitude", tempChar);


    Serial.print("\nHumidity = ");
    Serial.print(bme.readHumidity());
    Serial.print(" %");

    memset(tempChar, 0, sizeof(tempChar)); 
    sprintf(tempChar, "%.2f", bme.readHumidity());
    client.publish("outdoornhumidity", tempChar);
}





