Program do kopiowania plików z Windowsa na RPi 
https://ninite.com/winscp/

login: pi
haslo: raspberry

Uruchamianie programu na RPi:
Na windowsie uruchomić cmd tam gdzie jest plik .sln
Wpisać komende:
dotnet publish -r linux-arm
Skopiować plik linux-arm z  AvaloniaTest/bin/Release/net8.0 do RPi- najlepiej na pulpit
Na Rpi wejść w skopiowany folder wejść w Tools-> Open Current Folder in Terminal
Wpisać komende dotnet AvaloniaTest.dll


Połączenie Arduino do RPi:
GND->GND
RX(Arduino) -> TX(RPi)
TX(Arduino)->RX(RPi)

Czujnik BME (temp, wilgoć ciśnienie):
  !!  3.3V

