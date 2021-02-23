using System;
using BobGreenhands.Map.MapObjects;
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

        /// <summary>
        /// Called when the item has been used on a MapObject. 
        /// </summary>
        public virtual void UsedOnMapObject(MapObject mapObject, PlayScene playScene) { }

        /// <summary>
        /// Returns the text that is supposed to be shown in an inventory slot with this item.
        /// </summary>
        public abstract string? GetInfoString();

        /// <summary>
        /// Returns the text that shows in the InfoElement when the player hovers of it.
        /// </summary>
        public abstract string? GetInfoText();

        /// <summary>
        /// A float between 0 and 1 that is visually displayed below the item itself in an inventory slot.x
        /// </summary>
        public abstract float GetInfoFloat();
    }
}