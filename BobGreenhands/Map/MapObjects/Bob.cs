using System;
using System.Collections.Generic;
using BobGreenhands.Scenes;
using Microsoft.Xna.Framework;
using Nez.Textures;
using Nez.Sprites;
using Nez;
using BobGreenhands.Utils;


namespace BobGreenhands.Map.MapObjects
{
    public class Bob : MovableMapObject
    {

        private Sprite _frontSprite;
        private Sprite _sideSprite;
        private Sprite _backSprite;

        private Sprite _arrowSprite;
        private Sprite _activeArrowSprite;

        private Queue<Entity> _arrows = new Queue<Entity>(); 
        
        public Bob(float x, float y)
        {
            Position = new Vector2(x, y);
            Speed = 5f;
            _frontSprite = new Sprite(FNATextureHelper.Load("img/mapobjects/player", Game.Content));
            _arrowSprite = new Sprite(FNATextureHelper.Load("img/ui/normal/arrow", Game.Content));
            _activeArrowSprite = new Sprite(FNATextureHelper.Load("img/ui/normal/active_arrow", Game.Content));
            // we don't have a side and back sprite yet, neither do we have animations
            _sideSprite = _frontSprite;
            _backSprite = _frontSprite;
            Location location = Location.FromEntityCoordinates(x, y);
            SpriteRenderer.SetSprite(_frontSprite);
            SpriteRenderer.SetRenderLayer(PlayScene.MapObjectRenderLayer);
            SpriteRenderer.SetOrigin(new Vector2(SpriteRenderer.Origin.X, SpriteRenderer.Origin.Y + Game.TextureResolution));
            CanCoverMapObjects = true;
            AddComponent(SpriteRenderer);
        }

        public override void EnqueueTask(Task task)
        {
            base.EnqueueTask(task);
            Vector2 target = task.Target;
            Entity arrow = new Entity();
            SpriteRenderer arrowSR = new SpriteRenderer(_arrowSprite);
            if (_arrows.Count == 0)
                arrowSR.SetSprite(_activeArrowSprite);
            arrow.AddComponent(arrowSR);
            arrow.SetPosition(new Vector2(target.X, target.Y));
            arrowSR.SetOrigin(new Vector2(arrowSR.Width / 2, arrowSR.Height));
            arrowSR.SetRenderLayer(PlayScene.SelectedMapObjectRenderLayer);
            Scene.AddEntity(arrow);
            _arrows.Enqueue(arrow);
        }

        public override void DequeueTask(bool runTask)
        {
            base.DequeueTask(runTask);
            _arrows.Peek().Destroy();
            _arrows.Dequeue();
            if (_arrows.Count > 0)
            {
                Entity a = _arrows.ToArray()[0];
                SpriteRenderer sr = a.GetComponent<SpriteRenderer>();
                sr.SetSprite(_activeArrowSprite);
                sr.SetOrigin(new Vector2(sr.Width / 2, sr.Height));
            }
        }

        public override void Initialize()
        {
        }

        public void ResetTasks()
        {
            IsMoving = false;
            while(_taskQueue.Count > 0)
            {
                DequeueTask(false);
            }
        }

        public override void OnTick(GameTime gameTime)
        {
            base.OnTick(gameTime);
            Location location = Location.FromEntityCoordinates(Position.X, Position.Y);
            PlayScene.CurrentSavegame.SavegameData.PlayerXPos = location.X;
            PlayScene.CurrentSavegame.SavegameData.PlayerYPos = location.Y;
        }

        public override void OnRandomTick(GameTime gameTime)
        {
        }
    }
}