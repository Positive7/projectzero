using UnityEngine;
using UnityEngine.EventSystems;

namespace TinyWorlds
{
    /// <summary>
    /// Kattintásra épületet rak a cellára (bal gomb), jobb gombbal lebontja.
    /// Húzásnál nem épít, mert az a kamera forgatása.
    /// </summary>
    public class BuildingPlacer : MonoBehaviour
    {
        public HexPlanet         planet;
        public PlanetOrbitCamera orbitCamera;
        public Camera            cam;

        [Tooltip("1x1 egységes footprintre modellezett prefabok (pivot az alján, +Y felfelé).")]
        public GameObject[] buildingPrefabs;
        public int          selected;

        [Tooltip("A cella méretéhez képest mekkora legyen az épület.")]
        [Range(0.2f, 1f)] public float footprint = 0.6f;

        [Header("Kijelölés")]
        public LineRenderer hoverOutline; // opcionális: loop = true, World Space = true
        public float        outlineLift = 0.02f;

        public HexTile Hovered { get; private set; }

        private void Start()
        {
            if (planet      == null) planet      = FindAnyObjectByType<HexPlanet>();
            if (orbitCamera == null) orbitCamera = FindAnyObjectByType<PlanetOrbitCamera>();
            if (cam         == null) cam         = Camera.main;
        }

        private void Update()
        {
            if (planet == null || cam == null) return;
            bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

            Hovered = null;
            if (!overUI && planet.TryGetTile(cam.ScreenPointToRay(PointerInput.Position), out var tile)) Hovered = tile;
            UpdateOutline();

            if (Hovered == null) return;

            if (PointerInput.PrimaryUp)
            {
                bool wasDrag = orbitCamera != null && orbitCamera.ConsumeDrag();
                if (!wasDrag) Place(Hovered);
            }

            if (PointerInput.SecondaryDown) Remove(Hovered);
        }

        public bool CanPlace(HexTile tile) => tile != null && tile.Occupant == null && !tile.IsWater;

        public void Place(HexTile tile)
        {
            if (!CanPlace(tile) || buildingPrefabs == null || buildingPrefabs.Length == 0) return;
            var prefab = buildingPrefabs[Mathf.Clamp(selected, 0, buildingPrefabs.Length - 1)];

            var go = Instantiate(prefab, planet.GetTileTop(tile), planet.GetTileRotation(tile, Random.Range(0, 6)), planet.transform);
            go.transform.localScale = Vector3.one * (planet.TileSize * footprint);
            tile.Occupant = go;

            // Kis "boing" a jam verzióból: felpattan a helyére
            StartCoroutine(Pop(go.transform, go.transform.localScale));
        }

        public void Remove(HexTile tile)
        {
            if (tile?.Occupant == null) return;
            Destroy(tile.Occupant);
            tile.Occupant = null;
        }

        private System.Collections.IEnumerator Pop(Transform tr, Vector3 target)
        {
            const float dur = 0.35f;
            for (float t = 0; t < dur; t += Time.deltaTime)
            {
                if (tr == null) yield break;
                float x = t / dur;
                // túllövés, majd visszaáll (easeOutBack)
                float s = 1f + 2.2f * Mathf.Pow(x - 1f, 3) + 1.2f * Mathf.Pow(x - 1f, 2);
                tr.localScale = target * s;
                yield return null;
            }
            if (tr != null) tr.localScale = target;
        }

        private void UpdateOutline()
        {
            if (hoverOutline == null) return;
            if (Hovered == null) { hoverOutline.enabled = false; return; }

            hoverOutline.enabled  = true;
            hoverOutline.loop     = true;
            var corners           = Hovered.Corners;
            hoverOutline.positionCount = corners.Length;
            float r = planet.radius + Hovered.Height + outlineLift;
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 planar = corners[i] * (r / Vector3.Dot(corners[i], Hovered.Center));
                hoverOutline.SetPosition(i, planet.transform.TransformPoint(Vector3.Lerp(Hovered.Center * r, planar, planet.tileInset)));
            }
            hoverOutline.startColor = hoverOutline.endColor = CanPlace(Hovered) ? Color.white : new Color(1f, 0.45f, 0.4f);
        }
    }
}
