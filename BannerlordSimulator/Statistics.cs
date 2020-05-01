using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;

namespace BannerlordSimulator
{
    public class Statistics
    {
        public static readonly Dictionary<Guid, Statistics> AllStatistics = new Dictionary<Guid, Statistics>();

        public static Statistics Get(Simulation simulation)
        {
            if (!AllStatistics.ContainsKey(simulation.Guid))
            {
                AllStatistics[simulation.Guid] = new Statistics(simulation);
            }

            return AllStatistics[simulation.Guid];
        }

        public Statistics(Simulation simulation)
        {
            _simulation = simulation;
            SimulationGUID = _simulation.Guid;
        }

        private Simulation _simulation;
        
        public Guid SimulationGUID { get; set; }
        public int Rounds { get; set; }
        public int AttackerCasualties { get; set; }
        public int DefenderCasualties { get; set; }
        public int Duels { get; set; }
        public int DuelTies => Duels - (AttackerCasualties + DefenderCasualties);
        public bool AttackerVictory => _simulation.BattleState == BattleState.AttackerVictory;
        public double AttackerTroopsUsed { get; set; }
        public double DefenderTroopsUsed { get; set; }
        public double TacticalAdvantage { get; set; }
        public double AttackerTroopTier { get; set; }
        public double DefenderTroopTier { get; set; }
        
        public double AverageAttackerTroopLevel => Math.Round(AttackerTroopTier / AttackerTroopsUsed, 2);
        public double AverageDefenderTroopLevel => Math.Round(DefenderTroopTier / DefenderTroopsUsed, 2);

        public double AverageLevelDifference => Math.Round(AverageAttackerTroopLevel - AverageDefenderTroopLevel, 3);

        public static void GetClassMap(BsonClassMap cm)
        {
            cm.MapProperty(nameof(Rounds));
            cm.MapProperty(nameof(AttackerCasualties));
            cm.MapProperty(nameof(DefenderCasualties));
            cm.MapProperty(nameof(DuelTies));
            cm.MapProperty(nameof(Duels));
            cm.MapProperty(nameof(AttackerVictory));
            cm.MapProperty(nameof(AverageLevelDifference));
            cm.MapProperty(nameof(TacticalAdvantage));
        }

    }
}