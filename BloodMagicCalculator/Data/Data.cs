using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodMagicCalculator.Data
{
    internal class Data
    {
        private static readonly IReadOnlyDictionary<OrbType, IBloodOrb> AltarTargets = new Dictionary<OrbType, IBloodOrb>()
        {
            {OrbType.WeakBloodOrb,          new BloodOrb("Weak Blood Orb", 2, 5000) },
            {OrbType.ApprenticeBloodOrb,    new BloodOrb("Apprentice Blood Orb", 5, 25000) },
            {OrbType.MagiciansBloodOrb,     new BloodOrb("Magician's Blood Orb", 15, 150000) },
            {OrbType.MasterBloodOrb,        new BloodOrb("Master Blood Orb", 25, 1000000) },
            {OrbType.ArchmagesBloodOrb,     new BloodOrb("Archmage's Blood Orb", 50, 10000000) },
            {OrbType.TranscendentBloodOrb,  new BloodOrb("Transcendent Blood Orb", 100, 30000000) },
            {OrbType.EldritchBloodOrb,      new BloodOrb("Eldritch Blood Orb", 140, 80000000) },
            {OrbType.Crafting,              new BloodOrb("Crafting", 20, 0) },
        };

        public static IBloodOrb GetOrb(OrbType orbType)
        {
            return AltarTargets[orbType];
        }
    }

    internal enum OrbType
    {
        WeakBloodOrb,
        ApprenticeBloodOrb,
        MagiciansBloodOrb,
        MasterBloodOrb,
        ArchmagesBloodOrb,
        TranscendentBloodOrb,
        EldritchBloodOrb,
        Crafting
    }
}
