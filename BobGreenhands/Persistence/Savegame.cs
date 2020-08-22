using System.Collections.Generic;
using System;
using System.IO;
using Nez.Persistence;
using BobGreenhands.Map.MapObjects;
using BobGreenhands.Map.Tiles;
using BobGreenhands.Map.Items;


namespace BobGreenhands.Persistence
{
    public class Savegame : IComparable
    {
        /// <summary>
        /// Increments every single update
        /// </summary>
        public static int SavegameVersion = 0;

        public string Path
        {
            get;
            private set;
        }

        public SavegameData SavegameData
        {
            get;
            private set;
        }
        
        private JsonSettings _settings = new JsonSettings
        {
            PrettyPrint = false,
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = false,
            TypeConverters = new JsonTypeConverter[] { new StrawberryJsonConverter(), new StrawberryObjectFactory() }
        };

        public Savegame(string path)
        {
            Path = path;
            SavegameData = new SavegameData();
        }

        public void Load()
        {
            SavegameData = Json.FromJson<SavegameData>(File.ReadAllText(Path), _settings);
        }

        public void Save()
        {
            SavegameData.LastChangedTimestamp = GetUnixTimestamp();
            File.WriteAllText(Path, Json.ToJson(SavegameData, _settings));
        }

        /// Thanks to Brad on StackOverflow for this answer: https://stackoverflow.com/a/22539971
        /// <summary>
        /// Returns the current Unix Timestamp (in UTC)
        /// </summary>
        public static long GetUnixTimestamp()
        {
            return (long)Math.Truncate(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }

        public static Savegame CreateSavegame(string name)
        {
            Savegame savegame = new Savegame(System.IO.Path.Combine(Game.GameFolder.SavegamesFolder, name));
            savegame.SavegameData.CreationTimestamp = GetUnixTimestamp();
            savegame.SavegameData.LastChangedTimestamp = GetUnixTimestamp();
            savegame.SavegameData.Seed = Nez.Random.NextInt(Int32.MaxValue);
            savegame.SavegameData.SavegameVersion = SavegameVersion;
            savegame.SavegameData.IsNew = true;
            savegame.SavegameData.Balance = 0.0;
            savegame.SavegameData.MapWidth = 40;
            savegame.SavegameData.MapHeight = 40;
            savegame.SavegameData.Tiles = new List<ushort>();
            // generate the map
            for (int x = 0; x < savegame.SavegameData.MapWidth * savegame.SavegameData.MapHeight; x++)
            {
                // if x is currently either in the top row or bottom row or at the first or last tile of a row
                if(x < savegame.SavegameData.MapWidth
                        || x > savegame.SavegameData.MapHeight * savegame.SavegameData.MapWidth - savegame.SavegameData.MapWidth
                        || x % savegame.SavegameData.MapWidth == 0
                        || x % savegame.SavegameData.MapWidth == savegame.SavegameData.MapWidth - 1)
                {
                    savegame.SavegameData.Tiles.Add((ushort)TileType.BoundaryBush);
                }
                else
                {
                    savegame.SavegameData.Tiles.Add((ushort)TileType.Grass);
                }
            }
            savegame.SavegameData.MapObjectList = new List<MapObject>();
            savegame.SavegameData.Hotbar = new List<Item>(10) {
                new Shovel(),
                new WateringCan(),
                new StrawberrySeeds()
            };
            savegame.SavegameData.PlayerXPos = 20;
            savegame.SavegameData.PlayerYPos = 20;
            savegame.SavegameData = savegame.SavegameData;
            savegame.Save();
            return savegame;
        }

        public int CompareTo(object? obj)
        {
            return Path.CompareTo(obj.ToString());
        }

        public TileType GetTileAt(int tileX, int tileY)
        {
            int tilePos = SavegameData.MapWidth * tileY + tileX;
            return (TileType) SavegameData.Tiles[tilePos];
        }

        public void SetTileAt(int tileX, int tileY, TileType newTileType)
        {
            int tilePos = SavegameData.MapWidth * tileY + tileX;
            SavegameData.Tiles[tilePos] = (ushort) newTileType;
        }

    }
}