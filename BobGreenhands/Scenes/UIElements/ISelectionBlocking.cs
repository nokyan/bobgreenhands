namespace BobGreenhands.Scenes.UIElements
{
    /// <summary>
    /// Interface to make it easier to determine if a Element is selection blocking.
    /// </summary>
    public interface ISelectionBlocking
    {
        bool HoveredOver();
    }
}