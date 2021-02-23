using System;
using System.Collections.Generic;
using BobGreenhands.Map.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.UI;


namespace BobGreenhands.Scenes.UIElements
{
    /// <summary>
    /// Generic and dynamic Element to use as an inventory interface
    /// </summary>
    public class Inventory : Stack, IInputListener, ISelectionBlocking
    {
        public Image Background;

        public Table Table = new Table();

        public int Columns;
        
        public int Rows;

        public int SelectedIndex = -1;

        private List<InventoryItem> _items = new List<InventoryItem>();

        private bool _hoveredOver;

        public Inventory(Texture2D texture, int columns, int rows, params Item[] items)
        {
            SetTouchable(Touchable.Enabled);
            Background = new Image(texture, Scaling.Stretch);
            Background.SetScale(PlayScene.GUIScale);
            Add(Background);
            Columns = columns;
            Rows = rows;
            // populate the internal list with first all the available items, then with empty InventoryItems
            // later, add them to the Table
            for (int x = 0; x < columns * rows; x++)
            {
                InventoryItem i;
                if( x < items.Length)
                    i = new InventoryItem(this, x, items[x]);
                else
                    i = new InventoryItem(this, x);
                _items.Add(i);
            }
            RebuildTable();
        }

        public void Refresh()
        {
            foreach (InventoryItem i in _items)
            {
                if (i != null && !i.IsEmpty)
                {
                    i.SetText(i.Item.GetInfoString());
                    i.SetBarFloat(i.Item.GetInfoFloat());
                }
            }
        }

        public void RebuildTable()
        {
            Table.Remove();
            Table = new Table();
            Table.Left();
            Table.Pad(4f * PlayScene.GUIScale);
            for (int x = 0; x < _items.Count; x++)
            {
                // if we reached the end of a row, start a new row
                if (x != 0 && x % Columns == 0)
                Table.Row();
                Table.Add(_items[x]).Space(4f * PlayScene.GUIScale);
            }
            Add(Table);
        }

        public void AddItem(Item item)
        {
            int firstEmptySlot = -1;
            for (int x = 0; x < _items.Count; x++)
            {
                if (!_items[x].IsEmpty)
                    firstEmptySlot = x;
                    break;
            }
            if (firstEmptySlot == -1)
                throw new IndexOutOfRangeException("All inventory slots are full!");
            _items[firstEmptySlot] = new InventoryItem(this, firstEmptySlot, item);
        }

        public void InsertItemAt(Item? item, int index)
        {
            _items[index].HoverLocked = true;
            _items[index] = new InventoryItem(this, index, item);
            RebuildTable();
        }

        public void RemoveItemAt(int index)
        {
            _items[index].HoverLocked = true;
            _items[index] = new InventoryItem(this, index);
            RebuildTable();
        }

        public InventoryItem GetItemAt(int index)
        {
            return _items[index];
        }

        public InventoryItem? GetSelectedItem()
        {
            if(SelectedIndex == -1)
                return null;
            return _items[SelectedIndex];
        }

        public int GetIndexOf(InventoryItem item)
        {
            return _items.IndexOf(item);
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