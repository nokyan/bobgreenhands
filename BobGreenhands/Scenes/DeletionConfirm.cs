using System.IO;
using Nez;
using Nez.UI;
using BobGreenhands.Utils.CultureUtils;
using BobGreenhands.Utils;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using BobGreenhands.Skins;

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
            table.Pad(NormalSkin.OuterSpacing);
            table.Add(new Label(Language.Translate("deletionWindowTitle"), Game.NormalSkin).SetWrap(true).SetFontScale(NormalSkin.HeadlineFontScale)).SetFillX().SetExpandX().SetSpaceBottom(NormalSkin.OuterSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("deletionConfirm", cleanFilename), Game.NormalSkin).SetAlignment(Align.TopLeft).SetWrap(true).SetFontScale(NormalSkin.NormalFontScale)).Fill().Expand().Top().Space(NormalSkin.OuterSpacing);
            table.Row();

            Table buttonTable = table.Add(new Table()).SetSpaceBottom(NormalSkin.OuterSpacing).SetExpandX().GetElement<Table>();

            TextButton yesButton = new TextButton(Language.Translate("yes"), Game.NormalSkin);
            yesButton.GetLabel().SetFontScale(NormalSkin.NormalFontScale);
            buttonTable.Add(yesButton).SetExpandX().Bottom().Right().Space(NormalSkin.Spacing);
            yesButton.OnClicked += YesButton_onClicked;

            TextButton noButton = new TextButton(Language.Translate("no"), Game.NormalSkin);
            noButton.GetLabel().SetFontScale(NormalSkin.NormalFontScale);
            buttonTable.Add(noButton).Bottom().Right().Space(NormalSkin.Spacing);
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