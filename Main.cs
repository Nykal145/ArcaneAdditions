using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using Kingmaker.Blueprints.JsonSystem;
using JetBrains.Annotations;
using UnityEngine.UI;
using HarmonyLib;
using Kingmaker;
using System.Linq;
using Kingmaker.Blueprints;

namespace ArcanistTweaks
{
    static class Main
    {
        public static Settings Settings;
        public static bool Enabled;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Settings = Settings.Load<Settings>(modEntry);
            Settings.ModEntry = modEntry;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();
            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Use tabletop spell ranges    ", GUILayout.ExpandWidth(false));
            GUILayout.Space(25);
            Settings.TableTopSpellRanges = GUILayout.Toggle(Settings.TableTopSpellRanges, $"", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use caster level to calculate range", GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            Settings.LevelBasedRangeCalc = GUILayout.Toggle(Settings.LevelBasedRangeCalc, $"", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Add Mythic Ability Score increases", GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            Settings.MythicASI = GUILayout.Toggle(Settings.MythicASI, $"", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Caster Level for spell range calculation", GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            Settings.RangeLevel = Mathf.RoundToInt(GUILayout.HorizontalSlider(Settings.RangeLevel, 1, 20, GUILayout.Width(300f)));
            GUILayout.Space(5);
            GUILayout.Label(Settings.RangeLevel.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mythic Ability Score Increase amount  ", GUILayout.ExpandWidth(false));
            GUILayout.Space(20);
            Settings.MythicASIAmount = Mathf.RoundToInt(GUILayout.HorizontalSlider(Settings.MythicASIAmount, 1, 5, GUILayout.Width(300f)));
            GUILayout.Space(5);
            GUILayout.Label(Settings.MythicASIAmount.ToString());
            GUILayout.EndHorizontal();
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }
        public static void Log(string msg)
        {
            Settings.ModEntry.Logger.Log(msg);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug(string msg)
        {
            Settings.ModEntry.Logger.Log(msg);
        }
        public static void LogPatch(string action, [NotNull] IScriptableObjectWithAssetId bp)
        {
            Log($"{action}: {bp.AssetGuid} - {bp.name}");
        }
        public static void LogHeader(string msg)
        {
            Log($"--{msg.ToUpper()}--");
        }
        public static Exception Error(String message)
        {
            Log(message);
            return new InvalidOperationException(message);
        }
    }
}
