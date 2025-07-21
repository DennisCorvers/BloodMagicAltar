namespace BloodMagicCalculator.Data
{
    internal enum WorldAcceleratorTier
    {
        None, LV, MV, HV, EV, IV, LuV, ZPM
    }

    internal enum WorldAcceleratorTarget
    {
        Altar, Ritual
    }

    internal static class WorldAcceleratorTierExtensions
    {
        private static readonly Dictionary<WorldAcceleratorTier, int> Multipliers = new()
    {
        { WorldAcceleratorTier.LV, 2 },
        { WorldAcceleratorTier.MV, 4 },
        { WorldAcceleratorTier.HV, 8 },
        { WorldAcceleratorTier.EV, 16 },
        { WorldAcceleratorTier.IV, 32 },
        { WorldAcceleratorTier.LuV, 64 },
        { WorldAcceleratorTier.ZPM, 128 },
    };

        public static int GetMultiplier(this WorldAcceleratorTier tier)
            => Multipliers.TryGetValue(tier, out var val) ? val : 0;
    }
}
