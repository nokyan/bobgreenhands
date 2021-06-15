using System;
using BobGreenhands.Map.Tiles;
using BobGreenhands.Map.MapObjects;
using BobGreenhands.Scenes;
using BobGreenhands.Utils.CultureUtils;


namespace BobGreenhands.Map.Items
{
    public class StrawberrySeeds : BreakableItem
    {
        public StrawberrySeeds()
        {
            _type = ItemType.StrawberrySeeds;
            Durability = 30;
            MaxDurability = 30;
        }

        public override string? GetInfoText()
        {
            return String.Format("{0}\n{1}", Language.Translate("item.strawberrySeeds.name"),  Language.Translate("game.quickInfo.amount", "" + Durability, "" + MaxDurability));
        }

        public override bool UsedOnTile(int tileX, int tileY, TileType tile, PlayScene playScene)
        {
            if(Durability <= 0)
            {
                return false;
            }
            if(tile == TileType.Farmland && !playScene.IsOccupiedByMapObject(tileX, tileY))
            {
                Location location = new Location(tileX + 0.5f, tileY + 0.5f);
                playScene.AddMapObject(new Strawberry(location.EntityX, location.EntityY, 0, false));
                Durability--;
                return true;
            }
            return false;
        }
    }
}