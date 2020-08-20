using BobGreenhands.Map.Tiles;
using BobGreenhands.Scenes;


namespace BobGreenhands.Map.Items
{
    public abstract class Item
    {
        protected ItemType _type;

        public ItemType GetItemType()
        {
            return _type;
        }

        /// <summary>
        /// Called when the item has been used on a tile. 
        /// Returns true if PlayScene.BuildMapTexture() should be called (e. g. if it has changed a tile)
        /// </summary>
        public virtual bool UsedOnTile(int tileX, int tileY, TileType tile, PlayScene playScene)
        {
            return false;
        }

        public abstract string? GetInfoString();

        public abstract string? GetTooltipString();
    }
}