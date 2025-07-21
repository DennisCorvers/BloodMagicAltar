using BloodMagicCalculator.Calc;
using BloodMagicCalculator.Runes;
using BloodMagicCalculator.Data;

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
            var runesToCheck = new List<BaseRune>()
            {
                new Runes.AugmentedCapacity(),
                new Runes.Orb(),
                new Runes.Sacrifice(),
                new Runes.Speed(),
                new Runes.SuperiorCapacity(),
            };

            var orb = Data.Data.GetOrb(OrbType.EldritchBloodOrb);
            var altar = new Altar(6);

            var calculator = new RuneCalculator(runesToCheck);
            var result = calculator.OptimiseOrbChargeRate(altar, orb);
        }
    }
}