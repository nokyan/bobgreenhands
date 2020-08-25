using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using BobGreenhands.Scenes;


namespace BobGreenhands.Map.MapObjects
{
    public abstract class MapObject : Entity
    {
        private bool _canCoverMapObjects;
        public bool CanCoverMapObjects
        {
            get
            {
                return _canCoverMapObjects;
            }
            set
            {
                _canCoverMapObjects = value;
                CalculateLayerDepth();
            }
        }

        public Vector2 Hitbox
        {
            get;
            protected set;
        }

        public SpriteRenderer SpriteRenderer = new SpriteRenderer();

        public bool OccupiesTiles
        {
            get;
            protected set;
        }

        /// <summary>
        /// Stuff that is supposed to happen after JSON initialization
        /// </summary>
        public abstract void Initialize();

        public abstract void OnTick(GameTime gameTime);

        public abstract void OnRandomTick(GameTime gameTime);

        public abstract string GetInfoText();

        public void CalculateLayerDepth()
        {
            if(_canCoverMapObjects)
            {
                Location location = Location.FromEntityCoordinates(Position);
                SpriteRenderer.SetLayerDepth(1f - (float)(location.Y / PlayScene.CurrentSavegame.SavegameData.MapHeight));
            }
            else
            {
                SpriteRenderer.SetLayerDepth(1f);
            }
        }

    }
}