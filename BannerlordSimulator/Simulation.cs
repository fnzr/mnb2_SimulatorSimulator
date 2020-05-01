using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BannerlordSimulator
{
    public class Simulation
    {
        private static readonly PropertyInfo BattleStateField =
            typeof(MapEvent).GetProperty("BattleState", BindingFlags.Instance | BindingFlags.Public);
        public readonly Guid Guid;
        private List<Troop> Attackers;
        private List<Troop> Defenders;
        private readonly MapEvent _mapEvent;
        private int RemainingAttackers;
        private int RemainingDefenders;

        private readonly bool IsDryRun;
        //private double TacticalAdvantage;
        public BattleState BattleState => _mapEvent.BattleState;

        public Simulation(MapEvent mapEvent, bool isDryRun)
        {
            _mapEvent = mapEvent;
            IsDryRun = isDryRun;
            Guid = Guid.NewGuid();
        }

        private bool Duel(Troop attacker, Troop defender, double attackerAdvantage)
        {
            var attackerPower = attacker.Power + attackerAdvantage;
            return attackerPower - defender.Power > 0.1;
        }
        
        private (int attackerSize, int defenderSize) CalculateTroopsPerSide()
        {
            var attackerSize = Config.BaseRoundTroopCount;
            var defenderSize = Config.BaseRoundTroopCount;

            if (_mapEvent.AttackerSide.NumRemainingSimulationTroops > _mapEvent.DefenderSide.NumRemainingSimulationTroops)
            {
                var ratio = 1 -(double)_mapEvent.AttackerSide.NumRemainingSimulationTroops /
                            _mapEvent.DefenderSide.NumRemainingSimulationTroops;
                attackerSize += Config.GetTroopsForNumericAdvantage(ratio);
            }
            else
            {
                var ratio = 1 - (double) _mapEvent.DefenderSide.NumRemainingSimulationTroops /
                            _mapEvent.AttackerSide.NumRemainingSimulationTroops;
                defenderSize += Config.GetTroopsForNumericAdvantage(ratio);
            }

            attackerSize = MBMath.ClampInt(attackerSize, 1, _mapEvent.AttackerSide.NumRemainingSimulationTroops);
            defenderSize = MBMath.ClampInt(defenderSize, 1, _mapEvent.DefenderSide.NumRemainingSimulationTroops);
            return (attackerSize, defenderSize);

        }

        private List<Troop> PrepareTroopsForSide(MapEventSide mapEventSide, int amount, bool isAttacker)
        {
            var troopList = Troop.GetTroopList(mapEventSide);
            var counter = 0;
            var selectedTroops = new List<Troop>();
            foreach(var descriptor in troopList)
            {
                var chance = amount / (troopList.Count - counter);
                if (chance > MBRandom.RandomFloat)
                {
                    amount--;
                    var character = mapEventSide.GetAllocatedTroop(descriptor);
                    selectedTroops.Add(new Troop(descriptor, character, isAttacker));
                }
                if (amount == 0) break;
                counter++;
            }

            return selectedTroops;
        }
        
        private void PrepareParticipatingTroops()
        {
            (int attackerCount, int defenderCounter) = CalculateTroopsPerSide();
            Attackers = PrepareTroopsForSide(_mapEvent.AttackerSide, attackerCount, true);
            Defenders = PrepareTroopsForSide(_mapEvent.DefenderSide, defenderCounter, false);
        }

        private bool OnTroopDefeated(Troop defeatedTroop)
        {
            defeatedTroop.IsDefeated = true;
            RemoveShimTroop(defeatedTroop);
            //dryrun only
            return true;
        }

        private void RemoveShimTroop(Troop troop)
        {
            if (IsDryRun)
            {
                var side = troop.IsAttacker ? _mapEvent.AttackerSide : _mapEvent.DefenderSide;
                side.RemoveShimTroop(troop.Descriptor);
            }
        }

        private bool PerformAttack(Troop attacker)
        {
            if (attacker.IsDefeated) return false;
            
            List<Troop> targets;
            MapEventSide attackerSide;
            MapEventSide targetSide;
            ref int remainingTroops = ref attacker.IsAttacker ? ref RemainingDefenders : ref RemainingAttackers;
            if (attacker.IsAttacker)
            {
                targets = (List<Troop>)Defenders;
                attackerSide = _mapEvent.AttackerSide;
                targetSide = _mapEvent.DefenderSide;
            }
            else
            {
                targets = (List<Troop>) Attackers;
                attackerSide = _mapEvent.DefenderSide;
                targetSide = _mapEvent.AttackerSide;
            }

            var target = targets.FirstOrDefault(t => !t.IsDefeated);
            if (target == null) return false;

            if (Duel(attacker, target, 0))
            {
                OnTroopDefeated(target);
                remainingTroops--;
                //reward and death
                return true;
            }
            return false;
        }

        private bool IsWinnerDecided()
        {
            if (RemainingAttackers == 0)
            {
                BattleStateField.SetValue(_mapEvent, BattleState.DefenderVictory);
                return true;
            }

            if (RemainingDefenders == 0)
            {
                BattleStateField.SetValue(_mapEvent, BattleState.AttackerVictory);
                return true;
            }

            return false;
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SimulateBattle()
        {
            RemainingAttackers = _mapEvent.AttackerSide.NumRemainingSimulationTroops;
            RemainingDefenders = _mapEvent.DefenderSide.NumRemainingSimulationTroops;
            if (IsWinnerDecided())
            {
                return;
            }
            PrepareParticipatingTroops();

            var troops = Attackers.Concat(Defenders).ToList();
            troops.Shuffle();

            foreach (var troop in troops)
            {
                if (PerformAttack(troop) && IsWinnerDecided())
                {
                    break;
                }
            }
        }
    }
}