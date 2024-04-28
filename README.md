## Weather Station
<br>RPI Weather Station with wirless sensors.

<br>SOFTWARE:
<br>Main program - C# + AvaloniaUI
<br>Arduino - C/C++
HARDWARE:
<br>The project uses Raspberry Pi 4 as the main unit with touch screen and indoor sensors. 
<br>Additionally, RPi connects to ESP32 via Wi-Fi.

<br>HARDWARE
<br>Indoor sensors: AHT20 + BMP280 + DF Robot CO:   
-Pressure
<br>-Temperature
<br>-Humidity
<br>-CO

<br>Outdoor sensors (Enviro + air quality):
<br>-Temperature
<br>-Pressure
<br>-Humidity
<br>-Light intensity
<br>-CO
<br>-NO2
<br>-DIY rain sensor
<br>-DIY wind sensor

## Roadmap

- [x] Run on RPI
- [X] Connect anlog sensors
- [X] Connect Enviro + air quality to ESP32
- [X] MainScreen
- [ ] SettingsScreen
    - [X] Units
    - [ ] Internet Connection
    - [ ] Appearance
- [X] Wirless ESP32 - RPi communication
- [ ] Data base
- [ ] Plots drawing

MainScreen preview:
![MainScreen](https://github.com/maks3211/WeatherStationRPi/assets/92019474/97da8a5d-1c70-4981-9a4e-68c30eff7fe2)
