using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BobGreenhands.Utils.CultureUtils;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;


namespace BobGreenhands.Scenes
{
    public class SettingsMenu : MovingBackgroundScene
    {
        private Table _table;

        private TabPane _tabPane;

        private Tab _audioSettings;
        private Tab _videoSettings;
        private Tab _languageSettings;
        private Tab _miscSettings;

        // this dictionary saves the differences to the original settings so they can be applied one by one when the user says so
        private Dictionary<string, object> _diff = new Dictionary<string, object>();

        private Dictionary<string, CultureInfo> _languages = new Dictionary<string, CultureInfo>()
        {
            {Language.Translate("langName", "de-DE", null), new CultureInfo("de-DE")},
            {Language.Translate("langName", "en-US", null), new CultureInfo("en-US")},
            {Language.Translate("langName", "es-ES", null), new CultureInfo("es-ES")},
            {Language.Translate("langName", "fr-FR", null), new CultureInfo("fr-FR")},
            {Language.Translate("langName", "nl-NL", null), new CultureInfo("nl-NL")},
        };

        private CultureInfo _oldCultureInfo = Language.CultureInfo;

        private TextField _resX = new TextField("", Game.NormalSkin);
        private TextField _resY = new TextField("", Game.NormalSkin);

        private TextButton _vsyncButton;
        private TextButton _fullscreenButton;
        private TextButton _vignetteButton;

        private TextButton _rpcButton;
        private TextButton _backButton;
        private TextButton _applyButton;

        private ListBox<string> _listBox = new ListBox<string>(new ListBoxStyle(Game.NormalSkin.NormalFont, new Color(255, 255, 255), new Color(128, 128, 128), new PrimitiveDrawable(new Color(0, 0, 0, 0))));

        public SettingsMenu()
        {
            _table = UICanvas.Stage.AddElement(new Table());
            _table.SetFillParent(true);
            _table.Pad(25);
            _table.Add(new Label(Language.Translate("settings"), Game.NormalSkin).SetFontScale(6f)).SetFillX().SetExpandX();

            _table.Row();

            TabWindowStyle tabWindowStyle = new TabWindowStyle();
            tabWindowStyle.Background = new PrimitiveDrawable(new Color(0, 0, 0, 160));
            TabButtonStyle tabButtonStyle = new TabButtonStyle();
            tabButtonStyle.Active = new PrimitiveDrawable(new Color(0, 0, 0, 0));
            tabButtonStyle.Hover = new PrimitiveDrawable(new Color(255, 255, 255, 64));
            tabButtonStyle.Inactive = new PrimitiveDrawable(new Color(0, 0, 0, 200));
            tabButtonStyle.LabelStyle = Game.NormalSkin.Get<LabelStyle>("label");
            tabWindowStyle.TabButtonStyle = tabButtonStyle;
            _tabPane = _table.Add(new TabPane(tabWindowStyle)).Expand().Space(15f).Fill().GetElement<TabPane>();

            _table.Row();

            Table buttonTable = _table.Add(new Table()).GetElement<Table>();

            _applyButton = new TextButton(Language.Translate("apply"), Game.NormalSkin);
            buttonTable.Add(_applyButton).Space(15f);
            _applyButton.OnClicked += ApplyButton_onClicked;

            _backButton = new TextButton(Language.Translate("back"), Game.NormalSkin);
            buttonTable.Add(_backButton).Space(15f);
            _backButton.OnClicked += BackButton_onClicked;

            TabStyle tabStyle = new TabStyle();
            tabStyle.Background = new PrimitiveDrawable(new Color(0, 0, 0, 0));
            /// tabs
            _audioSettings = new Tab(String.Format("  {0}  ", Language.Translate("audioSettings")), tabStyle);
            _audioSettings.Pad(25f);
            _audioSettings.Add(new Label("WIP! :)", Game.NormalSkin).SetFontScale(3f));
            _tabPane.AddTab(_audioSettings);

            _videoSettings = new Tab(String.Format("  {0}  ", Language.Translate("videoSettings")), tabStyle);
            _videoSettings.Pad(25f);
            HorizontalGroup resolutionGroup = _videoSettings.Add(new HorizontalGroup(15f)).SetExpandX().Center().Space(15f).GetElement<HorizontalGroup>();
            resolutionGroup.AddElement(new Label(Language.Translate("resolution"), Game.NormalSkin).SetFontScale(2f).SetWrap(true));
            _resX.SetText("" + Screen.Width);
            _resX.SetAlignment(Align.Center);
            _resX.OnTextChanged += resX_onChanged;
            _resX.OnEnterPressed += resX_onEnter;
            _resX.OnTabPressed += resX_onEnter;
            resolutionGroup.AddElement(_resX);
            resolutionGroup.AddElement(new Label("x", Game.NormalSkin).SetFontScale(3f));
            _resY.SetText("" + Screen.Height);
            _resY.SetAlignment(Align.Center);
            _resY.OnTextChanged += resY_onChanged;
            _resY.OnEnterPressed += resY_onEnter;
            _resY.OnTabPressed += resY_onEnter;
            resolutionGroup.AddElement(_resY);
            _videoSettings.Row();
            HorizontalGroup videoSettings1 = _videoSettings.Add(new HorizontalGroup(15f)).SetExpandX().Center().Space(15f).GetElement<HorizontalGroup>();
            videoSettings1.SetAlignment(Align.Center);
            _vsyncButton = videoSettings1.AddElement(new TextButton(Language.Translate("vsync"), Game.NormalSkin));
            _vsyncButton.GetLabel().SetFontScale(3f);
            _vsyncButton.IsChecked = Screen.SynchronizeWithVerticalRetrace;
            _vsyncButton.OnClicked += VsyncButton_onClicked;
            _fullscreenButton = videoSettings1.AddElement(new TextButton(Language.Translate("fullscreen"), Game.NormalSkin));
            _fullscreenButton.GetLabel().SetFontScale(3f);
            _fullscreenButton.IsChecked = Screen.IsFullscreen;
            _fullscreenButton.OnClicked += FullscreenButton_onClicked;
            _vignetteButton = videoSettings1.AddElement(new TextButton(Language.Translate("vignette"), Game.NormalSkin));
            _vignetteButton.GetLabel().SetFontScale(3f);
            _vignetteButton.IsChecked = Game.GameFolder.Settings.GetBool("vignette");
            _vignetteButton.OnClicked += VignetteButton_onClicked;
            _videoSettings.Row();
            _videoSettings.Add(new Label(Language.Translate("restartInfo"), Game.NormalSkin).SetFontScale(2f).SetWrap(true).SetAlignment(Align.BottomLeft)).Expand().Fill().Bottom();
            _tabPane.AddTab(_videoSettings);
            
            _languageSettings = new Tab(String.Format("  {0}  ", Language.Translate("language")), tabStyle);
            _languageSettings.Pad(25f);
            ScrollPane scrollPane = new ScrollPane(_listBox);
            scrollPane.SetScrollSpeed(1f);
            scrollPane.SetSmoothScrolling(true);
            _languageSettings.Add(scrollPane).Expand().Fill().Space(25f);
            // init the string list of all the languages available
            List<string> output = new List<string>();
            foreach(string s in _languages.Keys)
            {
                output.Add(s);
            }
            _listBox.SetItems(output.ToArray());
            _listBox.SetSelected(Language.Translate("langName", Language.CultureInfo.ToString(), null));
            _tabPane.AddTab(_languageSettings);
        }

        private void resX_onChanged(TextField obj, string text)
        {
            _resX.SetTextForced(Regex.Replace(text, @"\D*", String.Empty));
        }

        private void resY_onChanged(TextField obj, string text)
        {
            _resY.SetTextForced(Regex.Replace(text, @"\D*", String.Empty));
        }

        private void resX_onEnter(TextField obj)
        {
            int res = 0;
            try
            {
                res = Convert.ToInt32(obj.GetText());
            }
            catch(FormatException e)
            {
                res = Screen.Width;
            }
            if(res < 1280)
                res = 1280;
            _resX.SetTextForced(Convert.ToString(res));
        }

        private void resY_onEnter(TextField obj)
        {
            int res = 0;
            try
            {
                res = Convert.ToInt32(obj.GetText());
            }
            catch(FormatException e)
            {
                res = Screen.Height;
            }
            if(res < 720)
                res = 720;
            _resY.SetTextForced(Convert.ToString(res));
        }

        private void BackButton_onClicked(Button obj)
        {
            obj.Toggle();
            Core.StartSceneTransition(new WindTransition(() => new MainMenu()));
            Language.Initialize(_oldCultureInfo);
        }

        private void ApplyButton_onClicked(Button obj)
        {
            resX_onEnter(_resX);
            resY_onEnter(_resY);
            _diff["width"] = Convert.ToInt32(_resX.GetText());
            _diff["height"] = Convert.ToInt32(_resY.GetText());
            obj.Toggle();
            foreach (string key in _diff.Keys.ToArray())
            {
                Game.GameFolder.Settings.Set(key, _diff[key]);
            }
            Language.Initialize(_languages[_listBox.GetSelected()]);
            Game.GameFolder.Settings.Set("language", _languages[_listBox.GetSelected()].ToString());
            Game.GameFolder.Settings.Save();
            _oldCultureInfo = _languages[_listBox.GetSelected()];
        }

        private void VsyncButton_onClicked(Button obj)
        {
            Screen.SynchronizeWithVerticalRetrace = !Screen.SynchronizeWithVerticalRetrace;
            _diff["vsync"] = Screen.SynchronizeWithVerticalRetrace;
            obj.IsChecked = Screen.SynchronizeWithVerticalRetrace;
        }

        private void FullscreenButton_onClicked(Button obj)
        {
            Screen.IsFullscreen = !Screen.IsFullscreen;
            _diff["fullscreen"] = Screen.IsFullscreen;
            obj.IsChecked = Screen.IsFullscreen;
        }

        private void VignetteButton_onClicked(Button obj)
        {
            _diff["vignette"] = _vignetteButton.IsChecked;
            obj.IsChecked = (bool)_diff["vignette"];
        }
    }
}