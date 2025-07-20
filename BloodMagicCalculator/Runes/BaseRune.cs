using BloodMagicCalculator.Calc;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BloodMagicCalculator.Runes
{
    [DebuggerDisplay("Name = {Name}")]
    internal abstract class BaseRune
    {
        public abstract string Name { get; }

        public abstract void ApplyRune(AltarContext context, int count);
    }
}
