using BloodMagicCalculator.Runes;

namespace BloodMagicCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var selfSacrifice = new Runes.SelfSacrifice();
            var speed = new Runes.Speed();
            var capacity = new Runes.AugmentedCapacity();
            var supCapacity = new Runes.SuperiorCapacity();
            var orbRune = new Runes.Orb();

            var orb = new BloodOrb("Apprentice Blood Orb", 5, 25000);

            var altar = new Altar(6);

            altar.AddRune(selfSacrifice, 10);
            altar.AddRune(speed, 10);
            altar.AddRune(capacity, 10);
            altar.AddRune(supCapacity, 10);
            altar.AddRune(orbRune, 37);

            altar.AddOrb(orb);
        }
    }
}