using System;
using System.Collections.Generic;
using BobGreenhands.Map.MapObjects;
using BobGreenhands.Map.Items;


namespace BobGreenhands.Persistence
{
    /// <summary>
    /// Contains the actual Savegame data so it can be easily parsed from and to JSON, without interrupting the actual Savegame class
    /// </summary>
    public class SavegameData
    {
        public long CreationTimestamp;
        public long LastChangedTimestamp;
        public int Seed;
        public int SavegameVersion;
        public bool IsNew;
        public double Balance;
        public long TotalTicks;
        public int MapWidth;
        public int MapHeight;
        public List<ushort> Tiles;
        public List<MapObject> MapObjectList;
        public List<Item> Hotbar;
        public float PlayerXPos;
        public float PlayerYPos;

        public SavegameData()
        {
            Tiles = new List<ushort>();
            MapObjectList = new List<MapObject>();
            Hotbar = new List<Item>(10);
        }
    }
}