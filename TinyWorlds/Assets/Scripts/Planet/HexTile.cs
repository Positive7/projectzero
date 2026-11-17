using UnityEngine;

namespace TinyWorlds
{
    public enum Biome
    {
        Ocean,
        Shallow,
        Sand,
        Meadow,
        Forest,
        Hills,
        Snow
    }

    /// <summary>
    /// Egy cella a bolygón. Minden pozíció a bolygó lokális terében értendő,
    /// egységgömbre normalizálva (Center, Corners).
    /// </summary>
    public sealed class HexTile
    {
        public int       Index;
        public Vector3   Center;    // egységvektor, egyben a cella "fel" iránya
        public Vector3[] Corners;   // egységvektorok, körbe rendezve
        public int[]     Neighbors; // szomszédos cellák indexei

        public Biome Biome;
        public float Height;        // a sugárhoz adott magasság (lokális egység)

        /// <summary>Ami a cellán áll (épület stb.). Null, ha üres.</summary>
        public GameObject Occupant;

        public bool IsPentagon => Corners.Length == 5;
        public bool IsWater    => Biome == Biome.Ocean || Biome == Biome.Shallow;
    }
}
