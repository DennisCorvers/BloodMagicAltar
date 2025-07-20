namespace BloodMagicCalculator.Calc
{
    internal class AltarContext
    {
        public int FlatCapacityBonus { get; set; }
        public double TransferSpeed { get; set; }
        public double SacrificeMultiplier { get; set; }
        public double SelfSacrificeMultiplier { get; set; }
        public double CapacityMultiplier { get; set; }
        public double CraftingSpeed { get; set; }
        public double SoulNetworkMultiplier { get; set; }
        public int TickAcceleration { get; set; }

        public AltarContext()
        {
            FlatCapacityBonus = 0;
            TransferSpeed = 1;
            SacrificeMultiplier = 1;
            SelfSacrificeMultiplier = 1;
            CapacityMultiplier = 1;
            CraftingSpeed = 1;
            SoulNetworkMultiplier = 1;
            TickAcceleration = 0;
        }
    }
}
