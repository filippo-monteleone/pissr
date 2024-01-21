# pissr
Sistema di gestione di acqua per irrigazioni di colture

## Struttura del progetto
- Backend che espone un'interfaccia di tipo REST e permette di inserire, modificare e cancellare in un database i dati rilevanti sulle risorse idriche disponibili e la loro assegnazione alle aziende, e sulle informazioni 
relative alle aziende ed alle loro coltivazioni
- Gestore Iot, un componente che comunica con i sensori e gli attuatori, invia periodicamente le misure rilevate tramite i sensori e agisce sullo stato 
degli attuatori
- Sensori/Attuatori emulati tramite terminale
