using Nez;
using Nez.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Nez.Textures;


namespace BobGreenhands.Scenes
{
    /// <summary>
    /// In order to keep the movement and position of the background image consistent (and to avoid redundant code), scenes wanting to use said image should inherit from this class.
    /// </summary>
    public abstract class MovingBackgroundScene : BaseScene
    {

        public static readonly float _backgroundSpeedMultiplier = 1f;

        protected static float _backgroundX = 0;
        protected static float _backgroundY = 0;
        protected static float _backgroundThreshold;
        protected static float _textureScale;

        protected static bool _backgroundMoveReverse = false;

        protected static Image _backgroundDrawable;

        public MovingBackgroundScene() : base()
        {
            UICanvas.RenderLayer = -10;
            UICanvas backgroundUICanvas = CreateEntity("ui-canvas").AddComponent(new UICanvas());
            // scale the background texture, if not already happened
            // we do it this way because since switching to FNA, _backgroundDrawable.SetScale(_textureScale) just didn't work anymore
            if(_backgroundDrawable == null)
            {
                Texture2D background = Game.Content.LoadTexture("img/background", true);
                _textureScale = Math.Max(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height) * 3 / background.Bounds.GetSize().X;
                RenderTarget2D renderTarget = RenderTarget.Create((int)(background.Bounds.GetSize().X * _textureScale), (int)(background.Bounds.GetSize().Y * _textureScale));
                Game.GraphicsDevice.SetRenderTarget(renderTarget);
                SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
                // without these options we would have antialiasing applied on the background texture, ew
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                spriteBatch.Draw(background, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, _textureScale, SpriteEffects.None, 1f);
                spriteBatch.End();
                spriteBatch.Dispose();
                Color[] texdata = new Color[renderTarget.Width * renderTarget.Height];
                renderTarget.GetData(texdata);
                Texture2D output = new Texture2D(Game.GraphicsDevice, (int)(background.Bounds.GetSize().X * _textureScale), (int)(background.Bounds.GetSize().Y * _textureScale));
                output.SetData(texdata);
                renderTarget.Dispose();
                _backgroundDrawable = new Image(new SpriteDrawable(output), Scaling.None);
                Game.GraphicsDevice.SetRenderTarget(null);
                _backgroundThreshold = Math.Min(_backgroundDrawable.GetWidth() - Game.GraphicsDevice.Viewport.Width, _backgroundDrawable.GetHeight() - Game.GraphicsDevice.Viewport.Height);
                _backgroundDrawable.SetPosition(_backgroundX, _backgroundY);
            }
            backgroundUICanvas.Stage.AddElement(_backgroundDrawable);
            // if either of the coordiantes of the background image at this point, the velocity of the background image should reverse
        }

        public override void Update()
        {
            base.Update();
            // what this mess is supposed to do is to reverse the velocity of the background image once one of its ends hits an edge of the window.
            // the threshold has been declared in the constructor.
            if (_backgroundMoveReverse)
            {
                if (_backgroundDrawable.GetX() >= 0 || _backgroundDrawable.GetY() >= 0)
                {
                    _backgroundMoveReverse = !_backgroundMoveReverse;
                    return;
                }
                _backgroundDrawable.SetPosition(_backgroundDrawable.GetX() + 0.25f * _backgroundSpeedMultiplier * Time.DeltaTime * 60, _backgroundDrawable.GetY() + 0.5f * _backgroundSpeedMultiplier * Time.DeltaTime * 60);
            }
            else
            {
                if (Math.Abs(_backgroundDrawable.GetX()) >= _backgroundThreshold || Math.Abs(_backgroundDrawable.GetY()) >= _backgroundThreshold)
                {
                    _backgroundMoveReverse = !_backgroundMoveReverse;
                    return;
                }
                _backgroundDrawable.SetPosition(_backgroundDrawable.GetX() - 0.25f * _backgroundSpeedMultiplier * Time.DeltaTime * 60, _backgroundDrawable.GetY() - 0.5f * _backgroundSpeedMultiplier * Time.DeltaTime * 60);
            }
            _backgroundX = _backgroundDrawable.GetX();
            _backgroundY = _backgroundDrawable.GetY();
        }
    }
}