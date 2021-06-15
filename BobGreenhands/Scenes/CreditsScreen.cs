using System.Reflection.PortableExecutable;
using Nez;
using Nez.UI;
using BobGreenhands.Utils.CultureUtils;
using BobGreenhands.Skins;

namespace BobGreenhands.Scenes
{


    // IMPORTANT: In case you're forking this game, **DO NOT** change ANY of the already existing labels in this scene - you can add some, if you want, BUT DO NOT CHANGE THE ALREADY EXISTING ONES!
    public class CreditsScreen : MovingBackgroundScene
    {

        public CreditsScreen()
        {
            Table table = UICanvas.Stage.AddElement(new Table());
            table.SetFillParent(true);
            table.Pad(NormalSkin.OuterSpacing);
            table.Add(new Label(Language.Translate("mainMenu.credits"), Game.NormalSkin).SetWrap(true).SetFontScale(2f)).SetFillX().SetExpandX().SetSpaceBottom(NormalSkin.Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.programming") + "ManicRobot", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.art") + "ManicRobot & MarsFreak", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.music") + "Composed by https://www.onemansymphony.bandcamp.com (CC BY 4.0)", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.sounds") + "Kenney.nl (https://www.kenney.nl, CC0 1.0)", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.font") + "\"Pear Soda\" & \"Unbalanced\" by Font End Dev (https://www.fontenddev.com, CC BY 4.0) + GNU Unifont 12.1.04 (GNU GPL)", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.de-translation") + "ManicRobot", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.fr-translation") + "DeepL (https://www.deepl.com)", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.es-translation") + "DeepL (https://www.deepl.com)", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.nl-translation") + "DeepL (https://www.deepl.com)", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.nez") + "prime31 (https://www.github.com/prime31/Nez, MIT License)", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.nezFork") + "jamesjoplin (https://www.github.com/jamesjoplin/Nez, MIT License)", Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);
            table.Row();
            table.Add(new Label(Language.Translate("credits.license"), Game.NormalSkin).SetWrap(true).SetFontScale(1f)).SetFillX().SetExpandX().SetPadTop(NormalSkin.Spacing).SetSpaceBottom(NormalSkin.SubSpacing);

            table.Row();
            TextButton backButton = new TextButton(Language.Translate("gui.back"), Game.NormalSkin);
            backButton.GetLabel().SetFontScale(1f);
            table.Add(backButton).Expand().Bottom().Right();
            backButton.OnClicked += BackButton_onClicked;

            table.Row().SetPadTop(10);
        }

        private void BackButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new MainMenu()));
        }
    }
}