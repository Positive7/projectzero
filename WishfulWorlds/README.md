# Wishful Worlds – hexa-bolygó alap

Unity 6.3 + URP. Az `Assets` mappa tartalmát másold be a Unity projekt `Assets` mappájába (Unity 6.3, Universal 3D sablon). A mappastruktúra leírása lent.

## Scene beállítás (kb. 5 perc)

1. **Bolygó:** üres GameObject (`Planet`) a (0,0,0) pontba → `HexPlanet` komponens
   (automatikusan hozzáadja a MeshFilter / MeshRenderer / MeshCollider-t).
   - Anyag: új `Universal Render Pipeline/Lit` (vagy Simple Lit), Base Color fehér, Smoothness kb. 0.1.
     A színeket a script adja egy paletta-textúrával, a Base Mapet üresen hagyhatod.
   - Jobb klikk a komponensen → **Generate**, hogy szerkesztőben is lásd (Play-ben magától generál).
2. **Kamera:** Main Camera → `PlanetOrbitCamera`, a `planet` mezőbe húzd be a bolygót.
3. **Építés:** üres GameObject → `BuildingPlacer`.
   - `buildingPrefabs`: bármilyen prefab, aminek a pivotja az alján van, +Y felfelé néz, és kb. 1×1 egység.
     Teszthez egy Cube is jó (pivot javítás: tedd egy üres szülő alá, y = 0.5 eltolással).
   - Opcionális kijelölés: `LineRenderer` (Loop ✔, Use World Space ✔, width 0.05) → `hoverOutline`.
4. **Fény:** egy Directional Light. Később ez fog keringeni a bolygó körül (nappal/éjszaka).

## Irányítás
- Bal gomb húzás: forgatás · görgő: zoom
- Bal klikk: építés · jobb klikk: bontás (vízre nem lehet építeni)

## Felépítés
| Fájl | Mit csinál |
|---|---|
| `HexSphereGenerator.cs` | Tiszta C#: felosztott ikozaéder duálisa → cellák, sarkok, szomszédok. Cellák = 10·n²+2, ebből 12 ötszög. |
| `HexTile.cs` | Cella adat: középpont, sarkok, szomszédok, biom, magasság, ami rajta áll. |
| `HexPlanet.cs` | Biomok (zaj), mesh (kiemelt cellák letöréssel és oldalfallal), raycast → cella, igazítás. |
| `PlanetOrbitCamera.cs` | Gimbal-lock mentes forgatás + zoom. |
| `BuildingPlacer.cs` | Hover, építés/bontás, „boing” animáció. |
| `PointerInput.cs` | Új Input System és régi Input Manager is megy. |

## Következő lépések
- A 12 ötszög (`tile.IsPentagon`) → különleges helyek.
- Szomszédsági bónusz: `tile.Neighbors` alapján.
- Saját Shader Graph (toon ramp + rim light) a sima Lit helyett.
- Keringő nap + két LUT (nappal/este).

## Mappastruktúra
```
Assets/
├── _Project/        minden saját tartalom (az aláhúzás miatt mindig felül van)
│   ├── Scripts/     Planet, Town, Buildings, Residents, Camera, Input, UI, Core, Editor
│   ├── Data/        ScriptableObjectek: Buildings, Biomes, Planets, Wishes
│   ├── Prefabs/     Buildings, Residents, Planet, UI
│   ├── Art/         Models, Materials, Shaders, Textures, VFX
│   ├── Audio/       Music, SFX
│   ├── Scenes/      Boot, MainMenu, Planet + Sandbox/ a kísérleteknek
│   └── Settings/    URP asset, Volume profilok, LUT-ok, Input Actions
├── ThirdParty/      asset packok
└── Plugins/
```
Szabályok: épület, biom és bolygó adat ScriptableObjectben legyen, ne kódban. Az üres mappákban lévő `.gitkeep` csak azért van, hogy a git megtartsa a mappát; törölhető, ha már van benne fájl.
