using System.Reflection.PortableExecutable;
using Nez;
using Nez.UI;
using BobGreenhands.Utils.CultureUtils;


namespace BobGreenhands.Scenes
{


    // IMPORTANT: In case you're forking this game, **DO NOT** change ANY of the already existing labels in this scene - you can add some, if you want, BUT DO NOT CHANGE THE ALREADY EXISTING ONES!
    public class CreditsScreen : MovingBackgroundScene
    {

        public static readonly float Spacing = 10f;

        public CreditsScreen()
        {
            Table table = UICanvas.Stage.AddElement(new Table());
            table.SetFillParent(true);
            table.Pad(25);
            table.Add(new Label(Language.Translate("credits"), Game.NormalSkin).SetWrap(true).SetFontScale(6f)).SetFillX().SetExpandX().SetSpaceBottom(25f);
            table.Row();
            table.Add(new Label(Language.Translate("programming") + "ManicRobot", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("art") + "ManicRobot & MarsFreak", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("music") + "James Hammond (https://www.jameshammondrf.bandcamp.com, CC BY-SA 3.0)", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("sounds") + "Kenney.nl (https://www.kenney.nl, CC0 1.0)", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("font") + "\"Pear Soda\" & \"Unbalanced\" by Font End Dev (https://www.fontenddev.com, CC BY 4.0) + GNU Unifont 12.1.04 (GNU GPL)", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("de-translation") + "ManicRobot", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("fr-translation") + "DeepL (https://www.deepl.com)", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("es-translation") + "DeepL (https://www.deepl.com)", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("nl-translation") + "DeepL (https://www.deepl.com)", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("nez") + "prime31 (https://www.github.com/prime31/Nez, MIT License)", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("nezFork") + "jamesjoplin (https://www.github.com/jamesjoplin/Nez, MIT License)", Game.NormalSkin).SetWrap(true).SetFontScale(3f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);
            table.Row();
            table.Add(new Label(Language.Translate("license"), Game.NormalSkin).SetWrap(true).SetFontScale(2f)).SetFillX().SetExpandX().SetPadTop(Spacing).SetSpaceBottom(Spacing);

            table.Row();
            TextButton backButton = new TextButton(Language.Translate("back"), Game.NormalSkin);
            backButton.GetLabel().SetFontScale(3f);
            table.Add(backButton).Expand().Bottom().Right();
            backButton.OnClicked += BackButton_onClicked;

            table.Row().SetPadTop(40);
        }

        private void BackButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new MainMenu()));
        }
    }
}