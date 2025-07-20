using BloodMagicCalculator.Calc;

namespace BloodMagicCalculator.Runes
{
    internal class Orb : BaseRune
    {
        public double SoulNetworkBonus { get; }

        public override string Name => "Rune of the Orb";

        public Orb(double soulNetworkBonus = 0.04d)
        {
            SoulNetworkBonus = soulNetworkBonus;
        }

        public override void ApplyRune(AltarContext context, int count)
            => context.SoulNetworkMultiplier += count * SoulNetworkBonus;
    }
}
