using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;

namespace BannerlordSimulator
{
    public class MBMath
    {
        public static int ClampInt(int value, int min, int max)
        {
            return Math.Min(max, Math.Max(value, min));
        }
    }

    public class MBRandom
    {
        public static readonly Random Random = new Random();
        public static float RandomFloat => (float)Random.NextDouble();

        public static int RandomInt(int max) => Random.Next(max);
    }
    
    public class MapEvent
    {
        private BattleState _battleState = BattleState.None;
        public MapEventSide AttackerSide;
        public MapEventSide DefenderSide;

        public BattleState BattleState
        {
            get => _battleState;
            set => _battleState = value;
        }

        public MapEvent(List<Troop> attackers, List<Troop> defenders)
        {
            AttackerSide = new MapEventSide(attackers);
            DefenderSide = new MapEventSide(defenders);
        }
    }

    public enum BattleState
    {
        None,
        AttackerVictory,
        DefenderVictory
    }

    public class MapEventSide
    {
        public List<Troop> Troops;
        private List<UniqueTroopDescriptor> _simulationTroopList;
        
        public MapEventSide(List<Troop> troops)
        {
            Troops = troops;
            _simulationTroopList = Troops.Select(t => t.Descriptor).ToList();
        }

        public int NumRemainingSimulationTroops
        {
            get => Troops.Count(t => !t.IsDefeated);
        }
        
        public CharacterObject GetAllocatedTroop(UniqueTroopDescriptor troopDesc0)
        {
            var troop = Troops.FirstOrDefault(t => troopDesc0.CompareTo(t.Descriptor) == 0);
            return troop?.Character;
        }

        public void RemoveShimTroop(UniqueTroopDescriptor descriptor)
        {
            _simulationTroopList.Remove(descriptor);
            Troops.RemoveAll(t => t.Descriptor.Equals(descriptor));
        }
    }

    public class CharacterObject
    {
        private int _tier;
        
        public int Tier => _tier;

        public CharacterObject(int tier)
        {
            _tier = tier;
        }
    }
    
    public struct UniqueTroopDescriptor : IComparable<UniqueTroopDescriptor>, IEquatable<UniqueTroopDescriptor>
    {
        readonly double value;
        
        public UniqueTroopDescriptor(double value)
        {
            this = default(UniqueTroopDescriptor);
            this.value = value;
        }
        public int CompareTo(UniqueTroopDescriptor other)
        {
            return this.value.CompareTo(other.value);
        }

        public bool Equals(UniqueTroopDescriptor other)
        {
            return other.value == this.value;
        }
    }
}