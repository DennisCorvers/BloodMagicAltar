using BloodMagicCalculator.Calc;
using BloodMagicCalculator.Runes;
using System.Collections.ObjectModel;

namespace BloodMagicCalculator
{
    internal class Altar
    {
        private const int CycleTime = 25;

        // Base values
        private readonly int m_baseCapacity;
        private readonly int m_baseCraftingRate;
        private readonly int m_baseLPGeneration;
        private readonly Dictionary<BaseRune, int> m_runes;
        private int m_reservedrunes = 0;
        private int m_totalRunesUsed = 0;
        private WorldAcceleratorTier m_acceleratorTier;
        private int m_acceleration;
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

        public WorldAcceleratorTier WorldAccelerator
        {
            get
            {
                return m_acceleratorTier;
            }
            set
            {
                m_acceleratorTier = value;
                m_acceleration = value.GetMultiplier();
            }
        }
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
            get => (int)Math.Floor(m_baseCraftingRate * AltarStats.CraftingSpeed);
        }

        /// <summary>
        /// The amount of LP gained per Well of Suffering cycle
        /// </summary>
        public int LPPerCycle
        {
            get => (int)Math.Floor(m_baseLPGeneration * AltarStats.SacrificeMultiplier) * m_acceleration;
        }

        public int LPPerTick
            => LPPerCycle / CycleTime;

        public int OrbChargeSpeed
        {
            get => m_orb == null
                ? 0
                : (int)Math.Floor(m_orb.ChargeSpeed * AltarStats.CraftingSpeed);
        }

        public int OrbCapacity
        {
            get => m_orb == null
                ? 0
                : (int)Math.Floor(m_orb.Capacity * AltarStats.SoulNetworkMultiplier);
        }

        public Altar(int altarTier, int capacity = 10000, int craftingRate = 20, int lpPerCycle = 2500, int reservedRunes = 0)
        {
            m_baseCapacity = capacity;
            m_baseCraftingRate = craftingRate;
            m_baseLPGeneration = lpPerCycle;
            MaxRuneSlots = GetMaxRuneCount(altarTier);

            ReservedRunes = reservedRunes;
            m_totalRunesUsed = ReservedRunes;
            m_runes = new Dictionary<BaseRune, int>(MaxRuneSlots);
            m_acceleratorTier = WorldAcceleratorTier.None;
            m_acceleration = 1;
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
