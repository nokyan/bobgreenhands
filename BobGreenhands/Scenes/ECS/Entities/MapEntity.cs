using BobGreenhands.Scenes.ECS.Components;
using Nez;


namespace BobGreenhands.Scenes.ECS.Entities
{
    public class MapEntity : Entity
    {
        public float Width
        {
            get
            {
                return MapRenderer.Width;
            }
        }

        public float Height
        {
            get
            {
                return MapRenderer.Height;
            }
        }

        public MapRenderer MapRenderer;
        
        public MapEntity()
        {
            MapRenderer = new MapRenderer();
            MapRenderer.SetRenderLayer(PlayScene.MapRenderLayer);
            AddComponent(MapRenderer);
        }

        public void Refresh()
        {
            MapRenderer.Refresh();
        }
    }
}