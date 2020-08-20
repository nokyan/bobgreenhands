using System.IO;
using Nez;
using Nez.UI;
using BobGreenhands.Utils.CultureUtils;
using BobGreenhands.Utils;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;


namespace BobGreenhands.Scenes
{
    public class DeletionConfirm : MovingBackgroundScene, IInputProcessor
    {

        private string _file;

        public DeletionConfirm(string file, string cleanFilename)
        {
            _file = file;

            Game.SubscribeToInputHandler(this);

            Table table = UICanvas.Stage.AddElement(new Table());
            table.SetFillParent(true);
            table.Pad(25);
            table.Add(new Label(Language.Translate("deletionWindowTitle"), Game.NormalSkin).SetWrap(true).SetFontScale(6f)).SetFillX().SetExpandX().SetSpaceBottom(25f);
            table.Row();
            table.Add(new Label(String.Format(Language.Translate("deletionConfirm"), cleanFilename.Remove(0, 2)), Game.NormalSkin).SetAlignment(Align.TopLeft).SetWrap(true).SetFontScale(4f)).Fill().Expand().Top().Space(25f);
            table.Row();

            Table buttonTable = table.Add(new Table()).SetSpaceBottom(25f).SetExpandX().GetElement<Table>();

            TextButton yesButton = new TextButton(Language.Translate("yes"), Game.NormalSkin);
            yesButton.GetLabel().SetFontScale(3f);
            buttonTable.Add(yesButton).SetExpandX().Bottom().Right().Space(10f);
            yesButton.OnClicked += YesButton_onClicked;

            TextButton noButton = new TextButton(Language.Translate("no"), Game.NormalSkin);
            noButton.GetLabel().SetFontScale(3f);
            buttonTable.Add(noButton).Bottom().Right().Space(10f);
            noButton.OnClicked += NoButton_onClicked;
        }

        private void YesButton_onClicked(Button obj)
        {
            obj.Toggle();
            File.Delete(Path.Combine(Game.GameFolder.SavegamesFolder, _file));
            Game.GameFolder.RefreshSavegameList();
            Game.UnsubscribeFromInputHandler(this);
            Core.StartSceneTransition(new WindTransition(() => new PlayMenu()));
        }

        private void NoButton_onClicked(Button obj)
        {
            obj.Toggle();
            Game.UnsubscribeFromInputHandler(this);
            Core.StartSceneTransition(new WindTransition(() => new PlayMenu()));
        }

        public void FirstExtendedMousePressed()
        {
        }

        public void FirstExtendedMouseReleased()
        {
        }

        public void KeyPressed(Keys key)
        {
            if (key == Keys.Escape)
            {
                Game.UnsubscribeFromInputHandler(this);
                Core.StartSceneTransition(new WindTransition(() => new PlayMenu()));
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