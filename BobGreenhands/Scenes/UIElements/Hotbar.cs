using BobGreenhands.Utils;
using Microsoft.Xna.Framework;


namespace BobGreenhands.Scenes.UIElements
{
    public class Hotbar : Inventory
    {
        public Hotbar() : base(FNATextureHelper.Load("img/ui/normal/hotbar", Game.Content), 10, 1, PlayScene.CurrentSavegame.SavegameData.Hotbar.ToArray())
        {
        }
    }
}