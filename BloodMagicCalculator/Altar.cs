using BloodMagicCalculator.Calc;
using BloodMagicCalculator.Data;
using BloodMagicCalculator.Runes;
using System.Collections.ObjectModel;

namespace BloodMagicCalculator
{
    internal class Altar
    {
        public const int CycleTime = 25;

        // Base values
        private readonly int m_altarTier;
        private readonly int m_baseCapacity;
        private readonly int m_baseCraftingRate;
        private readonly int m_baseLPGeneration;
        private readonly Dictionary<BaseRune, int> m_runes;

        private int m_reservedrunes = 0;
        private int m_totalRunesUsed = 0;
        private int m_altarAcceleration = 1;
        private int m_ritualAcceleration = 1;
        private AltarContext? m_statCache = null;
        private IBloodOrb? m_orb = null;

        public int MaxRuneSlots { get; }
        public int ReservedRunes
        {
            get => m_reservedrunes;
            private set => m_reservedrunes = Math.Min(value, MaxRuneSlots);
        }
        public int TotalRunesUsed
        { get => m_totalRunesUsed; }

        public AltarContext AltarStats
        {
            get
            {
                if (m_statCache == null)
                {
                    var context = new AltarContext();
                    foreach (var rune in m_runes)
                        rune.Key.ApplyRune(context, rune.Value);

                    m_statCache = context;
                }
                return m_statCache;
            }
        }
        public IReadOnlyDictionary<BaseRune, int> Runes
            => m_runes;

        public int AltarAcceleration => m_altarAcceleration;

        public int RitualAcceleration => m_ritualAcceleration;

        /// <summary>
        /// Total capacity of the altar
        /// </summary>
        public int Capacity
        {
            get
            {
                return (int)Math.Floor(m_baseCapacity * AltarStats.CapacityMultiplier + AltarStats.FlatCapacityBonus);
            }
        }

        /// <summary>
        /// Input/output buffer capacity.
        /// </summary>
        public int IOCapacity
        {
            get => (int)Math.Floor(Capacity * 0.1d);
        }

        /// <summary>
        /// Amount of LP used per tick for crafting
        /// </summary>
        public int CraftingLP
        {
            get
            {
                var baseRate = (int)Math.Floor(m_baseCraftingRate * AltarStats.CraftingSpeed);
                return baseRate * m_altarAcceleration;
            }
        }

        /// <summary>
        /// The amount of LP gained per Well of Suffering cycle
        /// </summary>
        public int LPPerCycle
        {
            get
            {
                var baseLP = (int)Math.Floor(m_baseLPGeneration * AltarStats.SacrificeMultiplier);
                return baseLP * m_ritualAcceleration;
            }
        }

        public int LPPerTick
            => LPPerCycle / CycleTime;

        /// <summary>
        /// Also mandates crafting speed
        /// </summary>
        public int OrbChargeSpeed
        {
            get
            {
                if (m_orb == null) return 0;
                var baseRate = (int)Math.Floor(m_orb.ChargeSpeed * AltarStats.CraftingSpeed);
                return baseRate * m_altarAcceleration;
            }
        }

        public int OrbCapacity
        {
            get => m_orb == null
                ? 0
                : (int)Math.Floor(m_orb.Capacity * AltarStats.SoulNetworkMultiplier);
        }

        public IBloodOrb? Orb => m_orb;

        public Altar(int altarTier, int capacity = 10000, int baseCraftingRate = 20, int lpPerCycle = 2500, int reservedRunes = 0)
        {
            m_altarTier = altarTier;
            m_baseCapacity = capacity;
            m_baseCraftingRate = baseCraftingRate;
            m_baseLPGeneration = lpPerCycle;
            MaxRuneSlots = GetMaxRuneCount(altarTier);

            ReservedRunes = reservedRunes;
            m_totalRunesUsed = ReservedRunes;
            m_runes = new Dictionary<BaseRune, int>(MaxRuneSlots);
            m_altarAcceleration = 1;
            m_ritualAcceleration = 1;
        }

        public void AddWorldAccelerator(WorldAcceleratorTier wa, WorldAcceleratorTarget target)
        {
            if (target == WorldAcceleratorTarget.Ritual)
            {
                m_ritualAcceleration = 1 + wa.GetMultiplier();
            }
            else
            {
                m_altarAcceleration = 1 + wa.GetMultiplier();
            }
        }

        public void AddOrb(IBloodOrb orb)
        {
            m_orb = orb;
        }

        public void AddRune(BaseRune rune, int amount = 1)
        {
            if (amount <= 0)
                return;

            int available = MaxRuneSlots - m_totalRunesUsed;
            if (available <= 0)
                throw new InvalidOperationException("Altar is full.");

            int toAdd = Math.Min(amount, available);

            if (m_runes.ContainsKey(rune))
                m_runes[rune] += toAdd;
            else
                m_runes[rune] = toAdd;

            m_totalRunesUsed += toAdd;
            m_statCache = null;
        }

        public void RemoveRune(BaseRune rune, int amount = -1)
        {
            if (!m_runes.ContainsKey(rune))
                return;

            if (amount == -1 || m_runes[rune] <= amount)
            {
                m_totalRunesUsed -= m_runes[rune];
                m_runes.Remove(rune);
            }
            else
            {
                m_runes[rune] -= amount;
                m_totalRunesUsed -= amount;
            }

            m_statCache = null;
        }

        public void ClearRunes()
        {
            m_totalRunesUsed -= m_runes.Values.Sum();
            m_runes.Clear();
            m_statCache = null;
        }

        public Altar Copy()
        {
            var copy = new Altar(m_altarTier, m_baseCapacity, m_baseCraftingRate, m_baseLPGeneration, ReservedRunes);

            foreach (var kvp in m_runes)
                copy.m_runes[kvp.Key] = kvp.Value;

            copy.m_totalRunesUsed = m_totalRunesUsed;
            copy.m_orb = m_orb;
            copy.m_ritualAcceleration = m_ritualAcceleration;
            copy.m_altarAcceleration = m_altarAcceleration;
            copy.m_statCache = null;

            return copy;
        }

        private static int GetMaxRuneCount(int altarTier)
        {
            return altarTier switch
            {
                2 => 8,
                3 => 28,
                4 => 56,
                5 => 108,
                6 => 184,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
