using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace BobGreenhands.Utils
{
    /// <summary>
    /// An effort to create an InputProcessor like the one in libGDX. The actual handling happens in Update() in Game.
    /// </summary>
    public interface IInputProcessor
    {
        // Keyboard
        void KeyPressed(Keys key);
        void KeyReleased(Keys key);

        // Mouse
        void LeftMousePressed();
        void LeftMouseReleased();
        void RightMousePressed();
        void RightMouseReleased();
        void MiddleMousePressed();
        void MiddleMouseReleased();
        void FirstExtendedMousePressed();
        void FirstExtendedMouseReleased();
        void SecondExtendedMousePressed();
        void SecondExtendedMouseReleased();
        void MouseScrolled(int delta);
        void MouseMoved(Point delta);
        void ScaledMouseMoved(Vector2 delta);

        // TODO: Gamepad
    }
}