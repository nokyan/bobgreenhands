using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
using BobGreenhands.Utils.CultureUtils;
using BobGreenhands.Persistence;
using BobGreenhands.Utils;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;


namespace BobGreenhands.Scenes
{
    // TODO: Everything

    /// <summary>
    /// The player can select their savegame here
    /// </summary>
    public class PlayMenu : MovingBackgroundScene, IInputProcessor
    {

        private ListBox<string> _list = new ListBox<string>(new ListBoxStyle(Game.NormalSkin.NormalFont, new Color(255, 255, 255), new Color(128, 128, 128), new PrimitiveDrawable(new Color(0, 0, 0, 0)))
        {
            Background = new PrimitiveDrawable(new Color(0, 0, 0, 160))
        });

        private TextButton _playButton = new TextButton(Language.Translate("play"), Game.NormalSkin);
        private TextButton _renameButton = new TextButton(Language.Translate("rename"), Game.NormalSkin);
        private TextButton _deleteButton = new TextButton(Language.Translate("delete"), Game.NormalSkin);

        // this will come in handy when we work with files that have characters in their names that have been filtered out
        private Dictionary<string, string> _realFileNames = new Dictionary<string, string>();

        private Table _table;

        public PlayMenu() : base()
        {
            _playButton.GetLabel().SetFontScale(3f);
            _renameButton.GetLabel().SetFontScale(3f);
            _deleteButton.GetLabel().SetFontScale(3f);
            
            _table = UICanvas.Stage.AddElement(new Table());
            _table.SetFillParent(true);
            _table.Add(new Label(Language.Translate("selectSavegame"), Game.NormalSkin).SetFontScale(6f)).SetFillX().SetExpandX().SetPadTop(25f).SetPadRight(25f).SetPadLeft(25f);
            _table.Row();

            RefreshList();
            ScrollPane scrollPane = new ScrollPane(_list);
            scrollPane.SetScrollSpeed(1f);
            scrollPane.SetSmoothScrolling(true);
            Container scrollPaneContainer = new Container();
            scrollPaneContainer.AddElement(scrollPane);
            scrollPaneContainer.SetScale(4f);
            scrollPaneContainer.FillParent = true;
            _table.Add(scrollPane).Expand().Fill().Space(25f);
            Game.SubscribeToInputHandler(this);

            _table.Row();

            Table topButtonTable = _table.Add(new Table()).SetExpandX().SetFillX().SetSpaceTop(25f).SetSpaceBottom(10f).GetElement<Table>();
            topButtonTable.Add(_playButton).Space(10f).SetFillX().GetElement<TextButton>();
            topButtonTable.Add(_renameButton).Space(10f).SetFillX().GetElement<TextButton>();
            topButtonTable.Add(_deleteButton).Space(10f).SetFillX().GetElement<TextButton>();

            _table.Row();

            Table bottomButtonTable = _table.Add(new Table()).SetExpandX().SetFillX().SetPadBottom(25f).GetElement<Table>();
            TextButton newButton = bottomButtonTable.Add(new TextButton(Language.Translate("new"), Game.NormalSkin)).Space(10f).SetFillX().GetElement<TextButton>();
            newButton.GetLabel().SetFontScale(3f);
            TextButton refreshButton = bottomButtonTable.Add(new TextButton(Language.Translate("refresh"), Game.NormalSkin)).Space(10f).SetFillX().GetElement<TextButton>();
            refreshButton.GetLabel().SetFontScale(3f);
            TextButton backButton = bottomButtonTable.Add(new TextButton(Language.Translate("back"), Game.NormalSkin)).Space(10f).SetFillX().GetElement<TextButton>();
            backButton.GetLabel().SetFontScale(3f);

            newButton.OnClicked += NewButton_onClicked;
            _playButton.OnClicked += PlayButton_onClicked;
            _deleteButton.OnClicked += DeleteButton_onClicked;
            refreshButton.OnClicked += RefreshButton_onClicked;
            backButton.OnClicked += BackButton_onClicked;
        }

        public void RefreshList()
        {
            _realFileNames.Clear();
            List<string> output = new List<string>();
            Dictionary<string, int> counter = new Dictionary<string, int>();
            foreach (Savegame s in Game.GameFolder.Savegames)
            {
                string filename = Path.GetFileName(s.Path);
                string cleanFilename = Game.NormalSkin.Filter(filename);
                // if the list contains multiple instances of the same cleanFileName, enumerate them
                if (output.Contains(cleanFilename))
                {
                    if(counter.ContainsKey(cleanFilename))
                        counter[cleanFilename] = counter[cleanFilename] + 1;
                    else
                        counter[cleanFilename] = 1;
                    cleanFilename += String.Format(" ({0})", counter[cleanFilename]);
                }
                cleanFilename = "  " + cleanFilename.Replace(".bgs", "");
                output.Add(cleanFilename);
                _realFileNames[cleanFilename] = Path.GetFileName(filename);
            }
            output.Sort();
            _list.SetItems(output);
            _playButton.SetDisabled(output.Count == 0);
            _renameButton.SetDisabled(output.Count == 0);
            _deleteButton.SetDisabled(output.Count == 0);
        }

        private void NewButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new NewSavegame()));
        }

        private void PlayButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new PlayScene(Game.GameFolder.Savegames.Find(x => Path.GetFileName(x.Path).Equals(_realFileNames[_list.GetSelected()])))));
        }

        private void DeleteButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new DeletionConfirm(_realFileNames[_list.GetSelected()], _list.GetSelected())));
        }

        private void RefreshButton_onClicked(Button obj)
        {
            obj.Toggle();
            Game.GameFolder.RefreshSavegameList();
            RefreshList();
        }

        private void BackButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new MainMenu()));
        }

        public void FirstExtendedMousePressed()
        {
        }

        public void FirstExtendedMouseReleased()
        {
        }

        // you can use the arrow keys to navigate the list
        public void KeyPressed(Keys key)
        {
            // f5 for refresh
            if (key == Keys.F5)
            {
                Game.GameFolder.RefreshSavegameList();
                RefreshList();
            }
            // arrow keys for navigation
            if (_list.GetItems().Count > 1) {
                if (key == Keys.Up)
                {
                    if (_list.GetSelectedIndex() == 0)
                    {
                        _list.SetSelectedIndex(_list.GetItems().Count - 1);
                    }
                    else
                    {
                        _list.SetSelectedIndex(_list.GetSelectedIndex() - 1);
                    }
                }
                else if (key == Keys.Down)
                {
                    if (_list.GetSelectedIndex() == _list.GetItems().Count - 1)
                    {
                        _list.SetSelectedIndex(0);
                    }
                    else
                    {
                        _list.SetSelectedIndex(_list.GetSelectedIndex() + 1);
                    }
                }
            }
        }

        public void KeyReleased(Keys key)
        {
        }

        public void LeftMousePressed()
        {
        }

        public void LeftMouseReleased()
        {
        }

        public void MiddleMousePressed()
        {
        }

        public void MiddleMouseReleased()
        {
        }

        public void MouseMoved(Point delta)
        {
        }

        public void MouseScrolled(int delta)
        {
        }

        public void RightMousePressed()
        {
        }

        public void RightMouseReleased()
        {
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