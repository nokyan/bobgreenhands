using System;
using System.Collections.Generic;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.UI;
using BobGreenhands.Utils.CultureUtils;
using BobGreenhands.Utils;
using BobGreenhands.Persistence;
using BobGreenhands.Map;
using BobGreenhands.Map.Tiles;
using BobGreenhands.Map.Items;
using BobGreenhands.Scenes.UIElements;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using BobGreenhands.Map.MapObjects;
using System.Threading;
using BobGreenhands.Scenes.ECS.Entities;


namespace BobGreenhands.Scenes
{
    public class PlayScene : BaseScene, IInputProcessor
    {

        public enum LockedState
        {
            None,
            TileLocked,
            MapObjectLocked,
            ItemLocked
        }

        public static Savegame CurrentSavegame;

        public static Dictionary<TileType, Texture2D> TileTextures = new Dictionary<TileType, Texture2D>();
        public static Dictionary<ItemType, Texture2D> ItemTextures = new Dictionary<ItemType, Texture2D>();

        public static bool CamPosLocked;

        public const int SelectedMapObjectRenderLayer = 6;
        public const int MapObjectRenderLayer = 5;
        public const int BackgroundRenderLayer = 4;
        public const int SelectedTileRenderLayer = 3;
        public const int MapRenderLayer = 2;
        public const int HotbarRenderLayer = 1;

        public const float RandomTickPercent = 0.001f;
        public const float GUIScale = 64f / Game.TextureResolution;

        public const float MaxCamZoom = 1.25f;
        public const float MinCamZoom = 0.25f;
        public const float CamZoomStep = 0.0625f;

        public static Sprite SelectedTileSprite;
        public static Sprite LockedTileSprite;
    
        private static LockedState _currentLockedState;
        public static LockedState CurrentLockedState
        {
            get
            {
                return _currentLockedState;
            }
            set
            {
                _currentLockedState = value;
                switch (value)
                {
                    case LockedState.None:
                        CamPosLocked = false;
                        _selectedTileSpriteRenderer.Sprite = SelectedTileSprite;
                        break;
                    case LockedState.TileLocked:
                        CamPosLocked = true;
                        _selectedTileSpriteRenderer.Sprite = LockedTileSprite;
                        break;
                    case LockedState.ItemLocked:
                        CamPosLocked = false;
                        _selectedTileSpriteRenderer.Sprite = SelectedTileSprite;
                        break;
                    default:
                        break;
                }
            }
        }

        public static int LockedIndex;

        public static Inventory? LockedInventory;

        public static List<ISelectionBlocking> SelectionBlockingUIElements = new List<ISelectionBlocking>();

        public Hotbar Hotbar;

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private static SpriteRenderer _selectedTileSpriteRenderer;

        private System.Threading.Tasks.Task _autoSave;

        private MapEntity _mapEntity;
        private Entity _selectedTileEntity;

        private Texture2D _mapTexture;

        private TiledDrawable _background;

        private Sprite _mapSprite;

        private SpriteRenderer _mapSpriteRenderer;
        private SpriteRenderer _hotbarRenderer;

        private Point _selectedTilePoint = Point.Zero;

        private int _zoom;

        private readonly float _maxCamSpeed = 4f * (Game.TextureResolution / 32f);
        private float _horizontalCamMovement;
        private float _verticalCamMovement;
        private float _maxCameraXPos;
        private float _maxCameraYPos;

        private List<MapObject> _selectionBlockingMapObjects = new List<MapObject>();

        private List<MapObject> _mapObjects = new List<MapObject>();

        private Bob Bob;

        private System.Random randomTick = new System.Random();

        public PlayScene(Savegame savegame)
        {
            if(Game.GameFolder.Settings.GetBool("vignette"))
            {
                VignettePostProcessor vignette = new VignettePostProcessor(0);
                vignette.Power = 0.5f;
                AddPostProcessor(vignette);
            }

            CurrentSavegame = savegame;
            savegame.Load();
            Game.SubscribeToInputHandler(this);

            // load all the textures
            // TODO: Make this somehow better
            SelectedTileSprite = new Sprite(FNATextureHelper.Load("img/ui/normal/selected_tile", Game.Content));
            LockedTileSprite = new Sprite(FNATextureHelper.Load("img/ui/normal/locked_tile", Game.Content));
            TileTextures[TileType.Unknown] = FNATextureHelper.Load("img/tiles/unknown", Game.Content);
            TileTextures[TileType.Grass] = FNATextureHelper.Load("img/tiles/grass", Game.Content);
            TileTextures[TileType.Farmland] = FNATextureHelper.Load("img/tiles/farmland", Game.Content);
            TileTextures[TileType.NSFence] = FNATextureHelper.Load("img/tiles/boundary_bush", Game.Content);
            TileTextures[TileType.WEFence] = FNATextureHelper.Load("img/tiles/boundary_bush", Game.Content);
            TileTextures[TileType.NEFence] = FNATextureHelper.Load("img/tiles/boundary_bush", Game.Content);
            TileTextures[TileType.ESFence] = FNATextureHelper.Load("img/tiles/boundary_bush", Game.Content);
            TileTextures[TileType.SWFence] = FNATextureHelper.Load("img/tiles/boundary_bush", Game.Content);
            TileTextures[TileType.WNFence] = FNATextureHelper.Load("img/tiles/boundary_bush", Game.Content);
            TileTextures[TileType.BoundaryBush] = FNATextureHelper.Load("img/tiles/boundary_bush", Game.Content);
            ItemTextures[ItemType.Unknown] = FNATextureHelper.Load("img/tiles/unknown", Game.Content);
            ItemTextures[ItemType.Shovel] = FNATextureHelper.Load("img/items/shovel", Game.Content);
            ItemTextures[ItemType.StrawberrySeeds] = FNATextureHelper.Load("img/items/strawberry_seeds", Game.Content);

            // set up the background
            Entity backgroundEntity = CreateEntity("background");
            RenderLayerRenderer backgroundLayerRenderer = new RenderLayerRenderer(0, BackgroundRenderLayer);
            AddRenderer(backgroundLayerRenderer);
            TiledSpriteRenderer tsr = new TiledSpriteRenderer(TileTextures[TileType.Grass]);
            tsr.SetRenderLayer(BackgroundRenderLayer);
            tsr.Height = 65536;
            tsr.Width = 65536;
            backgroundEntity.SetPosition(-tsr.Width/2f, -tsr.Height/2f);
            backgroundEntity.AddComponent(tsr);
            

            // build the map texture for the first time
            RenderLayerRenderer mapLayerRenderer = new RenderLayerRenderer(1, MapRenderLayer);
            AddRenderer(mapLayerRenderer);
            _mapEntity = new MapEntity();
            _mapEntity.SetPosition(-_mapEntity.Width/2f, -_mapEntity.Height/2f);
            AddEntity(_mapEntity);
            Camera.SetMaximumZoom(5);
            Camera.SetMinimumZoom(2);
            // TODO: Make zoom adjust to TextureResolution properly
            Camera.SetZoom(0.75f);
            RecalculateMaxCamPos();

            // init the selected tile sprite
            _selectedTileEntity = CreateEntity("selectedTile");
            RenderLayerRenderer selectedTileLayerRenderer = new RenderLayerRenderer(2, SelectedTileRenderLayer);
            AddRenderer(selectedTileLayerRenderer);
            _selectedTileSpriteRenderer = new SpriteRenderer(SelectedTileSprite);
            _selectedTileSpriteRenderer.SetRenderLayer(SelectedTileRenderLayer);
            _selectedTileEntity.AddComponent(_selectedTileSpriteRenderer);

            // ui stuff
            UICanvas.RenderLayer = BaseScene.UIRenderLayer;
            Table table = UICanvas.Stage.AddElement(new Table());
            table.SetFillParent(true);
            table.Pad(20);
            
            table.Add(new TextButton(Language.Translate("menu"), Game.NormalSkin)).Expand().Top().Right();
            table.Row();

            Hotbar = new Hotbar();
            SelectionBlockingUIElements.Add(Hotbar);
            table.Add(Hotbar).Left();

            // mapobjects!
            foreach(MapObject m in CurrentSavegame.SavegameData.MapObjectList)
            {
                _mapObjects.Add(m);
                m.Initialize();
                AddEntity(m);
            }
            RenderLayerRenderer mapObjectLayerRenderer = new RenderLayerRenderer(3, MapObjectRenderLayer);
            AddRenderer(mapObjectLayerRenderer);
            Bob = new Bob(CurrentSavegame.SavegameData.PlayerXPos, CurrentSavegame.SavegameData.PlayerYPos);
            _mapObjects.Add(Bob);
            AddEntity(Bob);
            _selectionBlockingMapObjects.Add(Bob);
            Camera.SetPosition(Bob.Position);

            RenderLayerRenderer selectedMapObjectRenderer = new RenderLayerRenderer(4, SelectedMapObjectRenderLayer);
            AddRenderer(selectedMapObjectRenderer);

            UpdateSelectedTile();

            // save the game every minute
            _autoSave = new System.Threading.Tasks.Task(() =>
            {
                {
                    while (true)
                    {
                        CurrentSavegame.Save();
                        _log.Debug("Saved the game (Auto-Save)");
                        Thread.Sleep(1000);
                    }
                }
            });
            _autoSave.Start();
        }

        ~PlayScene()
        {
            _autoSave.Dispose();
        }

        private void RecalculateMaxCamPos()
        {
            _maxCameraXPos = _mapEntity.Width * Camera.Zoom * 2;
            _maxCameraYPos = _mapEntity.Height * Camera.Zoom * 2;
        }

        public void RefreshMap()
        {
            _mapEntity.Refresh();
        }

        public List<MapObject> GetMapObjects()
        {
            return _mapObjects;
        }

        public void AddMapObject(MapObject mapObject)
        {
            List<MapObject> mapObjects = _mapObjects;
            mapObjects.Add(mapObject);
            _mapObjects = mapObjects;
            AddEntity(mapObject);
            CurrentSavegame.SavegameData.MapObjectList.Add(mapObject);
        }

        // TODO: make that a little bit more efficient
        public bool IsOccupiedByMapObject(float x, float y)
        {
            foreach (MapObject m in _mapObjects.ToArray())
            {
                if(!m.OccupiesTiles)
                    continue;
                Location minLocation = Location.FromEntityCoordinates(m.Position.X - m.SpriteRenderer.Origin.X, m.Position.Y - m.SpriteRenderer.Origin.Y);
                Location maxLocation = Location.FromEntityCoordinates(m.Position.X + (m.Hitbox.X - m.SpriteRenderer.Origin.X), m.Position.Y + (m.Hitbox.Y - m.SpriteRenderer.Origin.Y));
                if(x >= minLocation.X && x < maxLocation.X && y >= minLocation.Y && y < maxLocation.Y)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Update()
        {
            base.Update();
            // move camera
            float newXPos = Math.Clamp(Camera.Position.X - _horizontalCamMovement * (Time.DeltaTime * 60), -_maxCameraXPos, _maxCameraXPos);
            float newYPos = Math.Clamp(Camera.Position.Y - _verticalCamMovement * (Time.DeltaTime * 60), -_maxCameraYPos, _maxCameraYPos);
            if(!CamPosLocked) {
                if(newXPos != Camera.Position.X || newYPos != Camera.Position.Y)
                {
                    Camera.SetPosition(new Vector2(newXPos, newYPos));
                }
            }
            if (CurrentLockedState != LockedState.TileLocked)
            {
                UpdateSelectedTile();
            }


            List<MapObject> toBeRandomTicked = new List<MapObject>();
            foreach (MapObject m in _mapObjects.ToArray())
            {
                m.OnTick(Game.GameTime);
                // random tick
                // since tick rate is tied to the FPS, we have to figure out a way, to still have a reasonable random tick rate no matter the fps
                // we say, that with 60 FPS we want ``RandomTickPercent`` of the on-screen entities random ticked
                // since any computer built in this millennium is able to surpass that easily, we have to compensate for that
                if(randomTick.NextDouble() < RandomTickPercent * (Time.DeltaTime * 60f))
                {
                    toBeRandomTicked.Add(m);
                }
            }
            foreach (MapObject m in toBeRandomTicked)
            {
                m.OnRandomTick(Game.GameTime);
            }
        }

        /// <summary>
        /// Find out, if an UI-blocking entity is in the way
        /// </summary>
        private bool EntityIsBlocking()
        {
            foreach (ISelectionBlocking i in SelectionBlockingUIElements)
            {
                if (i.HoveredOver())
                    return true;
            }
            foreach (MapObject m in _selectionBlockingMapObjects)
            {
                SpriteRenderer spriteRenderer = m.SpriteRenderer;
                float minX = m.Position.X - spriteRenderer.Origin.X;
                float maxX = m.Position.X + (spriteRenderer.Width - spriteRenderer.Origin.X);
                float minY = m.Position.Y - spriteRenderer.Origin.Y;
                float maxY = m.Position.Y + (spriteRenderer.Height - spriteRenderer.Origin.Y);
                if(Camera.MouseToWorldPoint().X > minX && Camera.MouseToWorldPoint().X < maxX && Camera.MouseToWorldPoint().Y > minY && Camera.MouseToWorldPoint().Y < maxY)
                    return true;
            }
            return false;
        }

        private void UpdateSelectedTile()
        {

            if (EntityIsBlocking())
            {
                _selectedTileEntity.Enabled = false;
                return;
            }

            _selectedTileEntity.Enabled = true;
            int xMapPos = (int)Math.Floor(Camera.MouseToWorldPoint().X / Game.TextureResolution) * Game.TextureResolution;
            int yMapPos = (int)Math.Floor(Camera.MouseToWorldPoint().Y / Game.TextureResolution) * Game.TextureResolution;

            _selectedTilePoint.X = xMapPos / Game.TextureResolution + (int)Math.Floor(CurrentSavegame.SavegameData.MapWidth / 2f);
            _selectedTilePoint.Y = yMapPos / Game.TextureResolution + (int)Math.Floor(CurrentSavegame.SavegameData.MapHeight / 2f);

            if(_selectedTilePoint.X < 0)
            {
                _selectedTileEntity.Enabled = false;
            }
            else if(_selectedTilePoint.X > CurrentSavegame.SavegameData.MapWidth - 1)
            {
                _selectedTileEntity.Enabled = false;
            }

            if (_selectedTilePoint.Y < 0)
            {
                _selectedTileEntity.Enabled = false;
            }
            else if (_selectedTilePoint.Y > CurrentSavegame.SavegameData.MapHeight - 1)
            {
                _selectedTileEntity.Enabled = false;
            }

            _selectedTileEntity.SetPosition(xMapPos + Game.TextureResolution/2, yMapPos + Game.TextureResolution/2);

        }

        public static void SetLockedItem(InventoryItem? item)
        {
            if (item == null)
            {
                if (LockedInventory != null)
                    LockedInventory.GetItemAt(LockedIndex).Locked.SetVisible(false);
                CurrentLockedState = LockedState.None;
                LockedInventory = null;
                LockedIndex = -1;
            }
            else
            {
                CurrentLockedState = LockedState.ItemLocked;
                LockedInventory = item.Inventory;
                LockedIndex = item.Index;
                item.Locked.SetVisible(true);
            }
        }

        public void FirstExtendedMousePressed()
        {
        }

        public void FirstExtendedMouseReleased()
        {
        }

        public void KeyPressed(Keys key)
        {
        }

        public void KeyReleased(Keys key)
        {
            if (key == Keys.Z)
            {
                Bob.ResetTasks();
            }
            else if (key == Keys.S && Input.IsKeyDown(Keys.LeftControl))
            {
                CurrentSavegame.Save();
            }
            else if (key == Keys.F9)
            {
                Camera.Zoom = Math.Clamp(Camera.Zoom - CamZoomStep, MinCamZoom, MaxCamZoom);
                RecalculateMaxCamPos();
                if (MathUtils.IsBetween(Camera.Position.X, -_maxCameraXPos, _maxCameraXPos) && MathUtils.IsBetween(Camera.Position.Y, -_maxCameraYPos, _maxCameraYPos))
                {
                    Camera.Position = Vector2.Zero;
                }

            }
            else if (key == Keys.F10)
            {
                Camera.Zoom = Math.Clamp(Camera.Zoom + CamZoomStep, MinCamZoom, MaxCamZoom);
                RecalculateMaxCamPos();
                if (MathUtils.IsBetween(Camera.Position.X, -_maxCameraXPos, _maxCameraXPos) && MathUtils.IsBetween(Camera.Position.Y, -_maxCameraYPos, _maxCameraYPos))
                {
                    Camera.Position = Vector2.Zero;
                }
            }
        }

        public void LeftMousePressed()
        {
        }

        public void LeftMouseReleased()
        {
            if(CurrentLockedState == LockedState.None && !EntityIsBlocking())
            {
                CurrentLockedState = LockedState.TileLocked;
            }
            else if(CurrentLockedState == LockedState.TileLocked && !EntityIsBlocking())
            {
                CurrentLockedState = LockedState.None;
            }
            else if(CurrentLockedState == LockedState.ItemLocked && !EntityIsBlocking())
            {
                Item item = LockedInventory.GetItemAt(LockedIndex).Item;
                int x = _selectedTilePoint.X;
                int y = _selectedTilePoint.Y;
                // if mouse is out of bounds
                if(x < 0 || x >= CurrentSavegame.SavegameData.MapWidth || y < 0 || y >= CurrentSavegame.SavegameData.MapWidth)
                    return;
                TileType tileType = CurrentSavegame.GetTileAt(x, y);
                Vector2 target = new Vector2(Camera.MouseToWorldPoint().X, Camera.MouseToWorldPoint().Y);
                Location location = Location.FromEntityCoordinates(target).SetToCenterOfTile();
                Action function = () => {if(item.UsedOnTile(x, y, tileType, this)) { RefreshMap(); Hotbar.RefreshTexts(); }};
                Bob.EnqueueTask(new Task(location.EntityCoordinates, function));
                Bob.IsMoving = true;
            }
        }

        public void MiddleMousePressed()
        {
        }

        public void MiddleMouseReleased()
        {
        }

        public void MouseMoved(Point delta)
        {
            int xPos = Input.RawMousePosition.X;
            int yPos = Input.RawMousePosition.Y;

            // check if cursor is out of the window or if an entity is blocking
            if (xPos < 0 || xPos > Screen.Width || yPos < 0 || yPos > Screen.Height || EntityIsBlocking())
                return;

            float horizontalThreshhold = Screen.Width / 20f;
            float verticalThreshhold = Screen.Height / 20f;

            _horizontalCamMovement = 0;
            _verticalCamMovement = 0;

            if (xPos < horizontalThreshhold)
                _horizontalCamMovement = (1 - (xPos / horizontalThreshhold)) * _maxCamSpeed;
            if (yPos < verticalThreshhold)
                _verticalCamMovement = (1 - (yPos / verticalThreshhold)) * _maxCamSpeed;
            if (xPos > Screen.Width - horizontalThreshhold)
                _horizontalCamMovement = -(1 - ((Screen.Width - xPos) / horizontalThreshhold)) * _maxCamSpeed;
            if (yPos > Screen.Height - verticalThreshhold)
                _verticalCamMovement = -(1 - ((Screen.Height - yPos) / verticalThreshhold)) * _maxCamSpeed;

            if(CurrentLockedState != LockedState.TileLocked)
                UpdateSelectedTile();
        }

        public void MouseScrolled(int delta)
        {
        }

        public void RightMousePressed()
        {
        }

        public void RightMouseReleased()
        {
            if(CurrentLockedState == LockedState.None && !EntityIsBlocking())
            {
                Vector2 destination = new Vector2(Camera.MouseToWorldPoint().X, Camera.MouseToWorldPoint().Y);
                Location location = Location.FromEntityCoordinates(destination);
                // if mouse is out of bounds
                if(location.X < 0 || location.X >= CurrentSavegame.SavegameData.MapWidth || location.Y < 0 || location.Y >= CurrentSavegame.SavegameData.MapWidth)
                    return;
                Bob.EnqueueTask(new Task(destination, null));
                Bob.IsMoving = true;
            }
        }

        public void ScaledMouseMoved(Vector2 delta)
        {
        }

        public void SecondExtendedMousePressed()
        {
        }

        public void SecondExtendedMouseReleased()
        {
        }
    }
}