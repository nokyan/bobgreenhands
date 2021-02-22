using System;
using Microsoft.Xna.Framework;
using BobGreenhands.Scenes;


namespace BobGreenhands.Map
{
    /// <summary>
    /// Because the Entity location and Tile location work a bit different from each other (and I'm too lazy to figure it out), we use this Location class as a common ground.
    /// What we want is a system, in which the topleft corner represents (0, 0) and the X-axis goes towards the right and the Y-axis goes down.
    /// 1 unit represents 1 tile.
    /// <summary>
    public class Location
    {
        public Vector2 Coordinates
        {
            get;
            private set;
        }

        public float X
        {
            get
            {
                return Coordinates.X;
            }
        }

        public float Y
        {
            get
            {
                return Coordinates.Y;
            }
        }

        public Vector2 EntityCoordinates
        {
            get
            {
                return CoordinatesToEntity(Coordinates);
            }
        }

        public float EntityX
        {
            get
            {
                return CoordinatesToEntity(Coordinates).X;
            }
        }

        public float EntityY
        {
            get
            {
                return CoordinatesToEntity(Coordinates).Y;
            }
        }

        public Location(float x, float y) => Coordinates = new Vector2(x, y);
        public Location(Vector2 vector) => Coordinates = vector;

        public static Location FromEntityCoordinates(float x, float y)
        {
            return new Location(EntityToCoordinates(new Vector2(x, y)));
        }

        public static Location FromEntityCoordinates(Vector2 vector)
        {
            return new Location(EntityToCoordinates(vector));
        }

        public static Vector2 EntityToCoordinates(Vector2 vector)
        {
            // we have to do this because the map size might be 0 on loading the savegame
            int width = PlayScene.CurrentSavegame.SavegameData.MapWidth;
            if(width == 0)
            {
                width = 40;
            }
            int height = PlayScene.CurrentSavegame.SavegameData.MapHeight;
            if(height == 0)
            {
                height = 40;
            }
            Vector2 newVector = new Vector2();
            newVector.X = (vector.X / Game.TextureResolution) + width / 2;
            newVector.Y = (vector.Y / Game.TextureResolution) + height / 2;
            return newVector;
        }

        public static Vector2 CoordinatesToEntity(Vector2 vector)
        {
            // we have to do this because the map size might be 0 on loading the savegame
            int width = PlayScene.CurrentSavegame.SavegameData.MapWidth;
            if(width == 0)
            {
                width = 40;
            }
            int height = PlayScene.CurrentSavegame.SavegameData.MapHeight;
            if(height == 0)
            {
                height = 40;
            }
            Vector2 newVector = new Vector2();
            newVector.X = (int) ((vector.X - width / 2f) * Game.TextureResolution);
            newVector.Y = (int) ((vector.Y - height / 2f) * Game.TextureResolution);
            return newVector;
        }

        public Location SetToCenterOfTile()
        {
            Coordinates = new Vector2((float)Math.Floor(Coordinates.X) + 0.5f, (float)Math.Floor(Coordinates.Y) + 0.5f);
            return this;
        }
    }
}