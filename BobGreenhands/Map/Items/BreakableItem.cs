using System;
using Nez.Persistence;


namespace BobGreenhands.Map.Items
{
    public abstract class BreakableItem : Item
    {
        [JsonExclude]
        public static readonly int MaxDurability;

        [JsonInclude]
        public int Durability
        {
            get;
            protected set;
        }

        public BreakableItem()
        {
            
        }

        public override string? GetInfoString()
        {
            return String.Format("{0}", Durability);
        }
    }
}