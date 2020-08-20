using System;
using System.Collections.Generic;
using BobGreenhands.Map.Tiles;
using BobGreenhands.Persistence;
using Microsoft.Xna.Framework;
using Nez;


namespace BobGreenhands.Scenes.ECS.Components
{
    public class MapRenderer : RenderableComponent
    {

        public float _width;
        public override float Width
        {
            get
            {
                return _width;
            }
        }

        public float _height;
        public override float Height
        {
            get
            {
                return _width;
            }
        }

        private List<TileType> _tilesCache;

        public MapRenderer()
        {
            Refresh();
        }

        public void Refresh()
        {
            Savegame savegame = PlayScene.CurrentSavegame;
            _width = savegame.SavegameData.MapWidth * Game.TextureResolution;
            _height = savegame.SavegameData.MapHeight * Game.TextureResolution;
            _tilesCache = new List<TileType>();
            foreach (ushort u in savegame.SavegameData.Tiles)
            {
                _tilesCache.Add((TileType) u);
            }
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            SavegameData savegameData = PlayScene.CurrentSavegame.SavegameData;
            for (int x = 0; x < savegameData.MapWidth * savegameData.MapHeight; x++)
            {
                int res = Game.TextureResolution;
                float xPos, yPos;
                xPos = Entity.Position.X + (x % savegameData.MapWidth) * res;
                yPos = Entity.Position.Y + Convert.ToInt32(Math.Floor((float) x / savegameData.MapHeight) * res);
                try
                {
                    batcher.Draw(PlayScene.TileTextures[_tilesCache[x]], new Vector2(xPos, yPos), Color.White);
                }
                catch(Exception e)
                {
                    if(e is KeyNotFoundException || e is ArgumentOutOfRangeException)
                    {
                        batcher.Draw(PlayScene.TileTextures[TileType.Unknown], new Vector2(xPos, yPos), Color.White);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}