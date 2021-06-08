using Microsoft.VisualBasic.CompilerServices;
using System;
using Nez.Persistence;
using BobGreenhands.Utils;


namespace BobGreenhands.Map.Items
{
    public abstract class BreakableItem : Item
    {
        [JsonExclude]
        protected int MaxDurability;

        [JsonInclude]
        protected int Durability;

        public BreakableItem()
        {
            
        }

        public override string? GetInfoString()
        {
            return "";
        }

        public override float GetInfoFloat()
        {
            try
            {
                return Math.Clamp(((float) Durability/MaxDurability), 0, 1);
            }
            catch(DivideByZeroException e)
            {
                return 0;
            }
        }
    }
}