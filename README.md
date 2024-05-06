<h1 align="center" style="font-weight: bold;">RPI Weather Station with wirless sensors</h1>



<h2 id="software">💻Software</h2>

- Main program - C# + AvaloniaUI
- Arduino - C/C++
<h2 id="hardware">⚙️Hardware</h2>

The project uses Raspberry Pi 4 as the main unit with touch screen and indoor sensors.
Additionally, RPi connects to ESP32 via Wi-Fi.
<br>Indoor sensors: AHT20 + BMP280 + DF Robot CO:   
-Pressure
<br> -Temperature
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
<h2 id="roadmap">🎯Roadmap</h2>

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

<h2 id="colab">🤝 Collaborators</h2>
<table>
<tr>

<td align="center">
<a href="https://github.com/koob7">
<sub>
<b>Konrad Kobielus</b>
</sub>
</a>
</td>
</tr>
</table>

MainScreen preview:
![MainScreen](https://github.com/maks3211/WeatherStationRPi/assets/92019474/97da8a5d-1c70-4981-9a4e-68c30eff7fe2)
