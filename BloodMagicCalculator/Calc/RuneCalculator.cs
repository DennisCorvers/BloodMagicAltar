using BloodMagicCalculator.Runes;

namespace BloodMagicCalculator.Calc
{
    internal class RuneCalculator
    {
        private readonly IReadOnlyList<BaseRune> m_runes;

        public RuneCalculator(IReadOnlyList<BaseRune> targetRunes)
        {
            m_runes = targetRunes;
        }

        public Altar OptimiseOrbChargeRate(Altar baseAltar, IBloodOrb orb)
        {
            int maxRunes = baseAltar.MaxRuneSlots - baseAltar.TotalRunesUsed;
            int runeCount = m_runes.Count;

            // Initialize all slots with the best single rune (based on estimate)
            int bestRuneIndex = 0;
            double bestSingleEfficiency = -1;

            for (int i = 0; i < runeCount; i++)
            {
                double eff = EstimateRuneEfficiency(m_runes[i], baseAltar, orb);
                if (eff > bestSingleEfficiency)
                {
                    bestSingleEfficiency = eff;
                    bestRuneIndex = i;
                }
            }

            // Current counts - start with all slots assigned to bestRuneIndex
            int[] currentCounts = new int[runeCount];
            currentCounts[bestRuneIndex] = maxRunes;

            double bestRate = EvaluateRuneSetup(baseAltar, orb, currentCounts, out Altar bestAltar);
            bool improved = true;
            int maxIterations = 10000; // limit for safety

            while (improved && maxIterations-- > 0)
            {
                improved = false;

                // Try swapping 1 rune from rune i to rune j for all pairs (i != j)
                for (int i = 0; i < runeCount; i++)
                {
                    if (currentCounts[i] == 0) continue;

                    for (int j = 0; j < runeCount; j++)
                    {
                        if (i == j) continue;

                        // Try swapping one rune from i to j
                        currentCounts[i]--;
                        currentCounts[j]++;

                        double newRate = EvaluateRuneSetup(baseAltar, orb, currentCounts, out Altar candidateAltar);

                        if (newRate > bestRate)
                        {
                            bestRate = newRate;
                            bestAltar = candidateAltar.Copy();
                            improved = true;
                            // Accept swap and break inner loops to restart search from improved state
                            break;
                        }
                        else
                        {
                            // Revert swap
                            currentCounts[i]++;
                            currentCounts[j]--;
                        }
                    }

                    if (improved) break;
                }
            }

            return bestAltar;
        }

        private double EvaluateRuneSetup(Altar baseAltar, IBloodOrb orb, int[] counts, out Altar resultAltar)
        {
            var altar = baseAltar.Copy();
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] > 0)
                    altar.AddRune(m_runes[i], counts[i]);
            }
            altar.AddOrb(orb);

            double bloodThisCycle = Math.Min(altar.LPPerCycle, altar.Capacity);
            double bloodPerTick = bloodThisCycle / Altar.CycleTime;
            double orbRate = Math.Min(bloodPerTick, altar.OrbChargeSpeed);

            resultAltar = altar.Copy();
            return orbRate;
        }

        // Same estimate function as before (or improved for speed)
        private double EstimateRuneEfficiency(BaseRune rune, Altar altar, IBloodOrb orb)
        {
            var testAltar = altar.Copy();
            testAltar.AddRune(rune, 1);
            testAltar.AddOrb(orb);

            double bloodThisCycle = Math.Min(testAltar.LPPerCycle, testAltar.Capacity);
            double bloodPerTick = bloodThisCycle / Altar.CycleTime;
            double rate = Math.Min(bloodPerTick, testAltar.OrbChargeSpeed);

            double baseBloodThisCycle = Math.Min(altar.LPPerCycle, altar.Capacity);
            double baseBloodPerTick = baseBloodThisCycle / Altar.CycleTime;
            double baseRate = Math.Min(baseBloodPerTick, altar.OrbChargeSpeed);

            return Math.Max(0, rate - baseRate);
        }
    }
}

