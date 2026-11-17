using System.Collections.Generic;
using UnityEngine;

namespace TinyWorlds
{
    /// <summary>
    /// Goldberg-poliéder (hexasphere) generálás: felosztott ikozaéder duálisa.
    /// Cellák száma = 10 * n^2 + 2, ebből mindig 12 ötszög.
    /// Tiszta C#, nincs benne MonoBehaviour, így tesztelhető és szálon is futtatható.
    /// </summary>
    public static class HexSphereGenerator
    {
        public static List<HexTile> Build(int frequency)
        {
            int n = Mathf.Max(1, frequency);

            // --- 1. Ikozaéder ---
            float t = (1f + Mathf.Sqrt(5f)) / 2f;
            Vector3[] ico =
            {
                new(-1, t, 0), new(1, t, 0), new(-1, -t, 0), new(1, -t, 0),
                new(0, -1, t), new(0, 1, t), new(0, -1, -t), new(0, 1, -t),
                new(t, 0, -1), new(t, 0, 1), new(-t, 0, -1), new(-t, 0, 1)
            };
            for (int i = 0; i < ico.Length; i++) ico[i] = ico[i].normalized;

            int[,] icoFaces =
            {
                { 0, 11, 5 }, { 0, 5, 1 }, { 0, 1, 7 }, { 0, 7, 10 }, { 0, 10, 11 },
                { 1, 5, 9 }, { 5, 11, 4 }, { 11, 10, 2 }, { 10, 7, 6 }, { 7, 1, 8 },
                { 3, 9, 4 }, { 3, 4, 2 }, { 3, 2, 6 }, { 3, 6, 8 }, { 3, 8, 9 },
                { 4, 9, 5 }, { 2, 4, 11 }, { 6, 2, 10 }, { 8, 6, 7 }, { 9, 8, 1 }
            };

            // --- 2. Felosztás, közös csúcsok összevonása ---
            var verts  = new List<Vector3>();
            var lookup = new Dictionary<Vector3Int, int>();
            var faces  = new List<int[]>();

            int VertexId(Vector3 p)
            {
                p = p.normalized;
                var key = new Vector3Int(Mathf.RoundToInt(p.x * 1e4f), Mathf.RoundToInt(p.y * 1e4f), Mathf.RoundToInt(p.z * 1e4f));
                if (lookup.TryGetValue(key, out int id)) return id;
                verts.Add(p);
                lookup[key] = verts.Count - 1;
                return verts.Count - 1;
            }

            for (int f = 0; f < icoFaces.GetLength(0); f++)
            {
                Vector3 a = ico[icoFaces[f, 0]], b = ico[icoFaces[f, 1]], c = ico[icoFaces[f, 2]];
                var grid = new int[n + 1][];
                for (int i = 0; i <= n; i++)
                {
                    grid[i] = new int[n + 1 - i];
                    for (int j = 0; j <= n - i; j++)
                    {
                        int k = n - i - j;
                        grid[i][j] = VertexId(a * k / n + b * i / n + c * j / n);
                    }
                }

                for (int i = 0; i < n; i++)
                for (int j = 0; j < n - i; j++)
                {
                    faces.Add(new[] { grid[i][j], grid[i + 1][j], grid[i][j + 1] });
                    if (j < n - i - 1) faces.Add(new[] { grid[i + 1][j], grid[i + 1][j + 1], grid[i][j + 1] });
                }
            }

            // --- 3. Csúcs -> háromszögek, csúcs -> szomszédok ---
            var vertFaces = new List<int>[verts.Count];
            var vertNeigh = new HashSet<int>[verts.Count];
            for (int i = 0; i < verts.Count; i++)
            {
                vertFaces[i] = new List<int>(6);
                vertNeigh[i] = new HashSet<int>();
            }

            var centroids = new Vector3[faces.Count];
            for (int fi = 0; fi < faces.Count; fi++)
            {
                int[] f = faces[fi];
                centroids[fi] = (verts[f[0]] + verts[f[1]] + verts[f[2]]).normalized;
                for (int e = 0; e < 3; e++)
                {
                    vertFaces[f[e]].Add(fi);
                    vertNeigh[f[e]].Add(f[(e + 1) % 3]);
                    vertNeigh[f[e]].Add(f[(e + 2) % 3]);
                }
            }

            // --- 4. Duális: minden csúcs egy cella, sarkai a körülötte lévő háromszögek középpontjai ---
            var tiles = new List<HexTile>(verts.Count);
            for (int vi = 0; vi < verts.Count; vi++)
            {
                Vector3 c = verts[vi];
                Vector3 u = Vector3.Cross(c, Mathf.Abs(c.y) < 0.9f ? Vector3.up : Vector3.right).normalized;
                Vector3 w = Vector3.Cross(c, u);

                var corners = new Vector3[vertFaces[vi].Count];
                for (int k = 0; k < corners.Length; k++) corners[k] = centroids[vertFaces[vi][k]];
                System.Array.Sort(corners, (p, q) =>
                    Mathf.Atan2(Vector3.Dot(p, w), Vector3.Dot(p, u))
                         .CompareTo(Mathf.Atan2(Vector3.Dot(q, w), Vector3.Dot(q, u))));

                var neighbors = new int[vertNeigh[vi].Count];
                vertNeigh[vi].CopyTo(neighbors);

                tiles.Add(new HexTile { Index = vi, Center = c, Corners = corners, Neighbors = neighbors });
            }

            return tiles;
        }
    }
}
