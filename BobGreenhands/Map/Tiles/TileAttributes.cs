using System.Collections.Generic;
using BobGreenhands.Map.Tiles;

namespace BobGreenhands.Map.Tiles

{
    /// <summary>
    /// Tile Attributes are attributes that is shared between several TileTypes. It's not a great way to do it, but hey, it works.
    /// </summary>
    public static class TileAttributes
    {

        public static List<TileType> COLLIDABLES = new List<TileType>() {
            TileType.ESFence,
            TileType.NEFence,
            TileType.NSFence,
            TileType.SWFence,
            TileType.WEFence,
            TileType.WNFence,
        };

    }
}