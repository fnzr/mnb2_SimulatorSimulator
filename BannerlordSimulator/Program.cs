using System.Collections.Generic;
using HarmonyLib;
using MongoDB.Bson.Serialization;

namespace BannerlordSimulator
{
    internal class Program
    {
        
        public static void Main(string[] args)
        {

            BsonClassMap.RegisterClassMap<Statistics>(Statistics.GetClassMap);
            //Mongo.Database.CreateCollection("statistics");
            var col = Mongo.Database.GetCollection<Statistics>("statistics");
            
            var harmony = new Harmony("fnzr");
            harmony.PatchAll();
            for (int i = 0; i < 500; i++)
            {
                var army = Template.GenerateArmy(Size.Micro, Tier.High);
                var army2 = Template.CloneArmy(army);
                RunSimulation(army, army2);
            }
            
            col.InsertMany(Statistics.AllStatistics.Values);
        }

        public static void RunSimulation(List<Troop> army1, List<Troop> army2)
        {
            var mapEvent = new MapEvent(army1, army2);
            var simulation = new Simulation(mapEvent, true);
            while (simulation.BattleState == BattleState.None)
            {
                simulation.SimulateBattle();
            }
        }
        /*

        private static double _uniqueTroopId = 0;

        private static double UniqueTroopId => ++_uniqueTroopId;

        private static IEnumerable<Troop> GenerateTroops(int min, int max, int size)
        {
            var dist = new DiscreteUniform(min, max);
            var samples = new int[size];
            dist.Samples(samples);
            var result = new List<Troop>();
            for (var i = 0; i < size; i++)
            {
                result.Add(new Troop(new UniqueTroopDescriptor(UniqueTroopId), new CharacterObject(samples[i]), true));
            }
            return result;
        }
        */
    }
}