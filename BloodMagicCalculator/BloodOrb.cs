namespace BloodMagicCalculator
{
    internal interface IBloodOrb
    {
        int ChargeSpeed { get; }
        int Capacity { get; }
    }

    internal class BloodOrb : IBloodOrb
    {
        public string Name { get; }

        public int ChargeSpeed { get; }

        public int Capacity { get; }

        public BloodOrb(string name, int chargeSpeed, int capacity)
        {
            Name = name;
            ChargeSpeed = chargeSpeed;
            Capacity = capacity;
        }
    }
}
