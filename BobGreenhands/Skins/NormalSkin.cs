using System;
using Nez.BitmapFonts;
using Nez.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace BobGreenhands.Skins
{
    /// <summary>
    /// Just the normal skin that we want to apply to pretty much everything
    /// </summary>
    public class NormalSkin : Skin
    {
        public BitmapFont NormalFont
        {
            get;
            private set;
        }

        public readonly Color BackgroundColor = Color.Green;

        public readonly float CheckboxScale = 1f;
        public readonly float TextButtonScale = 1f;

        public const float OuterSpacing = 8f;

        public const float Spacing = OuterSpacing / 2;

        public const float SubSpacing = Spacing / 2;

        public const float HeadlineFontScale = 2f;

        public const float NormalFontScale = 1f;


        public NormalSkin()
        {
            NormalFont = BitmapFontLoader.LoadFontFromFile("Content/fonts/pearsoda/pearsoda.fnt");

            // Label
            LabelStyle labelStyle = new LabelStyle(NormalFont, Color.White);
            Add("label", labelStyle);

            // Debug Label
            LabelStyle debugStyle = new LabelStyle(NormalFont, Color.Yellow);
            Add("debugLabel", debugStyle);

            // Checkbox
            SpriteDrawable checkboxOff = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/checkbox"));
            ApplyScaling(ref checkboxOff, CheckboxScale);
            SpriteDrawable checkboxOn = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/checkbox_marked"));
            ApplyScaling(ref checkboxOn, CheckboxScale);
            SpriteDrawable checkboxOffDisabled = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/checkbox_inactive"));
            ApplyScaling(ref checkboxOffDisabled, CheckboxScale);
            SpriteDrawable checkboxOnDisabled = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/checkbox_marked_inactive"));
            ApplyScaling(ref checkboxOnDisabled, CheckboxScale);
            SpriteDrawable checkboxOver = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/checkbox_hover"));
            ApplyScaling(ref checkboxOver, CheckboxScale);
            CheckBoxStyle checkBoxStyle = new CheckBoxStyle(checkboxOff, checkboxOn, NormalFont, Color.White);
            checkBoxStyle.CheckboxOffDisabled = checkboxOffDisabled;
            checkBoxStyle.CheckboxOnDisabled = checkboxOnDisabled;
            checkBoxStyle.CheckboxOver = checkboxOver;
            Add("checkbox", checkBoxStyle);

            // TextButton
            SpriteDrawable textButtonUp = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/button"));
            ApplyScaling(ref textButtonUp, TextButtonScale);
            SpriteDrawable textButtonDown = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/button_checked"));
            ApplyScaling(ref textButtonDown, TextButtonScale);
            SpriteDrawable textButtonDisabled = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/button_inactive"));
            ApplyScaling(ref textButtonDisabled, TextButtonScale);
            SpriteDrawable textButtonOver = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/button_over"));
            ApplyScaling(ref textButtonOver, TextButtonScale);
            SpriteDrawable textButtonChecked = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/button_checked"));
            ApplyScaling(ref textButtonChecked, TextButtonScale);
            SpriteDrawable textButtonCheckedOver = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/button_checked_over"));
            ApplyScaling(ref textButtonCheckedOver, TextButtonScale);
            TextButtonStyle textButtonStyle = new TextButtonStyle(textButtonUp, textButtonDown, textButtonOver, NormalFont);
            textButtonStyle.Disabled = textButtonDisabled;
            textButtonStyle.Checked = textButtonChecked;
            textButtonStyle.CheckedOver = textButtonCheckedOver;
            Add("textbutton", textButtonStyle);

            // Slider
            SpriteDrawable sliderBackground = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/button_checked"));
            ApplyScaling(ref sliderBackground, TextButtonScale);
            SpriteDrawable sliderKnob = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/knob"));
            ApplyScaling(ref sliderKnob, TextButtonScale);
            SpriteDrawable sliderDisabledKnob = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/knob_inactive"));
            ApplyScaling(ref sliderDisabledKnob, TextButtonScale);
            SpriteDrawable sliderKnobOver = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/knob_over"));
            ApplyScaling(ref sliderKnobOver, TextButtonScale);
            SpriteDrawable sliderKnobDown = new SpriteDrawable(Game.Content.Load<Texture2D>("img/ui/normal/knob_down"));
            ApplyScaling(ref sliderKnobDown, TextButtonScale);
            SliderStyle sliderStyle = new SliderStyle(sliderBackground, sliderKnob);
            sliderStyle.DisabledBackground = textButtonDisabled;
            sliderStyle.DisabledKnob = sliderDisabledKnob;
            sliderStyle.KnobOver = sliderKnobOver;
            sliderStyle.KnobDown = sliderKnobDown;
            Add("slider", sliderStyle);

            // Window
            PrimitiveDrawable background = new PrimitiveDrawable(new Color(0, 0, 0, 160));
            WindowStyle windowStyle = new WindowStyle(NormalFont, Color.White, background);
            Add("window", windowStyle);

            // TextField
            TextFieldStyle textFieldStyle = new TextFieldStyle(NormalFont, Color.White, new PrimitiveDrawable(Color.White), new PrimitiveDrawable(new Color(255, 255, 255, 100)), new PrimitiveDrawable(new Color(0, 0, 0, 160)));
            Add("textfield", textFieldStyle);

            // TextTooltip
            TextTooltipStyle textTooltipStyle = new TextTooltipStyle(labelStyle, new PrimitiveDrawable(new Color(0, 0, 0, 160)));
            Add("texttooltip", textTooltipStyle);
        }

        private void ApplyScaling(ref SpriteDrawable drawable, float scale)
        {
            drawable.MinWidth = drawable.MinWidth * scale;
            drawable.MinHeight = drawable.MinHeight * scale;
        }

        /// <summary>
        /// Removes characters from input that are not covered by the Normal Font and might cause the game to crash
        /// </summary>
        public string Filter(string input)
        {
            string output = "";
            foreach (char c in input)
            {
                if(NormalFont.ContainsCharacter(c))
                {
                    output += c;
                }
                else
                {
                    output += "?";
                }
            }
            return output;
        }
    }
}