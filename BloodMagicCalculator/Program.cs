using BloodMagicCalculator.Calc;
using BloodMagicCalculator.Runes;
using BloodMagicCalculator.Data;
using BloodMagicCalculator.Utils;

namespace BloodMagicCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Orb Charge Rate Optimizer ===\n");

            OrbType orbType = Prompt.PromptEnum<OrbType>("Select Orb Type:");
            int altarTier = Prompt.PromptInt("Enter Altar Tier (2-6):", min: 2, max: 6);

            Console.WriteLine("\n-- World Accelerators --");
            WorldAcceleratorTier ritualWA = Prompt.PromptEnum<WorldAcceleratorTier>("Select Ritual Accelerator Tier:");
            WorldAcceleratorTier altarWA = Prompt.PromptEnum<WorldAcceleratorTier>("Select Altar Accelerator Tier:");

            var networkSize = Prompt.PromptInt("Enter Network Size (leave empty or 0 for default):", allowEmpty: true);

            Console.WriteLine("\nCalculating optimal charge rate...");

            try
            {
                var orb = Data.Data.GetOrb((OrbType)orbType);
                var altar = new Altar(altarTier);

                altar.AddWorldAccelerator(ritualWA, WorldAcceleratorTarget.Ritual);
                altar.AddWorldAccelerator(altarWA, WorldAcceleratorTarget.Altar);

                var isCraft = orbType == OrbType.Crafting;
                var calculator = new RuneCalculator();
                var result = calculator.OptimiseOrbChargeRate(altar, orb, networkSize, isCraft);

                ShowResults(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        static void ShowResults(Altar result)
        {
            // If the "orb" has no capacity, assume we are infusing an item instead.
            var isCraft = result.Orb != null && result.Orb.Capacity == 0;

            Prompt.WriteColour("Optimised Altar Setup:", ConsoleColor.Green);

            Console.WriteLine(result.Orb != null ? result.Orb.Name : "NO TARGET");
            Console.WriteLine();
            Console.WriteLine($"{"Max Rune Slots",-20}: {result.MaxRuneSlots}");

            Console.WriteLine($"{"Capacity",-20}: {result.Capacity:N0}LP");
            Console.WriteLine($"{"I/O Buffer Capacity",-20}: {result.IOCapacity:N0}LP");
            Console.WriteLine();
            Console.WriteLine($"{"LP Per Cycle",-20}: {result.LPPerCycle:N0}LP");
            Console.WriteLine($"{"LP PerTick",-20}: {result.LPPerTick:N0}LP/t");
            Console.WriteLine();

            if (isCraft)
            {
                Console.WriteLine($"{"Crafting LP Usage",-20}: {result.CraftingLP:N0}LP/t");
            }
            else
            {
                Console.WriteLine($"{"Orb Capacity",-20}: {result.OrbCapacity:N0}LP");
                Console.WriteLine($"{"Orb Charge Speed",-20}: {result.OrbChargeSpeed:N0}LP/t");
            }

            Console.WriteLine();
            Prompt.WriteColour("Rune Configuration", ConsoleColor.Red);
            foreach (var runeEntry in result.Runes)
                Console.WriteLine($"    {runeEntry.Key.Name,-30}: {runeEntry.Value}");

            Console.WriteLine();
            Console.WriteLine($"{"Total Runes Used",-20}: {result.TotalRunesUsed}");

            Console.WriteLine($"{"Altar Acceleration",-20}: {result.AltarAcceleration}");
            Console.WriteLine($"{"Ritual Acceleration",-20}: {result.RitualAcceleration}");

            Console.ReadLine();
        }
    }
}