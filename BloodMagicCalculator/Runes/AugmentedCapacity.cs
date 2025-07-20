using BloodMagicCalculator.Calc;

namespace BloodMagicCalculator.Runes
{
    internal class AugmentedCapacity : BaseRune
    {
        public int CapacityBonus { get; }

        public override string Name => "Rune of Augmented Capacity";

        public AugmentedCapacity(int capacityBonus = 3500)
        {
            CapacityBonus = capacityBonus;
        }

        public override void ApplyRune(AltarContext context, int count)
            => context.FlatCapacityBonus += count * 3500;
    }
}
