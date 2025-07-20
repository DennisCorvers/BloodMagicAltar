using BloodMagicCalculator.Calc;

namespace BloodMagicCalculator.Runes
{
    internal class SuperiorCapacity : BaseRune
    {
        public double CapacityBonus { get; }

        public override string Name => "Rune of Superior Capacity";

        public SuperiorCapacity(double capacityBonus = 0.14d)
        {
            CapacityBonus = capacityBonus;
        }

        public override void ApplyRune(AltarContext context, int count)
         => context.CapacityMultiplier *= Math.Pow(1 + CapacityBonus, count);
    }
}
