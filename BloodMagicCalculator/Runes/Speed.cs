using BloodMagicCalculator.Calc;

namespace BloodMagicCalculator.Runes
{
    internal class Speed : BaseRune
    {
        public double SpeedBonus { get; }

        public override string Name => "Speed Rune";

        public Speed(double speedBonus = 0.25d)
        {
            SpeedBonus = speedBonus;
        }

        public override void ApplyRune(AltarContext context, int count)
            => context.CraftingSpeed += count * SpeedBonus;
    }
}
