using System;
using System.Collections.Generic;
using BobGreenhands.Utils;
using Nez.Textures;


namespace BobGreenhands.Map.MapObjects
{
    public abstract class Plant : MapObject
    {
        // growth between 0 and 1
        public float GrowthValue = 0;

        public float Water;
        
        public float Fertilizer;

        public static float GrowthRate = 0.0325f;

        public static float WaterRate = -0.0285f;

        public static float FertilizerRate = -0.015f;

        public static Random PlantRandom = new Random();

        public bool FullyGrown = false;
        
        public static int FirstCoveringStage;

        protected List<Sprite> _sprites = new List<Sprite>();

        protected Sprite _lastGrowthStage;

        protected void AutoPopulateDictionary(string path, int highestNotFullyGrownStage)
        {
            OccupiesTiles = true;
            for (int i = 0; i <= highestNotFullyGrownStage; i++)
            {
                _sprites.Add(new Sprite(FNATextureHelper.Load(path + i, Game.Content)));
            }
            _lastGrowthStage = new Sprite(FNATextureHelper.Load(path + (highestNotFullyGrownStage + 1), Game.Content));
        }

        public override void OnRandomTick(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // use random coefficients to make the game... well, more random
            GrowthValue = Math.Clamp(GrowthValue + (float)(PlantRandom.NextDouble() * 0.85f + 0.15f) * (GrowthRate * ((Water * 0.9f) + 0.1f) * ((Fertilizer * 0.20f) + 1f)), 0f, 1f);
            if(GrowthValue == 1f)
            {
                FullyGrown = true;
            }
            float currentWaterRate = (float)(PlantRandom.NextDouble() * 0.3f + 0.7f) * WaterRate;
            Water = Math.Clamp(Water + (FullyGrown ? currentWaterRate : currentWaterRate * 0.7f), 0f, 1f);
            float currentFertilizerRate = (float)(PlantRandom.NextDouble() * 0.28f + 0.72f) * FertilizerRate;
            Fertilizer = Math.Clamp(Fertilizer + (FullyGrown ? currentFertilizerRate : currentFertilizerRate * 0.5f), 0f, 1f);
            int growthStage = (int)Math.Round(GrowthValue * (_sprites.Count - 1));
            if(FullyGrown)
            {
                CanCoverMapObjects = growthStage >= FirstCoveringStage;
                SpriteRenderer.SetSprite(_lastGrowthStage);
                return;
            }
            CanCoverMapObjects = growthStage >= FirstCoveringStage;
            SpriteRenderer.SetSprite(_sprites[(int)Math.Round(GrowthValue * (_sprites.Count - 1))]);
        }
    }
}