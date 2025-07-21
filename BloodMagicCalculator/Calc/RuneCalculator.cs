using BloodMagicCalculator.Data;
using BloodMagicCalculator.Runes;

namespace BloodMagicCalculator.Calc
{
    internal class RuneCalculator
    {
        private readonly IReadOnlyList<BaseRune> m_runes;

        public RuneCalculator()
        {
            m_runes = new List<BaseRune>()
            {
                new Runes.AugmentedCapacity(),
                new Runes.Sacrifice(),
                new Runes.Speed(),
                new Runes.SuperiorCapacity(),
            };
        }

        public Altar OptimiseOrbChargeRate(Altar baseAltar, IBloodOrb orb, int networkCapacity = 0, bool preventDryAltar = false)
        {
            Altar working = baseAltar.Copy();
            working.AddOrb(orb);
            if (networkCapacity > orb.Capacity)
                ApplyNetworkCapacityRunes(working, orb, networkCapacity);

            int maxSlots = working.MaxRuneSlots - working.TotalRunesUsed;
            int runeTypes = m_runes.Count;

            if (maxSlots == 0)
                return working;

            double bestRate = 0;
            Altar bestAltar = working.Copy();

            var combinations = new List<int[]>();
            GenerateCombinations(maxSlots, 0, runeTypes, new int[runeTypes], combinations);

            foreach (var counts in combinations)
            {
                var testAltar = working.Copy();
                for (int i = 0; i < runeTypes; i++)
                {
                    if (counts[i] > 0)
                        testAltar.AddRune(m_runes[i], counts[i]);
                }

                testAltar.AddOrb(orb);

                double lpPerCycle = Math.Min(testAltar.LPPerCycle, testAltar.Capacity);
                double bloodPerTick = lpPerCycle / Altar.CycleTime;
                double orbRate = Math.Min(bloodPerTick, testAltar.OrbChargeSpeed);

                if (preventDryAltar && bloodPerTick < testAltar.OrbChargeSpeed)
                    continue;

                if (orbRate > bestRate)
                {
                    bestRate = orbRate;
                    bestAltar = testAltar.Copy();
                }
            }

            return bestAltar;
        }

        private void GenerateCombinations(int slotsLeft, int runeIndex, int runeTypes, int[] current, List<int[]> results)
        {
            if (runeIndex == runeTypes - 1)
            {
                current[runeIndex] = slotsLeft;
                results.Add((int[])current.Clone());
                return;
            }

            for (int i = 0; i <= slotsLeft; i++)
            {
                current[runeIndex] = i;
                GenerateCombinations(slotsLeft - i, runeIndex + 1, runeTypes, current, results);
            }
        }

        private static void ApplyNetworkCapacityRunes(Altar baseAltar, IBloodOrb orb, int targetCapacity)
        {
            var networkCapacityRune = new Runes.Orb();
            var testContext = new AltarContext();
            var targetAmount = 0;

            for (int i = 0; i <= baseAltar.MaxRuneSlots - baseAltar.TotalRunesUsed; i++)
            {
                networkCapacityRune.ApplyRune(testContext, 1);
                targetAmount = i + 1;
                if (testContext.SoulNetworkMultiplier * orb.Capacity >= targetCapacity)
                    break;

            }

            baseAltar.AddRune(networkCapacityRune, targetAmount);
        }
    }
}

