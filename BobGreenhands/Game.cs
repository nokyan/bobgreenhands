using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using BobGreenhands.Scenes;
using BobGreenhands.Skins;
using Microsoft.Xna.Framework.Media;
using Nez;
using Nez.UI;
using System.Globalization;
using BobGreenhands.Utils.CultureUtils;
using BobGreenhands.Utils;
using BobGreenhands.Persistence;
using System;
using NLog;


namespace BobGreenhands
{
    public class Game : Core, IInputProcessor
    {

        public CultureInfo CultureInfo
        {
            get; set;
        }

        public static GameFolder GameFolder
        {
            get; private set;
        }

        public static NormalSkin NormalSkin
        {
            get; private set;
        }

        public static GameTime GameTime;

        public const int TextureResolution = 16;

        private float _crashCounter;

        private Song _song;

        // this list contains all the subscribers to our input """handler""" in Update()
        private static List<IInputProcessor> _inputSubscribers = new List<IInputProcessor>();

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public Game(GameFolder folder, int width, int height, bool fullscreen, bool vsync, bool debugEnabled) : base(width, height, fullscreen, true, Program.Name + " " + Program.Version.ToString(), "Content")
        {
            this.CultureInfo = new CultureInfo(folder.Settings.GetString("language"));
            ExitOnEscapeKeypress = false;
            Screen.SynchronizeWithVerticalRetrace = vsync;
            GameFolder = folder;
            SubscribeToInputHandler(this);
            DebugRenderEnabled = debugEnabled;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // resolution supported warnings
            // first, test if the resolution is less than 1280x720
            if (Screen.Width < 1920 || Screen.Height < 1080)
            {
                if (Screen.Width < 1280 || Screen.Height < 720)
                {
                    _log.Warn("Your resolution is less than 1280x720, UI elements (especially small ones) might become hard to read at this resolution!");
                }
                else
                {
                    _log.Warn("The design resolution of this game is 1920x1080, you might see some degradations regarding scaling!");
                }
            }
            // is the aspect ratio slimmer than 4:3?
            if (((float)Screen.Width / Screen.Height) < 4f / 3f)
            {
                _log.Warn("Your aspect ratio is slimmer than 4:3, UI elements might be cut off the screen!");
            }
            // is the aspect ratio wider than 21:9?
            if (((float)Screen.Width / Screen.Height) > 21f / 9f)
            {
                _log.Warn("Your aspect ratio is wider than 21:9, the game is not designed to run on monitors that wide, your experience might suffer from this!");
            }

            Language.Initialize(CultureInfo);

            NormalSkin = new NormalSkin();

            Scene = new MainMenu();

            BaseScene scene = (BaseScene)Scene;
            if (scene != null)
            {
                foreach (Element e in scene.UICanvas.Stage.GetElements().ToArray())
                {
                    if (e is Table t)
                    {
                        RecursiveToggleDebug(t, DebugRenderEnabled);
                    }
                }
            }

            _song = Content.Load<Song>("music/main");
            MediaPlayer.Volume = GameFolder.Settings.GetFloat("musicVolume") * GameFolder.Settings.GetFloat("totalVolume");
            MediaPlayer.Play(_song);
            MediaPlayer.IsRepeating = true;

            _log.Info("Finished initializing the game");

        }

        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            // manual crash trigger
            if (Input.IsKeyDown(Keys.F3) && Input.IsKeyDown(Keys.C))
            {
                _crashCounter += Time.DeltaTime;
            }
            else
            {
                _crashCounter = 0;
            }
            if (_crashCounter > 10)
            {
                _log.Warn("Manually invoking crash!");
                throw new Exception("Manually invoked crash.");
            }

            // Input """Handler"""
            // if there's no subscriber to the input handler, just don't do anything
            if (_inputSubscribers.Count > 0)
            {
                IInputProcessor[] procs = _inputSubscribers.ToArray();
                foreach (Keys k in Enum.GetValues(typeof(Keys)))
                {
                    if (Input.IsKeyPressed(k))
                    {
                        foreach (IInputProcessor i in procs)
                        {
                            i.KeyPressed(k);
                        }
                    }
                    if (Input.IsKeyReleased(k))
                    {
                        foreach (IInputProcessor i in procs)
                        {
                            i.KeyReleased(k);
                        }
                    }
                }
                if (Input.LeftMouseButtonPressed)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.LeftMousePressed();
                    }
                }
                if (Input.LeftMouseButtonReleased)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.LeftMouseReleased();
                    }
                }
                if (Input.RightMouseButtonPressed)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.RightMousePressed();
                    }
                }
                if (Input.RightMouseButtonReleased)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.RightMouseReleased();
                    }
                }
                if (Input.MiddleMouseButtonPressed)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.MiddleMousePressed();
                    }
                }
                if (Input.MiddleMouseButtonReleased)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.MiddleMouseReleased();
                    }
                }
                if (Input.FirstExtendedMouseButtonPressed)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.FirstExtendedMousePressed();
                    }
                }
                if (Input.FirstExtendedMouseButtonReleased)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.FirstExtendedMouseReleased();
                    }
                }
                if (Input.SecondExtendedMouseButtonPressed)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.SecondExtendedMousePressed();
                    }
                }
                if (Input.SecondExtendedMouseButtonReleased)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.SecondExtendedMouseReleased();
                    }
                }
                if (Input.MouseWheelDelta != 0)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.MouseScrolled(Input.MouseWheelDelta);
                    }
                }
                if (Input.MousePositionDelta.X != 0 || Input.MousePositionDelta.Y != 0)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.MouseMoved(Input.MousePositionDelta);
                    }
                }
                if (Input.ScaledMousePositionDelta.X != 0 || Input.ScaledMousePositionDelta.Y != 0)
                {
                    foreach (IInputProcessor i in procs)
                    {
                        i.ScaledMouseMoved(Input.ScaledMousePositionDelta);
                    }
                }
            }

            base.Update(gameTime);

        }

        public static void SubscribeToInputHandler(IInputProcessor proc)
        {
            if (_inputSubscribers.Contains(proc))
            {
                throw new ArgumentException(String.Format("{0} is already subscribed!", proc.ToString()));
            }
            _inputSubscribers.Add(proc);
        }

        public static void UnsubscribeFromInputHandler(IInputProcessor proc)
        {
            if (!_inputSubscribers.Contains(proc))
            {
                throw new ArgumentException(String.Format("{0} is not subscribed yet!", proc.ToString()));
            }
            _inputSubscribers.Remove(proc);
        }

        public static bool IsSubscribedToInputHandler(IInputProcessor proc)
        {
            return _inputSubscribers.Contains(proc);
        }

        public static void DiscordRpc()
        {
            // TODO: Do Discord RPC stuff
        }

        private void SaveScreenshot(Texture2D texture2D)
        {
            // TODO: Fix a bug where the game crashes when taking a screenshot in a non-fullscreen window (maybe implement own SaveAsPng function?)
            using (FileStream fs = File.Create(Path.Combine(GameFolder.ScreenshotsFolder, DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + ".png")))
            {
                texture2D.SaveAsPng(fs, Screen.Width, Screen.Height);
            }
            texture2D.Dispose();
        }

        public void KeyPressed(Keys key)
        {
        }

        public void KeyReleased(Keys key)
        {
            if (key == Keys.F2)
            {
                Scene.RequestScreenshot(SaveScreenshot);
            }
            if (key == Keys.F3)
            {
                DebugRenderEnabled = !DebugRenderEnabled;
                // enable/disable debug view for tables inside the scene
                BaseScene scene = (BaseScene)Scene;
                if (scene != null)
                {
                    foreach (Element e in scene.UICanvas.Stage.GetElements().ToArray())
                    {
                        if (e is Table t)
                        {
                            RecursiveToggleDebug(t, DebugRenderEnabled);
                        }
                    }
                }
            }
        }

        public static void RecursiveToggleDebug(Table table, bool enabled)
        {
            table.SetDebug(enabled);
            foreach (Element e in table.GetChildren().ToArray())
            {
                if (e is Table t)
                {
                    RecursiveToggleDebug(t, enabled);
                }
            }
        }

        public void LeftMousePressed()
        {
        }

        public void LeftMouseReleased()
        {
        }

        public void RightMousePressed()
        {
        }

        public void RightMouseReleased()
        {
        }

        public void MiddleMousePressed()
        {
        }

        public void MiddleMouseReleased()
        {
        }

        public void FirstExtendedMousePressed()
        {
        }

        public void FirstExtendedMouseReleased()
        {
        }

        public void SecondExtendedMousePressed()
        {
        }

        public void SecondExtendedMouseReleased()
        {
        }

        public void MouseScrolled(int delta)
        {
        }

        public void MouseMoved(Point delta)
        {
        }

        public void ScaledMouseMoved(Vector2 delta)
        {
        }
    }
}
