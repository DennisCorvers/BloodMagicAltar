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
                new Runes.Orb(),
                new Runes.Sacrifice(),
                new Runes.Speed(),
                new Runes.SuperiorCapacity(),
            };
        }

        public Altar OptimiseOrbChargeRate(Altar baseAltar, IBloodOrb orb, int networkCapacity = 0, bool preventDryAltar = false)
        {
            // 1) Handle network capacity setup
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

            // 2) Try every 5-tuple (rune counts summing to maxSlots)
            for (int n0 = 0; n0 <= maxSlots; n0++)
            {
                for (int n1 = 0; n1 <= maxSlots - n0; n1++)
                {
                    for (int n2 = 0; n2 <= maxSlots - n0 - n1; n2++)
                    {
                        for (int n3 = 0; n3 <= maxSlots - n0 - n1 - n2; n3++)
                        {
                            int n4 = maxSlots - n0 - n1 - n2 - n3;
                            int[] counts = new[] { n0, n1, n2, n3, n4 };

                            // Build full altar
                            var testAltar = working.Copy();
                            for (int i = 0; i < runeTypes; i++)
                                if (counts[i] > 0)
                                    testAltar.AddRune(m_runes[i], counts[i]);

                            // Add orb again (if not already applied inside AddRune logic)
                            testAltar.AddOrb(orb);

                            // Score based on fully derived stats
                            double lpPerCycle = Math.Min(testAltar.LPPerCycle, testAltar.Capacity);
                            double bloodPerTick = lpPerCycle / Altar.CycleTime;
                            double orbRate = Math.Min(bloodPerTick, testAltar.OrbChargeSpeed);

                            if (preventDryAltar && bloodPerTick < testAltar.OrbChargeSpeed)
                                continue; // discard config that risks starvation


                            if (orbRate > bestRate)
                            {
                                bestRate = orbRate;
                                bestAltar = testAltar.Copy();
                            }
                        }
                    }
                }
            }

            return bestAltar;
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

