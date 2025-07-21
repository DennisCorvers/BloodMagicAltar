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
            altar.AddWorldAccelerator(WorldAcceleratorTier.HV, WorldAcceleratorTarget.Ritual);
            altar.AddWorldAccelerator(WorldAcceleratorTier.HV, WorldAcceleratorTarget.Altar);

            var calculator = new RuneCalculator(runesToCheck);
            var result = calculator.OptimiseOrbChargeRate(altar, orb, 125000000);

            ShowResults(result);
        }

        static void ShowResults(Altar result)
        {
            Console.WriteLine($"Optimised Altar Setup:");

            Console.WriteLine(result.Orb != null ? result.Orb.Name : "NO TARGET");
            Console.WriteLine();
            Console.WriteLine($"{"Max Rune Slots",-20}: {result.MaxRuneSlots}");

            Console.WriteLine($"{"Capacity",-20}: {result.Capacity:N0}LP");
            Console.WriteLine($"{"I/O Buffer Capacity",-20}: {result.IOCapacity:N0}LP");
            Console.WriteLine();
            Console.WriteLine($"{"LP Per Cycle",-20}: {result.LPPerCycle:N0}LP");
            Console.WriteLine($"{"LP PerTick",-20}: {result.LPPerTick:N0}LP/t");
            Console.WriteLine();
            Console.WriteLine($"{"Orb Capacity",-20}: {result.OrbCapacity:N0}LP");
            Console.WriteLine($"{"Orb Charge Speed",-20}: {result.OrbChargeSpeed:N0}LP/t");

            Console.WriteLine();
            Console.WriteLine("Rune Configuration:");
            foreach (var runeEntry in result.Runes)
                Console.WriteLine($"    {runeEntry.Key.Name,-30}: {runeEntry.Value}");

            Console.WriteLine();
            Console.WriteLine($"{"Total Runes Used",-20}: {result.TotalRunesUsed}");

            Console.WriteLine($"{"Altar Acceleration",-20}: {result.AltarAcceleration}");
            Console.WriteLine($"{"Ritual Acceleration",-20}: {result.RitualAcceleration}");
        }
    }
}