using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using HarmonyLib;
using Kingmaker.Utility;
using UnityEngine;

namespace ArcanistTweaks
{
    class Tweaks
    {
        [HarmonyPatch(typeof(BlueprintAbility), "GetRange")]
        static class BlueprintAbility_GetRange_Patch
        {
            static void Postfix(ref Feet __result)
            {
                bool UseTTRanges = Main.Settings.TableTopSpellRanges;
                bool LvlBasedCalc = Main.Settings.LevelBasedRangeCalc;

                if (!Main.Enabled || (!LvlBasedCalc && !UseTTRanges)) return;

                int Level = Main.Settings.RangeLevel;
                if (__result == defaultClose)
                {
                    int lvlRangeMod = (int)(LvlBasedCalc ? (UseTTRanges ? (5 * Mathf.Floor(Level / 2)) : (5 * Mathf.Floor(Level / 6))) : 0);

                    __result = ((UseTTRanges ? 25 : 30) + lvlRangeMod).Feet();
                }
                else if (__result == defaultMedium)
                {
                    int lvlRangeMod = (int)(LvlBasedCalc ? (UseTTRanges ? (10 * Level) : (5 * Mathf.Floor(Level / 4))) : 0);

                    __result = ((UseTTRanges ? 100 : 40) + lvlRangeMod).Feet();
                }
                else if (__result == defaultLong)
                {
                    int lvlRangeMod = (int)(LvlBasedCalc ? (UseTTRanges ? (40 * Level) : (5 * Mathf.Floor(Level / 2))) : 0);

                    __result = ((UseTTRanges ? 400 : 50) + lvlRangeMod).Feet();
                }
            }

            private static Feet defaultClose = 30.Feet();

            private static Feet defaultMedium = 40.Feet();

            private static Feet defaultLong = 50.Feet();
        }

        class ContentAdder
        {
            [HarmonyPatch(typeof(BlueprintsCache), "Init")]
            static class BlueprintsCache_Patch
            {
                static bool Initialized;

                [HarmonyPriority(Priority.First)]
                static void Postfix()
                {
                    if (Initialized) return;
                    Initialized = true;
                    Main.LogHeader("Loading New Content");
                    Archetypes.SchoolSavant.Create();
                    MythicAbilities.Add();
                    Feats.add();
                }
            }

        }
    }
}
