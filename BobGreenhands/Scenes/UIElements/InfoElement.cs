using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.UI;


namespace BobGreenhands.Scenes.UIElements
{
    public class InfoElement : Stack, ISelectionBlocking, IInputListener
    {

        private Table _table;

        private Image _backgroundImage;

        private Image _image;

        private Label _label;

        private bool _hoveredOver;

        public InfoElement(Texture2D texture)
        {
            PlayScene.SelectionBlockingUIElements.Add(this);
            _backgroundImage = new Image(texture);
            _backgroundImage.SetScale(PlayScene.GUIScale);
            _backgroundImage.SetSize(PlayScene.GUIScale * Game.TextureResolution, PlayScene.GUIScale * Game.TextureResolution);
            Add(_backgroundImage);
            _table = new Table();
            _table.SetDebug(true);
            _table.Pad(PlayScene.GUIScale * 4f);
            _image = new Image();
            _image.SetScale(PlayScene.GUIScale);
            _image.SetScaling(Scaling.Fit);
            // use a stack to first put a transparent PrimitiveDrawable on it and then the actual image,
            // this way we avoid weird scaling and position issues from non-square sprites, without
            // having to manually build a new Texture2D or something like that.
            Stack stack = new Stack();
            stack.Add(new Image(new PrimitiveDrawable(2f * PlayScene.GUIScale * Game.TextureResolution, 2f * PlayScene.GUIScale * Game.TextureResolution, new Color(0, 0, 0, 0))));
            stack.Add(_image);
            _label = new Label("", Game.NormalSkin);
            // TODO: find a workaround for proper word wrapping
            _label.SetWrap(false).SetFontScale(1f);
            _table.Add(stack).Pad(PlayScene.GUIScale * 4f).SetPadRight(PlayScene.GUIScale * 11f);
            _table.Add(new Container(_label).SetAlign(Align.Left).SetFill(true)).Pad(PlayScene.GUIScale * 4f).Expand().Fill().Left().Top();
            Add(_table);
        }

        public void SetImage(Texture2D texture)
        {
            _image.SetDrawable(new SpriteDrawable(texture));
            _image.SetScale(2f * PlayScene.GUIScale / (texture.Height / Game.TextureResolution));
        }

        public void SetText(string text)
        {
            _label.SetText(text);
            _label.Layout();
        }

        public bool HoveredOver()
        {
            return _hoveredOver;
        }

        public void OnMouseEnter()
        {
            _hoveredOver = true;
        }

        public void OnMouseExit()
        {
            _hoveredOver = false;
        }

        public void OnMouseMoved(Vector2 mousePos)
        {
        }

        public bool OnMousePressed(Vector2 mousePos)
        {
            return false;
        }

        public bool OnMouseScrolled(int mouseWheelDelta)
        {
            return false;
        }

        public void OnMouseUp(Vector2 mousePos)
        {
        }
    }
}