# AA22-23-Gruppo05

## Come avviare:

- Visual studio
- Mosquitto

Le cartelle AttuatoreGui, GestoreIot, RestApi-Pissr e Sensore contengono tutti un solution file abbreviato sln. <br> Semplicemente basta cliccare su quel file e verra' aperto il progetto.

**Attenzione:** bisogna fare quel passaggio con ogni cartella, quindi si dovra' avere 4 istanze di visual studio aperte.

Le dipendenze nuget dovrebbero essere scaricate automaticamente da vs 2022. <br> Se non dovesse importare i nuget bisogna aprire la Console e scrivere `nuget restore <projectPath> ` per esempio nel caso del gestoreIot faccio `nuget restore GestoreIot.sln`

**Attenzione** per connettersi bisogna sostituire nel file appsettings la property: DefaultConnection con il proprio Host, porta, username e password.

### Cambiare azienda agricola al gestore iot

Di default il gestore iot ascolta l'azienda con id 1. E' possibile cambiarlo su visual studio andando a cambiare la proprieta' `comandLineArgs` in launchSettings.json, oppure quando si crea l'exe aggiungendo come parametro l'id dell'azienda cosi' `GestoreIot.exe 2`.

### Creare exe

Su visual studio andare su solution explorer e fare click dx sulla soluzione e poi fare Build Solution. <br>
Una volta buildato correttamente troverete l'exe in bin -> Debug -> net -> .exe <br>
Per quanto riguarda a windows basta andare sulla cartella con il cmd e scrivere per esempio `GestoreIot.exe`

## Documentazione:

https://docs.google.com/document/d/1EkAa2XbWhmkUXCMXjBT-fJeWm0ywWvzWNQ3VeMhOZf4/edit?usp=sharing

## Dump del DB

Il db e' fatto con postgresql e si chiama Pissr.sql.

## Comandi utili per testing

### Cancellare le tabelle e resettare l'id

```sql
    TRUNCATE "AziendeAgricole" RESTART IDENTITY CASCADE;
    TRUNCATE "AziendeIdriche" RESTART IDENTITY CASCADE;
    TRUNCATE "Campi" RESTART IDENTITY CASCADE;
    TRUNCATE "Contratti" RESTART IDENTITY CASCADE;
    TRUNCATE "Dispositivi" RESTART IDENTITY CASCADE;
    TRUNCATE "Eventi" RESTART IDENTITY CASCADE;
```
