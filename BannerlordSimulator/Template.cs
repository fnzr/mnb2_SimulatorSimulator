using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace BannerlordSimulator
{
    public enum Size
    {
        Micro,
        Small,
        Medium,
        Big,
        Giant
    }

    public enum Tier
    {
        Low,
        Medium,
        High,
        Elite,
        Spread
    }
    
    public class Template
    {
        private static readonly Dictionary<Size, int[]> SizeTemplate;
        private static readonly Dictionary<Tier, int[]> TierTemplate;
        private static readonly Random Random = new Random();
        private static double _uniqueTroopId = 0;

        private static double UniqueTroopId => ++_uniqueTroopId;
        
        static Template()
        {
            SizeTemplate = new Dictionary<Size, int[]>()
            {
                {Size.Micro, new []{1, 30}},
                {Size.Small, new []{50, 100}},
                {Size.Medium, new []{150, 300}},
                {Size.Big, new []{500, 1000}},
                {Size.Giant, new []{1500, 2000}},
            };

            TierTemplate = new Dictionary<Tier, int[]>()
            {
                {Tier.Low, new []{0, 3}},
                {Tier.Medium, new []{2, 5}},
                {Tier.High, new []{4, 7}},
                {Tier.Elite, new []{6, 7}},
                {Tier.Spread, new []{0, 7}},
            };
        }

        public static List<Troop> GenerateArmy(Size size, Tier tier)
        {
            var tierArr = TierTemplate[tier];
            var sizeArr = SizeTemplate[size];
            var armySize = Random.Next(sizeArr[0], sizeArr[1]);
            return GenerateTroops(tierArr[0], tierArr[1], armySize);
        }
        
        public static List<Troop> CloneArmy(List<Troop> original)
        {
            return original.Select(t =>
                new Troop(new UniqueTroopDescriptor(UniqueTroopId), new CharacterObject(t.Tier), true)).ToList();
        }
        
        private static List<Troop> GenerateTroops(int minTier, int maxTier, int size)
        {
            var dist = new DiscreteUniform(minTier, maxTier);
            var samples = new int[size];
            dist.Samples(samples);
            var result = new List<Troop>();
            for (var i = 0; i < size; i++)
            {
                result.Add(new Troop(new UniqueTroopDescriptor(UniqueTroopId), new CharacterObject(samples[i]), true));
            }
            return result;
        }
    }
}