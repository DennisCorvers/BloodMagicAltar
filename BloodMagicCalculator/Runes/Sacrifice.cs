using BloodMagicCalculator.Calc;

namespace BloodMagicCalculator.Runes
{
    internal class Sacrifice : BaseRune
    {
        public double LPBonus { get; }

        public override string Name => "Rune of Sacrifice";

        public Sacrifice(double lPBonus = 0.12d)
        {
            LPBonus = lPBonus;
        }

        public override void ApplyRune(AltarContext context, int count)
            => context.SacrificeMultiplier += count * LPBonus;
    }
}
