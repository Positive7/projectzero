using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

namespace TinyWorlds
{
    [Serializable]
    public struct BiomeSettings
    {
        public Biome biome;
        public Color color;
        [Tooltip("Magasság a bolygó sugara fölött (lokális egység).")]
        public float height;
    }

    /// <summary>
    /// Hexa-bolygó: legenerálja a cellákat, a biomokat és a mesh-t (kiemelt, letört élű hatszögek).
    /// A színezés egy futásidőben készült paletta-textúrával megy, így sima URP/Lit anyaggal is működik.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class HexPlanet : MonoBehaviour
    {
        [Header("Rács")]
        [Range(2, 16)] public int frequency = 7;
        public float radius = 10f;

        [Header("Terep")]
        public int  seed = 42;
        public bool randomSeedOnStart;
        [Range(0.7f, 1f)] public float tileInset    = 0.92f; // a cella teteje mennyire kisebb a teljes cellánál
        public                    float bevelHeight = 0.08f;  // a letörés magassága
        public                    float skirtDepth  = 0.6f;   // az oldalfal ennyivel nyúlik a sugár alá

        [Header("Biomok")]
        public BiomeSettings[] biomes = DefaultBiomes();

        public IReadOnlyList<HexTile> Tiles => tiles;
        /// <summary>Két szomszédos cella középpontja közti átlagos távolság (lokális egység). Épületek méretezéséhez.</summary>
        public float TileSize { get; private set; }

        public event Action<HexPlanet> Generated;

        private List<HexTile> tiles = new();
        private int[]         triangleToTile;
        private Mesh          mesh;
        private Texture2D     palette;
        private MeshCollider  meshCollider;

        private void Awake() => meshCollider = GetComponent<MeshCollider>();

        private void Start()
        {
            if (randomSeedOnStart) seed = Environment.TickCount;
            Generate();
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            if (meshCollider == null) meshCollider = GetComponent<MeshCollider>();
            if (biomes == null || biomes.Length == 0) biomes = DefaultBiomes();

            tiles = HexSphereGenerator.Build(frequency);
            AssignBiomes();
            ComputeTileSize();
            BuildPalette();
            BuildMesh();
            Generated?.Invoke(this);
        }

        // ---------------- Lekérdezések ----------------

        public bool TryGetTile(Ray ray, out HexTile tile, float maxDistance = 1000f)
        {
            tile = null;
            if (!meshCollider.Raycast(ray, out RaycastHit hit, maxDistance)) return false;
            if (hit.triangleIndex < 0 || hit.triangleIndex >= triangleToTile.Length) return false;
            tile = tiles[triangleToTile[hit.triangleIndex]];
            return true;
        }

        /// <summary>A cella tetejének közepe világtérben (ide kerülnek az épületek).</summary>
        public Vector3 GetTileTop(HexTile tile) => transform.TransformPoint(tile.Center * (radius + tile.Height));

        /// <summary>A cella "fel" iránya világtérben.</summary>
        public Vector3 GetTileUp(HexTile tile) => transform.TransformDirection(tile.Center);

        /// <summary>Forgatás, ami a cellára igazít: Y a cella normálja, Z az első sarok felé néz.</summary>
        public Quaternion GetTileRotation(HexTile tile, int cornerOffset = 0)
        {
            Vector3 up     = tile.Center;
            Vector3 corner = tile.Corners[((cornerOffset % tile.Corners.Length) + tile.Corners.Length) % tile.Corners.Length];
            Vector3 fwd    = Vector3.ProjectOnPlane(corner - up, up).normalized;
            return transform.rotation * Quaternion.LookRotation(fwd, up);
        }

        public BiomeSettings GetBiomeSettings(Biome b)
        {
            foreach (var s in biomes) if (s.biome == b) return s;
            return new BiomeSettings { biome = b, color = Color.magenta, height = 0f };
        }

        // ---------------- Generálás ----------------

        private struct Wave { public Vector3 dir; public float freq, phase, amp; }

        private void AssignBiomes()
        {
            var rng   = new Random(seed);
            var waves = new Wave[7];
            for (int i = 0; i < waves.Length; i++)
            {
                waves[i] = new Wave
                {
                    dir   = new Vector3((float) rng.NextDouble() * 2 - 1, (float) rng.NextDouble() * 2 - 1, (float) rng.NextDouble() * 2 - 1).normalized,
                    freq  = 1.5f + (float) rng.NextDouble() * 3.5f,
                    phase = (float) rng.NextDouble() * Mathf.PI * 2,
                    amp   = 0.5f + (float) rng.NextDouble()
                };
            }

            foreach (var t in tiles)
            {
                float h = 0f;
                foreach (var w in waves) h += Mathf.Sin(Vector3.Dot(t.Center, w.dir) * w.freq * 2f + w.phase) * w.amp;
                h /= waves.Length;

                Biome b = h < -0.12f ? Biome.Ocean
                        : h < -0.02f ? Biome.Shallow
                        : h <  0.04f ? Biome.Sand
                        : h <  0.22f ? Biome.Meadow
                        : h <  0.36f ? Biome.Forest
                        : Biome.Hills;

                float lat = Mathf.Abs(t.Center.y);
                if ((lat > 0.86f && b != Biome.Ocean && b != Biome.Shallow) || lat > 0.93f) b = Biome.Snow;

                t.Biome  = b;
                t.Height = GetBiomeSettings(b).height;
            }
        }

        private void ComputeTileSize()
        {
            float sum = 0f; int cnt = 0;
            foreach (var t in tiles)
            foreach (int nb in t.Neighbors)
            {
                sum += Vector3.Distance(t.Center, tiles[nb].Center);
                cnt++;
            }
            TileSize = cnt > 0 ? sum / cnt * radius : 1f;
        }

        private void BuildPalette()
        {
            if (palette == null)
            {
                palette = new Texture2D(biomes.Length, 1, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    wrapMode   = TextureWrapMode.Clamp,
                    name       = "BiomePalette"
                };
            }
            else if (palette.width != biomes.Length) palette.Reinitialize(biomes.Length, 1);

            for (int i = 0; i < biomes.Length; i++) palette.SetPixel(i, 0, biomes[i].color);
            palette.Apply();

            var mpb = new MaterialPropertyBlock();
            mpb.SetTexture("_BaseMap", palette); // URP Lit / Simple Lit
            mpb.SetTexture("_MainTex", palette); // régi / saját shaderek
            GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
        }

        private Vector2 BiomeUV(Biome b)
        {
            for (int i = 0; i < biomes.Length; i++)
                if (biomes[i].biome == b) return new Vector2((i + 0.5f) / biomes.Length, 0.5f);
            return new Vector2(0.5f / biomes.Length, 0.5f);
        }

        private void BuildMesh()
        {
            var vertices = new List<Vector3>(tiles.Count * 40);
            var uvs      = new List<Vector2>(tiles.Count * 40);
            var tris     = new List<int>(tiles.Count * 90);
            var triTile  = new List<int>(tiles.Count * 30);

            foreach (var t in tiles)
            {
                Vector3 c     = t.Center;
                float   top   = radius + t.Height;
                float   mid   = top - bevelHeight;
                float   bottom = radius - skirtDepth;
                Vector2 uv    = BiomeUV(t.Biome);
                int     k     = t.Corners.Length;

                Vector3 topCenter = c * top;
                var topCorners = new Vector3[k];
                var midCorners = new Vector3[k];
                var botCorners = new Vector3[k];
                for (int i = 0; i < k; i++)
                {
                    Vector3 dir = t.Corners[i];
                    // sík tető: a sarok a cella érintősíkjára vetítve, befelé húzva
                    Vector3 planar = dir * (top / Vector3.Dot(dir, c));
                    topCorners[i] = Vector3.Lerp(topCenter, planar, tileInset);
                    // a letörés alja és a fal alja gömbi pont, így az azonos magasságú szomszédok pontosan illeszkednek
                    midCorners[i] = dir * mid;
                    botCorners[i] = dir * bottom;
                }

                // Tető (legyező)
                int baseIndex = vertices.Count;
                vertices.Add(topCenter); uvs.Add(uv);
                for (int i = 0; i < k; i++) { vertices.Add(topCorners[i]); uvs.Add(uv); }
                for (int i = 0; i < k; i++)
                    AddTri(baseIndex, baseIndex + 1 + i, baseIndex + 1 + (i + 1) % k, c, t.Index);

                // Oldalak: letörés + fal
                for (int i = 0; i < k; i++)
                {
                    int     j       = (i + 1) % k;
                    Vector3 edgeMid = (midCorners[i] + midCorners[j]) * 0.5f;
                    Vector3 outward = (edgeMid - c * Vector3.Dot(edgeMid, c)).normalized;

                    AddQuad(topCorners[i], topCorners[j], midCorners[j], midCorners[i], outward + c * 0.5f, t.Index);
                    AddQuad(midCorners[i], midCorners[j], botCorners[j], botCorners[i], outward, t.Index);
                }

                void AddQuad(Vector3 a, Vector3 b, Vector3 d, Vector3 e, Vector3 facing, int tileIndex)
                {
                    int s = vertices.Count;
                    vertices.Add(a); vertices.Add(b); vertices.Add(d); vertices.Add(e);
                    uvs.Add(uv); uvs.Add(uv); uvs.Add(uv); uvs.Add(uv);
                    AddTri(s, s + 1, s + 2, facing, tileIndex);
                    AddTri(s, s + 2, s + 3, facing, tileIndex);
                }
            }

            // Unity-ben a háromszög eleje a Cross(b-a, c-a) irányába néz; ehhez igazítjuk a sorrendet.
            void AddTri(int a, int b, int d, Vector3 facing, int tileIndex)
            {
                Vector3 nrm = Vector3.Cross(vertices[b] - vertices[a], vertices[d] - vertices[a]);
                if (Vector3.Dot(nrm, facing) >= 0f) { tris.Add(a); tris.Add(b); tris.Add(d); }
                else                                { tris.Add(a); tris.Add(d); tris.Add(b); }
                triTile.Add(tileIndex);
            }

            if (mesh == null)
            {
                mesh = new Mesh { name = "HexPlanet" };
                mesh.MarkDynamic();
            }
            mesh.Clear();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals(); // minden lapnak saját csúcsai vannak -> lapos (flat) árnyalás
            mesh.RecalculateBounds();

            GetComponent<MeshFilter>().sharedMesh = mesh;
            meshCollider.sharedMesh = null; // frissítés kikényszerítése
            meshCollider.sharedMesh = mesh;
            triangleToTile = triTile.ToArray();
        }

        private void Reset() => biomes = DefaultBiomes();

        private static BiomeSettings[] DefaultBiomes() => new[]
        {
            new BiomeSettings { biome = Biome.Ocean,   color = Hex("#5AA9C9"), height = -0.15f },
            new BiomeSettings { biome = Biome.Shallow, color = Hex("#8FD0D6"), height = -0.08f },
            new BiomeSettings { biome = Biome.Sand,    color = Hex("#ECD7A2"), height =  0.00f },
            new BiomeSettings { biome = Biome.Meadow,  color = Hex("#9CCC7A"), height =  0.08f },
            new BiomeSettings { biome = Biome.Forest,  color = Hex("#5F9E64"), height =  0.14f },
            new BiomeSettings { biome = Biome.Hills,   color = Hex("#B6B47D"), height =  0.30f },
            new BiomeSettings { biome = Biome.Snow,    color = Hex("#F2F5F7"), height =  0.20f },
        };

        private static Color Hex(string s) => ColorUtility.TryParseHtmlString(s, out var c) ? c : Color.magenta;
    }
}
