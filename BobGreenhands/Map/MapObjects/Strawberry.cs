using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Nez.Persistence;
using Nez.Sprites;
using BobGreenhands.Scenes;
using BobGreenhands.Utils.CultureUtils;

namespace BobGreenhands.Map.MapObjects
{

    public class Strawberry : Plant
    {

        public static new int FirstCoveringStage = 4;

        /// <summary>
        /// This constructor is supposed to be used by the Json Decoder.
        /// </summary>
        public Strawberry()
        {
            GrowthRate = 0.035f;
            Water = 0.7f;
            Fertilizer = 0.9f;
            GrowthRate = 0.035f;
            Hitbox = new Vector2(Game.TextureResolution, Game.TextureResolution);
        }

        /// <summary>
        /// Use this constructor for creating a new instance of Strawberry.
        /// </summary>
        public Strawberry(float x, float y, float growth, bool fullyGrown) : this()
        {
            GrowthValue = growth;
            FullyGrown = fullyGrown;
            Position = new Vector2(x, y);
            Initialize();
        }

        public override void Initialize()
        {
            AutoPopulateDictionary("img/mapobjects/plants/strawberry_", 6);
            if (!FullyGrown)
            {
                int growthStage = (int)Math.Round(GrowthValue * 6);
                SpriteRenderer.SetSprite(_sprites[growthStage]);
                CanCoverMapObjects = growthStage >= FirstCoveringStage;
            }
            else
            {
                SpriteRenderer.SetSprite(_lastGrowthStage);
                CanCoverMapObjects = true;
            }
            Location location = Location.FromEntityCoordinates(Position);
            SpriteRenderer.SetRenderLayer(PlayScene.MapObjectRenderLayer);
            AddComponent(SpriteRenderer);
        }

        public override void OnRandomTick(GameTime gameTime)
        {
            base.OnRandomTick(gameTime);
        }

        public override string GetInfoText()
        {
            string returnString = String.Format("{0}\n{1}",
                    Language.Translate("mapobject.strawberry.name"),
                    Language.Translate("game.quickInfo.plantInfo", (int) (Water*100), (int) (Fertilizer*100), (int) (GrowthValue*100)));
            return returnString;
        }

        public override void OnTick(GameTime gameTime)
        {
        }
    }


    public class StrawberryJsonConverter : JsonTypeConverter<Strawberry>
    {
        public override bool WantsExclusiveWrite => true;

        public override void OnFoundCustomData(Strawberry instance, string key, object value)
        {
        }

        public override void WriteJson(IJsonEncoder encoder, Strawberry value)
        {
            Location location = Location.FromEntityCoordinates(value.Position);
            encoder.EncodeKeyValuePair("XPos", location.X);
            encoder.EncodeKeyValuePair("YPos", location.Y);
            encoder.EncodeKeyValuePair("GrowthValue", value.GrowthValue);
            encoder.EncodeKeyValuePair("FullyGrown", value.FullyGrown);
            encoder.EncodeKeyValuePair("Water", value.Water);
            encoder.EncodeKeyValuePair("Fertilizer", value.Fertilizer);
        }
    }


    public class StrawberryObjectFactory : JsonObjectFactory<Strawberry>
    {
        public override Strawberry Create(Type objectType, IDictionary objectData)
        {
            Strawberry strawberry = new Strawberry();
            Location location = new Location(Convert.ToSingle(objectData["XPos"]), Convert.ToSingle(objectData["YPos"]));

            strawberry.Position = new Vector2(location.EntityX, location.EntityY);
            strawberry.GrowthValue = Convert.ToSingle(objectData["GrowthValue"]);
            strawberry.FullyGrown = Convert.ToBoolean(objectData["FullyGrown"]);
            strawberry.Water = Convert.ToSingle(objectData["Water"]);
            strawberry.Fertilizer = Convert.ToSingle(objectData["Fertilizer"]);

            return strawberry;
        }
    }
}