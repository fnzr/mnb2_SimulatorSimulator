using System;
using System.Collections.Generic;
using System.Reflection;

namespace BannerlordSimulator
{
    public class Troop
    {
        public readonly UniqueTroopDescriptor Descriptor;
        public readonly CharacterObject Character;
        
        public readonly bool IsAttacker;
        public int Tier => Character.Tier;
        public bool IsDefeated = false;

        public Troop(UniqueTroopDescriptor descriptor, CharacterObject character, bool isAttacker)
        {
            Descriptor = descriptor;
            Character = character;
            IsAttacker = isAttacker;
        }

        public double Power
        {
            get => Config.TroopTierFactor * (Config.TroopPower[Tier] + Config.TroopNormal[Tier].Sample());
        }
        
        private static readonly FieldInfo SimulationTroopListFieldInfo =
            typeof(MapEventSide).GetField("_simulationTroopList",
                BindingFlags.Instance | BindingFlags.NonPublic);

        public static List<UniqueTroopDescriptor> GetTroopList(MapEventSide mapEventSide)
        {
            return (List<UniqueTroopDescriptor>)SimulationTroopListFieldInfo.GetValue(mapEventSide);
        }
    }
}