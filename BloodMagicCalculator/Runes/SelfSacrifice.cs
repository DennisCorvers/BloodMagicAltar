using BloodMagicCalculator.Calc;

namespace BloodMagicCalculator.Runes
{
    internal class SelfSacrifice : BaseRune
    {
        public double LPBonus { get; }

        public override string Name => "Rune of Self-Sacrifice";

        public SelfSacrifice(double lPBonus = 0.12d)
        {
            LPBonus = lPBonus;
        }

        public override void ApplyRune(AltarContext context, int count)
            => context.SelfSacrificeMultiplier += count * LPBonus;
    }
}
