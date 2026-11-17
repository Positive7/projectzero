# Wishful Worlds – projekt kontextus Claude számára

A felhasználóval magyarul beszélj. Egyedül fejleszti a játékot, Unity 6.3 + URP alatt.
Projekt helye a gépén: `C:\UnityProjects\Wishful Worlds\`

## A játék röviden
Kozy bolygóépítő Steamre (7,99–9,99 USD). Kis hexarácsos gömbbolygók, a lakóknak neve és kívánságai vannak,
a teljesített kívánságok **virágzást** adnak (egyetlen erőforrás). Nincs vesztés, bolygónként 3 csillag.

- **Játékterv (élő doksi):** https://claude.ai/artifact/Hsa56TrByCRU8UcQAJqpRP
- **Látványterv (forgatható hexabolygó-demóval):** https://claude.ai/artifact/BNhvB4NQLJMzQxNVWqeB7P
- Eredet: egy 1 hetes game jam projekt (udc-jam-week-2, itch.io) – a `positive7/projectzero` repó gyökerében lévő régi scriptek. Az új játék a `WishfulWorlds/` mappában van.

## Meghozott döntések
- **Két szint:** a bolygó hexacellái régiók. Egy cellára kattintva betölt egy sík hexarácsos **város**:
  5 sugarú hatszög (91 cella), szintlépéskor 6 sugarú (127 cella). A kívánság-hurok a városban zajlik.
- **Terjeszkedés:** a 3. városszinttől telepesek küldhetők egy szabad cellára, legfeljebb 2 cellányira egy meglévő várostól.
  Ha egy várost mind a 6 (5) szomszédos cellán saját város veszi körül → **megapolisz** (bolygónként 1–2).
- **Alvó városok:** mindig csak egy város aktív. A többi nem kér figyelmet, nem romlik, de csak kb. **25%** virágzást termel.
- **Kampány:** 4 bolygó (Zsálya 252 cella, Lagúnák 492, Fagyó 492, Kert 1002) + **sandbox mód**.
- **Nehézség:** puha cél (csillagok). Később opcionális „Gondos” mód nyersanyagokkal (fa, kő, élelem),
  amik **mindig visszatermelődnek**, sosem fogynak el, és a raktárnak plafonja van.
- **Lakók:** névvel, szakmával, 1–2 kívánsággal. Szomszédsági bónuszok az épületek között.
- **12 ötszögű cella** a gömbön = különleges csodahelyek.
- **Látvány:** puha low-poly dioráma, lapos árnyalás, egy paletta-textúra, keringő nap, esti ablakfények, tilt-shift közelről.
- Ütemterv: 1.0 kb. 2027 nyara, két döntési kapuval (szórakoztató-e a prototípus; van-e wishlist-érdeklődés).

## Kód állapota
`Assets/_Project/Scripts/`, namespace `WishfulWorlds`:
- `Planet/HexSphereGenerator.cs` – Goldberg-poliéder (felosztott ikozaéder duálisa), cellák = 10·n²+2, ebből 12 ötszög. Tiszta C#.
- `Planet/HexTile.cs` – cella adat (Center, Corners, Neighbors, Biome, Height, Occupant), `Biome` enum.
- `Planet/HexPlanet.cs` – biomok zajból, mesh (kiemelt cellák, letörés, oldalfal), paletta-textúra sima URP/Lit anyaghoz,
  `TryGetTile(Ray)`, `GetTileTop/Up/Rotation`, `TileSize`.
- `Camera/PlanetOrbitCamera.cs` – gimbal-lock mentes forgatás + zoom.
- `Buildings/BuildingPlacer.cs` – hover kijelölés, építés bal klikk, bontás jobb klikk, „boing” animáció.
- `Input/PointerInput.cs` – új Input System és régi Input Manager is.

**Működik** Unity-ben (a felhasználó tesztelte, kockákkal építve). Ismert apróságok: a teszt-kockák lebegnek
(pivot / footprint), és a háttér még az alap skybox.

## Következő lépések (javasolt sorrend)
1. Bolygó → város váltás: klikk egy cellára, ráközelítés, sík hexarácsos város betöltése (91 cella).
2. Az ötszögek megjelölése csodahelyként.
3. Keringő nap, esti oldal.
4. Toon Shader Graph (lépcsős ramp + rim light).
5. Épületek, biomok, bolygók ScriptableObject adatként (`_Project/Data/`).

## Munkaszabályok
- A scripteket a Unity Project ablakában mozgasd (vagy a .meta fájlokkal együtt), különben elvesznek a komponens-hivatkozások.
- Nyitott kérdések: domain és social nevek, védjegy (EUIPO / TMview), asset pack választás, zene, kiadó vagy saját kiadás.
