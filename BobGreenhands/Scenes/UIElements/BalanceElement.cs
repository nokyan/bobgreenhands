using System;
using BobGreenhands.Utils;
using BobGreenhands.Utils.CultureUtils;
using Microsoft.Xna.Framework;
using Nez.UI;


namespace BobGreenhands.Scenes.UIElements
{
    /// <summary>
    /// Small UI Element that is used to display the player's current balance
    /// </summary>
    public class BalanceElement : Table, IInputListener, ISelectionBlocking
    {
        private Label _label;

        private bool _hoveredOver;

        public BalanceElement(double balance)
        {
            SetTouchable(Touchable.Enabled);
            SetBackground(new SpriteDrawable(FNATextureHelper.Load("img/ui/normal/balance_element", Game.Content)));
            _label = new Label(_generateString(balance), Game.NormalSkin);
            _label.SetAlignment(Nez.UI.Align.Right);
            Add(_label).SetPadLeft(4f).SetPadRight(4f).Expand().Right();
        }

        public void SetBalance(double balance)
        {
            _label.SetText(_generateString(balance));
        }

        private string _generateString(double balance)
        {
            return String.Format(Language.CultureInfo, "힧 {0:0.00}", (Math.Truncate(balance * 100) / 100));
        }

        public void OnMouseEnter()
        {
            _hoveredOver = true;
        }

        public void OnMouseExit()
        {
            _hoveredOver = false;
        }

        public bool OnMousePressed(Vector2 mousePos)
        {
            return true;
        }

        public void OnMouseMoved(Vector2 mousePos)
        {
        }

        public void OnMouseUp(Vector2 mousePos)
        {
        }

        public bool OnMouseScrolled(int mouseWheelDelta)
        {
            return true;
        }

        public bool HoveredOver()
        {
            return _hoveredOver;
        }
    }
}