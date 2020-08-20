using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using BobGreenhands.Map.Tiles;
using BobGreenhands.Scenes;


namespace BobGreenhands.Map.MapObjects
{
    public abstract class MovableMapObject : MapObject
    {
        
        protected Queue<Task> _taskQueue = new Queue<Task>();

        private float _speed;
        public float Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value * (Game.TextureResolution/32f);
            }
        }

        public bool IsMoving;

        public override void OnTick(GameTime gameTime)
        {
            if(IsMoving)
            {
                Vector2 target = _taskQueue.Peek().Target;
                Vector2 distance = new Vector2(target.X - Position.X, target.Y - Position.Y);
                if(distance.Length() < 0.5)
                {
                    Position = target;
                    DequeueTask(true);
                    if(_taskQueue.Count == 0)
                        IsMoving = false;
                    return;
                }
                distance.Normalize();
                distance.X = (float)((distance.X * Speed * 16) * gameTime.ElapsedGameTime.TotalSeconds);
                distance.Y = (float)((distance.Y * Speed * 16) * gameTime.ElapsedGameTime.TotalSeconds);
                Location location = Location.FromEntityCoordinates(Position + distance);
                // best tile-to-mapobject collision detection system ever, where's my Nobel prize?
                if(TileAttributes.COLLIDABLES.Contains(PlayScene.CurrentSavegame.GetTileAt((int) location.X, (int) location.Y)))
                {
                    DequeueTask(false);
                    if(_taskQueue.Count == 0)
                        IsMoving = false;
                    return;
                }
                Position = new Vector2(Position.X + distance.X, Position.Y + distance.Y);
                CalculateLayerDepth();
            }
        }

        public virtual Task PeekTask()
        {
            return _taskQueue.Peek();
        }

        public virtual void EnqueueTask(Task task)
        {
            _taskQueue.Enqueue(task);
        }

        public virtual void DequeueTask(bool runTask)
        {
            if(runTask)
                _taskQueue.Peek().Run();
            _taskQueue.Dequeue();
        }

    }


    /// <summary>
    /// A small helper class mainly useful for Bob, so doing something after arriving at the target can be implemented easily.
    /// </summary>
    public class Task
    {
        public Vector2 Target;

        public Action? Function;

        public Task(Vector2 target, Action? function)
        {
            Target = target;
            Function = function;
        }

        public void Run()
        {
            if(Function != null)
                Function();
        }
    }
}