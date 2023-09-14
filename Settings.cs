using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace ArcanistTweaks
{
    public class Settings : ModSettings
    {
        public static ModEntry ModEntry;

        public bool TableTopSpellRanges = false;
        public bool LevelBasedRangeCalc = false;
        public bool MythicASI = false;
        public int MythicASIAmount = 2;

        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
