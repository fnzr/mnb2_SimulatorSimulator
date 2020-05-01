using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using CsvHelper;
using HarmonyLib;
using MathNet.Numerics.Distributions;

namespace BannerlordSimulator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var harmony = new Harmony("fnzr");
            harmony.PatchAll();
            //var rounds = 1;
            for (int i = 0; i < 500; i++)
            {
                var army1 = GenerateTroops(1, 7, 500).ToList();
                var army2 = GenerateTroops(1, 7, 500);
                var mapEvent = new MapEvent(army1, CloneArmy(army1));
                var simulation = new Simulation(mapEvent, true);
                while (simulation.BattleState == BattleState.None)
                {
                    simulation.SimulateBattle();
                    //Console.WriteLine($"Round  done");
                    //rounds++;
                }
            }

            using (var writer = new StreamWriter("./out.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<StatisticsMap>();
                csv.WriteRecords(Statistics.AllStatistics.Values.ToList());
            }
        }

        private static double _uniqueTroopId = 0;

        private static double UniqueTroopId => ++_uniqueTroopId;

        private static List<Troop> CloneArmy(List<Troop> original)
        {
            return original.Select(t =>
                new Troop(new UniqueTroopDescriptor(UniqueTroopId), new CharacterObject(t.Tier), true)).ToList();
        }

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

        private static int PointCount = 100;
        private static List<string> PlotStrings = new List<string>();

        public static void PreparePlot(List<double> xs)
        {
            var xstring = string.Join(",", xs);
            PlotStrings.Add(xstring);
        }

        public static void Plot()
        {
            var ys = new List<double>();
            var part = 0.1 / PointCount;
            for (var i = 1; i <= PointCount; i++)
            {
                ys.Add(i*0.1);
            }
            var ystring = string.Join(",", ys);
            var arg = ystring + " " + string.Join(" ", PlotStrings);
            Console.WriteLine(arg);
        }
    }
}