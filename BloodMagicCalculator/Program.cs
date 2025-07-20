using BloodMagicCalculator.Calc;
using BloodMagicCalculator.Runes;

namespace BloodMagicCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CalcOptimum();
        }

        static void CalcOptimum()
        {
            var possibleRunes = new List<BaseRune>()
            {
                new Runes.AugmentedCapacity(),
                new Runes.Orb(),
                new Runes.Sacrifice(),
                new Runes.Speed(),
                new Runes.SuperiorCapacity(),
            };

            var orb = new BloodOrb("Apprentice Blood Orb", 140, 80000000);
            var altar = new Altar(6);

            var calculator = new RuneCalculator(possibleRunes);
            var result = calculator.OptimiseOrbChargeRate(altar, orb);
        }
    }
}