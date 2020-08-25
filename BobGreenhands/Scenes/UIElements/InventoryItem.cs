using System;
using BobGreenhands.Map.Items;
using BobGreenhands.Utils;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;


namespace BobGreenhands.Scenes.UIElements
{
    /// <summary>
    /// An Element to use in Inventory UIs
    /// </summary>
    public class InventoryItem : Stack, IInputListener, ISelectionBlocking
    {
        /// <summary>
        /// this font scale is relative to PlayScene.HotbarScale!
        /// </summary>
        public const float FontScale = 0.5f * (Game.TextureResolution / 16f);

        private Item? _item;
        public Item? Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                if(value != null)
                {
                    Image = new Image(PlayScene.ItemTextures[value.GetItemType()], Scaling.Stretch);
                    Image.SetScale(PlayScene.GUIScale);
                    Label.SetText(value.GetInfoString());
                    IsEmpty = false;
                }
                else
                {
                    Image = new Image(new PrimitiveDrawable(Game.TextureResolution, Game.TextureResolution, new Color(1f, 1f, 1f, 0f)));
                    Image.SetScale(PlayScene.GUIScale);
                    Label.SetText("");
                    IsEmpty = true;
                }
            }
        }

        public Image Selected;
        public Image Locked;

        public Image Image
        {
            get;
            private set;
        }

        public Label Label
        {
            get;
            private set;
        }

        public bool IsEmpty
        {
            get;
            private set;
        }

        public Inventory? Inventory
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        public bool HoverLocked = false;

        private bool _hoveredOver;

        public InventoryItem(Inventory? inventory, int index, Item? item = null) : base()
        {
            SetTouchable(Touchable.Enabled);
            PlayScene.SelectionBlockingUIElements.Add(this);
            Index = index;
            Inventory = inventory;
            Selected = new Image(PlayScene.SelectedTileSprite.Texture2D, Scaling.Fill);
            Locked = new Image(PlayScene.LockedTileSprite.Texture2D, Scaling.Fill);
            Selected.SetVisible(false);
            Locked.SetVisible(false);
            Container labelContainer = new Container();
            Label = new Label("", Game.NormalSkin);
            labelContainer.SetElement(Label);
            labelContainer.SetAlign(Align.BottomRight);
            Image = new Image();
            Label.SetFontScale(PlayScene.GUIScale * FontScale);
            Item = item;
            Add(Image);
            Add(labelContainer);
            Add(Selected);
            Add(Locked);
            IsEmpty = item == null;
        }

        ~InventoryItem()
        {
            PlayScene.SelectionBlockingUIElements.Remove(this);
        }

        public bool IsLocked()
        {
            return Inventory == PlayScene.LockedInventory && Inventory.GetIndexOf(this) == PlayScene.LockedIndex;
        }

        public void SetText(string? text)
        {
            Label.SetText(text);
        }

        public void OnMouseEnter()
        {
            _hoveredOver = true;
            if(Inventory != null)
                Inventory.SelectedIndex = Inventory.GetIndexOf(this);
                Selected.SetVisible(true);
                try
                {
                    PlayScene.InfoElement.SetImage(PlayScene.ItemTextures[Item.GetItemType()]);
                    PlayScene.InfoElement.SetText(Item.GetInfoText());
                    PlayScene.InfoElement.SetVisible(true);
                }
                catch (NullReferenceException) { }
        }

        public void OnMouseExit()
        {
            _hoveredOver = false;
            if(Inventory != null)
                Inventory.SelectedIndex = -1;
                Selected.SetVisible(false);
                PlayScene.InfoElement.SetVisible(false);
        }

        public bool OnMousePressed(Vector2 mousePos)
        {
            if(PlayScene.CurrentLockedState == PlayScene.LockedState.ItemLocked)
            {
                if(Inventory == null)
                    return true;
                // unselect
                if(Index == PlayScene.LockedIndex && Inventory == PlayScene.LockedInventory)
                {
                    PlayScene.SetLockedItem(null);
                }
                // else swap both items
                else
                {
                    // TODO: maybe do it with pointers/references to make it more efficient?
                    InventoryItem? previousItem = PlayScene.LockedInventory.GetItemAt(PlayScene.LockedIndex);
                    if(previousItem != null)
                    {
                        previousItem.Locked.SetVisible(false);
                        Locked.SetVisible(false);
                        PlayScene.LockedInventory.InsertItemAt(this.Item, PlayScene.LockedIndex);
                        Inventory.InsertItemAt(previousItem.Item, Index);
                    }
                    PlayScene.SetLockedItem(null);
                }
            }
            else
            {
                PlayScene.SetLockedItem(this);
            }
            return true;
        }

        public void OnMouseMoved(Vector2 mousePos)
        {
            if(IsEmpty)
                return;
        }

        public void OnMouseUp(Vector2 mousePos)
        {
            if(IsEmpty)
                return;
        }

        public bool OnMouseScrolled(int mouseWheelDelta)
        {
            return true;
        }

        public bool HoveredOver()
        {
            return _hoveredOver && !HoverLocked;
        }
    }
}