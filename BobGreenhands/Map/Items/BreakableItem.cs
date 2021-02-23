using System;
using Nez.Persistence;


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
                return ((float) Durability/MaxDurability);
            }
            catch(DivideByZeroException e)
            {
                return 0;
            }
        }
    }
}