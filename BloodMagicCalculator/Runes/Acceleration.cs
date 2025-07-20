using BloodMagicCalculator.Calc;

namespace BloodMagicCalculator.Runes
{
    internal class Acceleration : BaseRune
    {
        public int TicksAccelerated { get; }

        public override string Name => "Rune of Acceleration";

        public Acceleration(int ticksAccelerated = 1)
        {
            TicksAccelerated = ticksAccelerated;
        }

        public override void ApplyRune(AltarContext context, int count)
         => context.TickAcceleration += TicksAccelerated * count;
    }
}
