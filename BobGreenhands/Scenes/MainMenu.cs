using Nez;
using Nez.UI;
using BobGreenhands.Utils.CultureUtils;
using BobGreenhands.Skins;

namespace BobGreenhands.Scenes
{
    /// <summary>
    /// It's the main menu, what do you expect this comment to say?!
    /// </summary>
    public class MainMenu : MovingBackgroundScene
    {

        public readonly float MaxFontScale = 4f;
        public readonly float MinFontScale = 9/3f;
        public readonly float FontScalar = 0.025f/3;

        public readonly string Title = Program.Name.ToUpper();

        private float _currentFontScale = 10/3f;

        private bool _shrink = false;

        private Label _mainLabel;

        private Table _table;

        public MainMenu() : base()
        {
            _table = UICanvas.Stage.AddElement(new Table());
            _table.SetFillParent(true).Top().PadTop(2 * NormalSkin.OuterSpacing);

            _mainLabel = new Label(Title, Game.NormalSkin);
            _mainLabel.SetAlignment(Align.Center);

            _table.Add(_mainLabel.SetFontScale(_currentFontScale))
                    .SetMaxHeight(Game.NormalSkin.NormalFont.MeasureString(Title).Y * MaxFontScale)
                    .SetMinHeight(Game.NormalSkin.NormalFont.MeasureString(Title).Y * MaxFontScale)
                    .SetMaxWidth(Game.NormalSkin.NormalFont.MeasureString(Title).X * MaxFontScale)
                    .SetMinWidth(Game.NormalSkin.NormalFont.MeasureString(Title).X * MaxFontScale);

            _table.Row().SetPadTop(NormalSkin.OuterSpacing);

            _table.Add(new Label(Language.Translate("mainMenu.slogan"), Game.NormalSkin).SetFontScale(NormalSkin.HeadlineFontScale));

            _table.Row().SetPadTop(3 * NormalSkin.OuterSpacing);

            TextButton playButton = _table.Add(new TextButton(Language.Translate("mainMenu.play"), Game.NormalSkin)).GetElement<TextButton>();
            playButton.GetLabel().SetFontScale(NormalSkin.NormalFontScale);
            _table.Row().SetPadTop(NormalSkin.OuterSpacing);
            TextButton settingsButton = _table.Add(new TextButton(Language.Translate("mainMenu.settings"), Game.NormalSkin)).GetElement<TextButton>();
            settingsButton.GetLabel().SetFontScale(NormalSkin.NormalFontScale);
            _table.Row().SetPadTop(NormalSkin.OuterSpacing);
            TextButton exitButton = _table.Add(new TextButton(Language.Translate("mainMenu.exit"), Game.NormalSkin)).GetElement<TextButton>();
            exitButton.GetLabel().SetFontScale(NormalSkin.NormalFontScale);
            _table.Row().Expand();


            Table lowerTable = new Table();
            _table.Add(lowerTable).Bottom().Pad(NormalSkin.OuterSpacing).Expand().Fill().GetMaxWidth();
            TextButton creditsButton = new TextButton(Language.Translate("mainMenu.credits"), Game.NormalSkin);
            creditsButton.GetLabel().SetFontScale(NormalSkin.NormalFontScale);
            Container creditsButtonContainer = lowerTable.Add(new Container(creditsButton)).Expand().Left().Bottom().GetElement<Container>();
            Label versionLabel = new Label(Program.Version.ToString(), Game.NormalSkin);
            versionLabel.SetFontScale(NormalSkin.NormalFontScale);
            Container versionLabelContainer = lowerTable.Add(new Container(versionLabel)).Expand().Right().Bottom().GetElement<Container>();
            _table.Layout();

            playButton.OnClicked += PlayButton_onClicked;
            settingsButton.OnClicked += SettingsButton_onClicked;
            exitButton.OnClicked += ExitButton_onClicked;
            creditsButton.OnClicked += CredtisButton_onClicked;
        }

        private void PlayButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new PlayMenu()));
        }

        private void SettingsButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new SettingsMenu()));
        }

        private void ExitButton_onClicked(Button obj)
        {
            obj.Toggle();
            System.Environment.Exit(0);
        }

        private void CredtisButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new CreditsScreen()));
        }

        public override void Update()
        {
            base.Update();
            if (_shrink)
            {
                _currentFontScale -= FontScalar * Time.DeltaTime * 60;
                if (_currentFontScale <= MinFontScale)
                {
                    _shrink = false;
                }
            }
            else
            {
                _currentFontScale += FontScalar * Time.DeltaTime * 60;
                if (_currentFontScale >= MaxFontScale)
                {
                    _shrink = true;
                }
            }
            _mainLabel.SetFontScale(_currentFontScale);
        }

    }
}