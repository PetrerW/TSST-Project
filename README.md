# TSST-Project
An emulator of the internet traffic and EON tunneling.
# Topologia
![Topologia](https://github.com/PetrerW/TSST-Project/blob/master/Topologia.PNG)
# Kolejność uruchamiania komponentów
Uruchamiamy programy ze ścieżki `<projekt>\bin\Debug`, gdzie `<projekt>` to nazwa projektu z Visual Studio, np. `ClientNode`.
## 1. CableCloud
Po prostu włączyć program `CableCloud.exe`.
## 2. Routery
Włączyć 7 programów z routerami (`NetworkNode.exe`) i wpisać na każdym liczbę, po kolei od 1 do 7. 
Poniżej adresy IP poszczególnych routerów:
1. Router 1 - IP 127.0.0.3
2. Router 2 - IP 127.0.0.5
3. Router 3 - IP 127.0.0.7
4. Router 4 - IP 127.0.0.31
5. Router 5 - IP 127.0.0.33
6. Router 6 - IP 127.0.0.35
7. Router 7 - IP 127.0.0.37
## 3. Podsieci
Włączyć 3 programy `SubNetwork.exe`. Wpisujemy w nich odpowiednio liczby 1, 2 i 5. 
## 4. Programy klienckie
Włączyć 3 programy `ClientNode.exe`. Wpisujemy do nich adresy IP: 127.0.0.2, 127.0.0.4, 127.0.0.6.
## 5. Network Call Controller
Włączyć 2 programy `NCC.exe`. Wpisać do konsoli odpowiednio 1 i 2.
# Uwagi
To jest ścisła kolejność, w której emulator działa i **zmienienie tej kolejności spowoduje, że emulator nie będzie działał poprawnie!**
