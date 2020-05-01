using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace BannerlordSimulator
{
    
    [HarmonyPatch(typeof(Simulation), "OnTroopDefeated")]
    public class OnTroopDefeatedPatch
    {
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Postfix(ref Simulation __instance, ref Troop defeatedTroop)
        {
            var statistics = Statistics.Get(__instance);
            if (defeatedTroop.IsAttacker)
            {
                statistics.AttackerCasualties++;
            }
            else
            {
                statistics.DefenderCasualties++;
            }
        }
    }
    
    [HarmonyPatch(typeof(Simulation), "SimulateBattle")]
    public class SimulateBattlePatch
    {
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Postfix(ref Simulation __instance)
        {
            var statistics = Statistics.Get(__instance);
            statistics.Rounds++;
        }
    }
    
    [HarmonyPatch(typeof(Simulation), "Duel")]
    public class DuelPatch
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Postfix(ref Simulation __instance)
        {
            var statistics = Statistics.Get(__instance);
            statistics.Duels++;
        }
    }

    [HarmonyPatch(typeof(Simulation), "PrepareParticipatingTroops")]
    public class CalculateTroopsPerSidePatch
    {
        private static readonly FieldInfo AttackersField =
            typeof(Simulation).GetField("Attackers", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo DefendersField =
            typeof(Simulation).GetField("Defenders", BindingFlags.Instance | BindingFlags.NonPublic);
        
        public static void Postfix(ref Simulation __instance)
        {
            var attackers = (List<Troop>) AttackersField.GetValue(__instance);
            var defenders = (List<Troop>) DefendersField.GetValue(__instance);

            var statistics = Statistics.Get(__instance);
            statistics.AttackerTroopsUsed += attackers.Count;
            statistics.DefenderTroopsUsed += defenders.Count;

            statistics.AttackerTroopTier = attackers.Sum(t => t.Tier);
            statistics.DefenderTroopTier = defenders.Sum(t => t.Tier);

        }
    }
}