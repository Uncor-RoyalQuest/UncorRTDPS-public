# UncorRTDPS - damage per second in Royal Quest game  
Disclaimer: this code is not an example of good design or proper WPF usage</br>

## Structure
* **UncorRTDPS** folder - core dll project</br>
* **UncorRTDPS standalone** folder - starter project for UncorRTDPS.dll</br>
* **Resources** folder - resources needed for the program to run (config, localization, mobs, etc.)</br>

Post on the official Royal Quest game forum: https://www.royalquest.ru/forum/index.php?showtopic=43506

## Start UncorRTDPS.dll
```c#
//Absolute path to the "Resources" folder
string absolutePathToResources = @"C:\MyPath";
//Supported languages: "en", "ru"
string uiLanguage = "en";
//Start all services and other basic init
UncorRTDPS.UncorRTDPS_Starter.InitUncorRTDPS(absolutePathToResources, uiLanguage);
//Start UncorRTDPS
new UncorRTDPS.UncorRTDPS_Windows.StatsHoveringWindow().Show();
```
