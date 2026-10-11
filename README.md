# 2D Platformer 🎮

Felfelé törekvő, csúzli-mechanikás 2D ügyességi játék mobilra, Unityben és C# nyelven.
Saját hobbiprojekt, folyamatos fejlesztés alatt.

![Játékmenet](docs/gameplay.gif)

## ▶️ Kipróbálás

**Böngészőben, letöltés nélkül:** [meopleo.itch.io/2d-endless-platformer](https://meopleo.itch.io/2d-endless-platformer)

## A játékról

A karaktert csúzliként lehet kilőni: húzd meg az egeret vagy az ujjad, engedd el, és a játékos a húzással ellentétes irányba repül. A cél minél magasabbra jutni.

- **Csúzli irányítás előnézettel:** húzás közben egy vonal mutatja a lövés irányát és erejét
- **Ugrások a levegőben:** egy ugrássorozatban legfeljebb 3 lövés használható
- **Tapadós felületek:** a ragadós falakon és platformokon meg lehet kapaszkodni, ilyenkor az ugrások újratöltődnek
- **Különböző felületek:** jeges részek, amelyek lefékezik a mozgást, és tüskék, amelyekhez érve újraindul a játék
- **Procedurálisan generált pálya:** a platformok és akadályok véletlenszerű méretben, elforgatással és elhelyezéssel jönnek létre, így minden menet más
- **Mozgó platformok** és a kamerát követő oldalfalak
- **Érmék és rekord:** a legjobb eredményt a játék elmenti
- **„Jelly” effekt:** a karakter becsapódáskor rugalmasan összenyomódik

## Irányítás

| Platform | Irányítás |
|---|---|
| Számítógép | Bal egérgomb: húzás, majd elengedés |
| Mobil | Ujjal húzás, majd elengedés |

## Technológiák

- **Unity 6** (6000.5.2f1), 2D URP
- **C#**
- Unity 2D Physics (Rigidbody2D, ütközések, triggerek)

## Futtatás Unityből

1. Klónozd a repót:
   ```bash
   git clone https://github.com/Meopleo/2DPlatformer.git
   ```
2. Nyisd meg a **Unity Hubot**, és válaszd az **Add → Add project from disk** lehetőséget.
3. Tallózd be a klónozott mappát. A projekt **Unity 6000.5.2f1** verzióval készült, ezt érdemes használni.
4. Nyisd meg az `Assets/Scenes/SampleScene` jelenetet, és nyomd meg a **Play** gombot.

## Projektstruktúra

```
Assets/Scripts/
├── PlayerController.cs   csúzli irányítás, ugrások, tapadás, felületek kezelése
├── LevelGenerator.cs     procedurális pályagenerálás, pontszám, érmék, rekord
├── CameraFollow.cs       a kamera követi a játékost
├── MovingPlatform.cs     mozgó platformok
├── WallMover.cs          a kamerával együtt mozgó oldalfalak
├── Coin.cs               érmék felvétele
└── JellyEffect.cs        rugalmas „zselé” animáció ütközéskor
```

## Szerző

**Tullner Levente**, BME VIK mérnökinformatikus hallgató
