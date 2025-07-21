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
            Func<Altar, double> scoreFunc = (altar) =>
            {
                double lpPerCycle = Math.Min(altar.LPPerCycle, altar.Capacity);
                double bloodPerTick = lpPerCycle / Altar.CycleTime;
                double orbChargeSpeed = Math.Min(bloodPerTick, altar.OrbChargeSpeed);

                if (preventDryAltar && bloodPerTick < altar.OrbChargeSpeed)
                    return double.MinValue;

                return orbChargeSpeed;
            };

            return OptimiseAltar(baseAltar, orb, scoreFunc, networkCapacity);
        }

        private Altar OptimiseAltar(Altar baseAltar, IBloodOrb orb, Func<Altar, double> scoreFunc, int networkCapacity = 0)
        {
            var working = baseAltar.Copy();
            working.AddOrb(orb);

            // If requested blood network capacity is larger tha the orb,
            // add Orb Runes until the target it met.
            if (networkCapacity > orb.Capacity)
            {
                ApplyNetworkCapacityRunes(working, orb, networkCapacity);
            }

            var maxSlots = working.MaxRuneSlots - working.TotalRunesUsed;
            var runeTypes = m_runes.Count;

            if (maxSlots == 0)
                return working;

            var bestScore = double.MinValue;
            var bestAltar = working.Copy();

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

                // Test/Score the altar setup
                var score = scoreFunc(testAltar);

                if (score > bestScore)
                {
                    bestScore = score;
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

