using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using HarmonyLib;
using Kingmaker.Utility;
using UnityEngine;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic;

namespace ArcanistTweaks
{
    class Tweaks
    {
        [HarmonyPatch(typeof(AbilityData), nameof(AbilityData.GetApproachDistance))]
        static class AbilityData_Range_Get_Patch
        {
            [HarmonyPostfix]
            static void fix(ref float __result, AbilityData __instance)
            {
                bool UseTTRanges = Main.Settings.TableTopSpellRanges;
                bool LvlBasedCalc = Main.Settings.LevelBasedRangeCalc;

                if (!Main.Enabled || !__instance.Blueprint.IsSpell || (!LvlBasedCalc && !UseTTRanges)) return;

                int Level = __instance.Spellbook.CasterLevel;

                if (__instance.Range == AbilityRange.Close)
                {
                    int lvlRangeMod = (int)(LvlBasedCalc ? (UseTTRanges ? (5 * Mathf.Floor(Level / 2)) : (1 * Level)) : 0);

                    __result = ((UseTTRanges ? 25 : 30) + lvlRangeMod).Feet().Meters;
                }
                else if (__instance.Range == AbilityRange.Medium)
                {
                    int lvlRangeMod = (int)(LvlBasedCalc ? (UseTTRanges ? (10 * Level) : (1 * Level)) : 0);

                    __result = ((UseTTRanges ? 100 : 40) + lvlRangeMod).Feet().Meters;
                }
                else if (__instance.Range == AbilityRange.Long)
                {
                    int lvlRangeMod = (int)(LvlBasedCalc ? (UseTTRanges ? (40 * Level) : (1 * Level)) : 0);

                    __result = ((UseTTRanges ? 400 : 50) + lvlRangeMod).Feet().Meters;
                }
            }
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
