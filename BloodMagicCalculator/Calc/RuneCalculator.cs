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

        public Altar OptimiseOrbChargeRate(Altar baseAltar, IBloodOrb orb, int networkCapacity = 0)
        {
            // 1) Pre-apply network-capacity runes (if needed) onto a working altar
            Altar working = baseAltar;
            if (networkCapacity > orb.Capacity)
            {
                working = baseAltar.Copy();
                working.AddOrb(orb);
                ApplyNetworkCapacityRunes(working, orb, networkCapacity);
            }

            // 2) How many free slots remain?
            int maxRunes = working.MaxRuneSlots - working.TotalRunesUsed;
            if (maxRunes <= 0)
            {
                // no room for runes → just return what we have
                if (working.Orb == null)
                    working.AddOrb(orb);
                return working;
            }

            // 3) Spin up a “seed” copy with the orb loaded to measure base stats
            var seed = working.Copy();
            if (seed.Orb == null) seed.AddOrb(orb);

            double baseLpPerCycle = seed.LPPerCycle;
            double baseCapacity = seed.Capacity;
            double baseChargeSpeed = seed.OrbChargeSpeed;
            double cycleTime = Altar.CycleTime;
            int runeCount = m_runes.Count;

            // 4) Compute “delta” for exactly +1 of each rune
            var deltaLp = new double[runeCount];
            var deltaCap = new double[runeCount];
            var deltaCharge = new double[runeCount];

            for (int i = 0; i < runeCount; i++)
            {
                var tmp = seed.Copy();
                tmp.AddRune(m_runes[i], 1);

                deltaLp[i] = tmp.LPPerCycle - baseLpPerCycle;
                deltaCap[i] = tmp.Capacity - baseCapacity;
                deltaCharge[i] = tmp.OrbChargeSpeed - baseChargeSpeed;
            }

            // 5) Find the single-slot best deltas (for optimistic bounding)
            double deltaMaxLp = deltaLp.Max();
            double deltaMaxCap = deltaCap.Max();
            double deltaMaxCharge = deltaCharge.Max();

            // 6) 4 nested loops (for 5 runes) + infer the 5th count
            double bestRate = 0;
            int[] bestCounts = new int[runeCount];

            // ONLY works as written when m_runes.Count == 5:
            for (int n0 = 0; n0 <= maxRunes; n0++)
            {
                // partial stats after n0 of rune 0
                double pLp0 = baseLpPerCycle + deltaLp[0] * n0;
                double pCap0 = baseCapacity + deltaCap[0] * n0;
                double pCharge0 = baseChargeSpeed + deltaCharge[0] * n0;
                int rem0 = maxRunes - n0;

                if (!CanBeat(pLp0, pCap0, pCharge0, rem0, deltaMaxLp, deltaMaxCap, deltaMaxCharge, cycleTime, bestRate))
                    continue;

                for (int n1 = 0; n1 <= rem0; n1++)
                {
                    double pLp1 = pLp0 + deltaLp[1] * n1;
                    double pCap1 = pCap0 + deltaCap[1] * n1;
                    double pCharge1 = pCharge0 + deltaCharge[1] * n1;
                    int rem1 = rem0 - n1;

                    if (!CanBeat(pLp1, pCap1, pCharge1, rem1, deltaMaxLp, deltaMaxCap, deltaMaxCharge, cycleTime, bestRate))
                        continue;

                    for (int n2 = 0; n2 <= rem1; n2++)
                    {
                        double pLp2 = pLp1 + deltaLp[2] * n2;
                        double pCap2 = pCap1 + deltaCap[2] * n2;
                        double pCharge2 = pCharge1 + deltaCharge[2] * n2;
                        int rem2 = rem1 - n2;

                        if (!CanBeat(pLp2, pCap2, pCharge2, rem2, deltaMaxLp, deltaMaxCap, deltaMaxCharge, cycleTime, bestRate))
                            continue;

                        for (int n3 = 0; n3 <= rem2; n3++)
                        {
                            int n4 = rem2 - n3;

                            // exact totals for this 5-tuple
                            double totalLp = pLp2 + deltaLp[3] * n3 + deltaLp[4] * n4;
                            double totalCap = pCap2 + deltaCap[3] * n3 + deltaCap[4] * n4;
                            double totalCharge = pCharge2 + deltaCharge[3] * n3 + deltaCharge[4] * n4;

                            // compute the orb-fill rate
                            double perCycle = Math.Min(totalLp, totalCap);
                            double perTick = perCycle / cycleTime;
                            double orbRate = Math.Min(perTick, totalCharge);

                            if (orbRate > bestRate)
                            {
                                bestRate = orbRate;
                                bestCounts[0] = n0;
                                bestCounts[1] = n1;
                                bestCounts[2] = n2;
                                bestCounts[3] = n3;
                                bestCounts[4] = n4;
                            }
                        }
                    }
                }
            }

            // 7) Build final altar exactly once
            var result = working.Copy();
            if (result.Orb == null)
                result.AddOrb(orb);

            for (int i = 0; i < runeCount; i++)
            {
                int cnt = bestCounts[i];
                if (cnt > 0)
                    result.AddRune(m_runes[i], cnt);
            }

            return result;
        }

        private bool CanBeat(
            double partialLp,
            double partialCap,
            double partialCharge,
            int remSlots,
            double maxDeltaLp,
            double maxDeltaCap,
            double maxDeltaCharge,
            double cycleTime,
            double currentBestRate)
        {
            // optimistic fill–all–remaining–slots with best single-slot rune
            double optimisticLp = partialLp + maxDeltaLp * remSlots;
            double optimisticCap = partialCap + maxDeltaCap * remSlots;
            double optimisticCharge = partialCharge + maxDeltaCharge * remSlots;

            double optPerCycle = Math.Min(optimisticLp, optimisticCap);
            double optPerTick = optPerCycle / cycleTime;
            double optRate = Math.Min(optPerTick, optimisticCharge);

            // if even optimism ≤ current best, prune
            return optRate > currentBestRate;
        }

        private void ApplyNetworkCapacityRunes(Altar baseAltar, IBloodOrb orb, int targetCapacity)
        {
            BaseRune? networkCapacityRune = null;

            foreach (var rune in m_runes)
            {
                var context = new AltarContext();
                var targetmodifier = context.SoulNetworkMultiplier;
                rune.ApplyRune(context, 1);
                if (context.SoulNetworkMultiplier > targetmodifier)
                {
                    networkCapacityRune = rune;
                    break;
                }
            }

            if (networkCapacityRune == null)
                return;

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

