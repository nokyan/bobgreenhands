using Nez;
using Nez.UI;
using BobGreenhands.Utils.CultureUtils;


namespace BobGreenhands.Scenes
{
    /// <summary>
    /// It's the main menu, what do you expect this comment to say?!
    /// </summary>
    public class MainMenu : MovingBackgroundScene
    {

        public readonly float MaxFontScale = 11f;
        public readonly float MinFontScale = 9f;
        public readonly float FontScalar = 0.025f;

        public readonly string Title = Program.Name.ToUpper();

        private float _currentFontScale = 10f;

        private bool _shrink = false;

        private Label _mainLabel;

        private Table _table;

        public MainMenu() : base()
        {
            _table = UICanvas.Stage.AddElement(new Table());
            _table.SetFillParent(true).Top().PadTop(50);

            _mainLabel = new Label(Title, Game.NormalSkin);
            _mainLabel.SetAlignment(Align.Center);

            _table.Add(_mainLabel.SetFontScale(_currentFontScale))
                    .SetMaxHeight(Game.NormalSkin.NormalFont.MeasureString(Title).Y * MaxFontScale)
                    .SetMinHeight(Game.NormalSkin.NormalFont.MeasureString(Title).Y * MaxFontScale)
                    .SetMaxWidth(Game.NormalSkin.NormalFont.MeasureString(Title).X * MaxFontScale)
                    .SetMinWidth(Game.NormalSkin.NormalFont.MeasureString(Title).X * MaxFontScale);

            _table.Row().SetPadTop(20);

            _table.Add(new Label(Language.Translate("slogan"), Game.NormalSkin).SetFontScale(4f));

            _table.Row().SetPadTop(60);

            TextButton playButton = _table.Add(new TextButton(Language.Translate("play"), Game.NormalSkin)).GetElement<TextButton>();
            playButton.GetLabel().SetFontScale(3f);
            _table.Row().SetPadTop(25);
            TextButton settingsButton = _table.Add(new TextButton(Language.Translate("settings"), Game.NormalSkin)).GetElement<TextButton>();
            settingsButton.GetLabel().SetFontScale(3f);
            _table.Row().SetPadTop(25);
            TextButton exitButton = _table.Add(new TextButton(Language.Translate("exit"), Game.NormalSkin)).GetElement<TextButton>();
            exitButton.GetLabel().SetFontScale(3f);
            _table.Row().Expand();


            Table lowerTable = new Table();
            _table.Add(lowerTable).Bottom().Pad(24f).Expand().Fill().GetMaxWidth();
            TextButton creditsButton = new TextButton(Language.Translate("credits"), Game.NormalSkin);
            creditsButton.GetLabel().SetFontScale(3f);
            Container creditsButtonContainer = lowerTable.Add(new Container(creditsButton)).Expand().Left().Bottom().GetElement<Container>();
            Label versionLabel = new Label(Program.Version.ToString(), Game.NormalSkin);
            versionLabel.SetFontScale(2f);
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