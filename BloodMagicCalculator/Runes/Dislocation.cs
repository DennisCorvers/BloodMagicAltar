using BloodMagicCalculator.Calc;

namespace BloodMagicCalculator.Runes
{
    internal class Dislocation : BaseRune
    {
        public double IOBonus { get; }

        public override string Name => "Rune of Dislocation";

        public Dislocation(double ioBonus = .50d)
        {
            IOBonus = ioBonus;
        }

        public override void ApplyRune(AltarContext context, int count)
            => context.TransferSpeed *= Math.Pow((1 + IOBonus), count);
    }
}
