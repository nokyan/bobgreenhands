using System;
using BobGreenhands.Map.MapObjects;
using BobGreenhands.Map.Tiles;
using BobGreenhands.Scenes;
using BobGreenhands.Utils.CultureUtils;


namespace BobGreenhands.Map.Items
{
    public class Shovel : BreakableItem
    {
        public static new readonly int MaxDurability = 120;

        public Shovel()
        {
            _type = ItemType.Shovel;
            Durability = 120;
        }

        public override string? GetTooltipString()
        {
            return Language.Translate("shovel");
        }

        public override bool UsedOnTile(int tileX, int tileY, TileType tile, PlayScene playScene)
        {
            if(Durability <= 0)
            {
                return false;
            }
            if(tile == TileType.Grass)
            {
                PlayScene.CurrentSavegame.SetTileAt(tileX, tileY, TileType.Farmland);
                Durability--;
                return true;
            }
            return false;
        }

        public override void UsedOnMapObject(MapObject mapObject, PlayScene playScene)
        {
            if(Durability <= 0)
            {
                return;
            }
            if(mapObjects is Plant)
            {
                mapObjects.Destroy();
                Durability--;
            }
        }
    }
}