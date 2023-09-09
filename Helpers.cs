// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

// Base copied from Arcanist Tweaks]]

using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Designers.Mechanics.Buffs;
using HarmonyLib;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.UI;
using Kingmaker.UnitLogic.ActivatableAbilities;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Loot;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.EventConditionActionSystem.Conditions;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UI.Log;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics.Actions;
using UnityEngine.UI;
using UnityModManagerNet;
using static Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbilityResourceLogic;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.Blueprints.Items.Components;
using Kingmaker.Designers.Mechanics.Prerequisites;
using System.IO;



namespace ArcanistTweaks {
    public static class Helpers {

        const string basicFeatSelection = "247a4068296e8be42890143f451b4b45";

        public static T Create<T>(Action<T> init = null) where T : new() {
            var result = new T();
            init?.Invoke(result);
            return result;
        }

        public static BlueprintBuff CreateBuff(Action<BlueprintBuff> init = null) {
            var result = Helpers.Create<BlueprintBuff>(bp => {
                bp.FxOnStart = new PrefabLink();
                bp.FxOnRemove = new PrefabLink();
            });
            init?.Invoke(result);
            return result;
        }

        public static BlueprintBuff CreateBuff(String name, String displayName, String description, String guid, UnityEngine.Sprite icon,
            PrefabLink fxOnStart,
            params BlueprintComponent[] components)
        {
            var buff = Create<BlueprintBuff>();
            buff.name = name;
            buff.FxOnStart = fxOnStart ?? new PrefabLink();
            buff.FxOnRemove = new PrefabLink();
            buff.SetComponents(components);
            buff.m_DisplayName = CreateString($"{buff.name}.Name", displayName);
            buff.m_Description = CreateString($"{buff.name}.Description", description);
            buff.m_Icon = icon;
            //Resources.AddAsset(buff, guid);
            return buff;
        }

        public static BlueprintActivatableAbility CreateActivatableAbility(String name, String displayName, String description,
            string assetId, UnityEngine.Sprite icon, BlueprintBuff buff, AbilityActivationType activationType, CommandType commandType,
            UnityEngine.AnimationClip activateWithUnitAnimation,
            params BlueprintComponent[] components)
        {
            var ability = Create<BlueprintActivatableAbility>();
            ability.name = name;
            ability.m_DisplayName = CreateString($"{ability.name}.Name", displayName);
            ability.m_Description = CreateString($"{ability.name}.Description", description);
            ability.m_Icon = icon;
            ability.m_Buff = buff.ToReference<BlueprintBuffReference>();
            ability.ResourceAssetIds = Array.Empty<string>();
            ability.ActivationType = activationType;
            ability.m_ActivateWithUnitCommand = commandType;
            ability.SetComponents(components);
            //ability.ActivateWithUnitAnimation = activateWithUnitAnimation;
            //Main.library.AddAsset(ability, assetId);
            return ability;
        }

        public static BlueprintFeature CreateFeature(String name, String displayName, String description, BlueprintGuid guid, UnityEngine.Sprite icon,
            FeatureGroup group, params BlueprintComponent[] components)
        {
            var feat = Create<BlueprintFeature>();
            SetFeatureInfo(feat, name, displayName, description, guid, icon, group, components);
            return feat;
        }

        public static void SetFeatureInfo(BlueprintFeature feat, String name, String displayName, String description, BlueprintGuid guid, UnityEngine.Sprite icon,
           FeatureGroup group, params BlueprintComponent[] components)
        {
            feat.name = name;
            feat.SetComponents(components);
            feat.Groups = new FeatureGroup[] { group };
            feat.m_DisplayName = CreateString($"{feat.name}.Name", displayName);
            feat.m_Description = CreateString($"{feat.name}.Description", description);
            feat.m_Icon = icon;
            feat.AssetGuid = guid;
        }

        public static int PopulationCount(int i) 
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        public static ActivatableAbilityResourceLogic CreateActivatableResourceLogic(this BlueprintAbilityResource resource,
            ResourceSpendType spendType)
        {
            var logic = Create<ActivatableAbilityResourceLogic>();
            logic.m_RequiredResource = resource.ToReference<BlueprintAbilityResourceReference>();
            logic.SpendType = spendType;
            return logic;
        }

        public static AddFacts CreateAddFact(this BlueprintUnitFact fact)
        {
            var result = Create<AddFacts>();
            result.name = $"AddFacts${fact.name}";
            result.m_Facts = new BlueprintUnitFactReference[] { fact.ToReference<BlueprintUnitFactReference>() };
            return result;
        }

        public static T CreateCopy<T>(T original, Action<T> init = null)
        {
            var result = (T)ObjectDeepCopier.Clone(original);
            init?.Invoke(result);
            return result;
        }

        public static LevelEntry LevelEntry(int level, BlueprintFeatureBase feature) {
            return new LevelEntry {
                Level = level,
                Features = {
                    feature
                }
            };
        }

        public static LevelEntry LevelEntry(int level, params BlueprintFeatureBase[] features) {
            return LevelEntry(level, features);
        }

        public static LevelEntry LevelEntry(int level, IEnumerable<BlueprintFeatureBase> features) {
            LevelEntry levelEntry = new LevelEntry();
            levelEntry.Level = level;
            features.ForEach(f => levelEntry.Features.Add(f));
            return levelEntry;
        }
        public static ContextValue CreateContextValueRank(AbilityRankType value = AbilityRankType.Default) => value.CreateContextValue();
        public static ContextValue CreateContextValue(this AbilityRankType value) {
            return new ContextValue() { ValueType = ContextValueType.Rank, ValueRank = value };
        }
        public static ContextValue CreateContextValue(this AbilitySharedValue value) {
            return new ContextValue() { ValueType = ContextValueType.Shared, ValueShared = value };
        }
        public static ActionList CreateActionList(params GameAction[] actions) {
            if (actions == null || actions.Length == 1 && actions[0] == null) actions = Array.Empty<GameAction>();
            return new ActionList() { Actions = actions };
        }
#if false
        public static ContextActionSavingThrow CreateActionSavingThrow(this SavingThrowType savingThrow, params GameAction[] actions) {
            var c = Create<ContextActionSavingThrow>();
            c.Type = savingThrow;
            c.Actions = CreateActionList(actions);
            return c;
        }
        public static ContextActionConditionalSaved CreateConditionalSaved(GameAction[] success, GameAction[] failed) {
            var c = Create<ContextActionConditionalSaved>();
            c.Succeed = CreateActionList(success);
            c.Failed = CreateActionList(failed);
            return c;
        }
#endif

        // All localized strings created in this mod, mapped to their localized key. Populated by CreateString.
        static readonly Dictionary<String, LocalizedString> textToLocalizedString = new Dictionary<String, LocalizedString>();
        public static LocalizedString CreateString(string key, string value)
        {
            // See if we used the text previously.
            // (It's common for many features to use the same localized text.
            // In that case, we reuse the old entry instead of making a new one.)
            if (textToLocalizedString.TryGetValue(value, out var localized))
            {
                return localized;
            }

#if DEBUG
            var current = LocalizationManager.CurrentPack.GetText(key, false);
            if (current != "" && current != value) {
                Main.LogDebug($"Info: duplicate localized string `{key}`, different text.");
            }
#endif
            LocalizationManager.CurrentPack.PutString(key, value);
            localized = new LocalizedString { m_ShouldProcess = false, m_Key = key };
            textToLocalizedString[value] = localized;
            return localized;
        }
        public static FastRef<T, S> CreateFieldSetter<T, S>(string name) {
            return new FastRef<T, S>(HarmonyLib.AccessTools.FieldRefAccess<T, S>(HarmonyLib.AccessTools.Field(typeof(T), name)));
            //return new FastSetter<T, S>(HarmonyLib.FastAccess.CreateSetterHandler<T, S>(HarmonyLib.AccessTools.Field(typeof(T), name)));
        }

        public static FastRef<T, S> CreateFieldGetter<T, S>(string name) {
            return new FastRef<T, S>(HarmonyLib.AccessTools.FieldRefAccess<T, S>(HarmonyLib.AccessTools.Field(typeof(T), name)));
            //return new FastGetter<T, S>(HarmonyLib.FastAccess.CreateGetterHandler<T, S>(HarmonyLib.AccessTools.Field(typeof(T), name)));
        }
        public static void SetField(object obj, string name, object value) {
            HarmonyLib.AccessTools.Field(obj.GetType(), name).SetValue(obj, value);
        }
        public static object GetField(object obj, string name)
        {
            return AccessTools.Field(obj.GetType(), name).GetValue(obj);
        }

        public static object GetField(Type type, object obj, string name)
        {
            return AccessTools.Field(type, name).GetValue(obj);
        }

        public static T GetField<T>(object obj, string name)
        {
            return (T)AccessTools.Field(obj.GetType(), name).GetValue(obj);
        }

        public static NewMechanics.ContextCalculateAbilityParamsBasedOnClasses createContextCalculateAbilityParamsBasedOnClassesWithArchetypes(BlueprintCharacterClass[] character_classes,
                                                                                                                                              BlueprintArchetype[] archetypes,
                                                                                                                                              StatType stat)
        {
            var c = Helpers.Create<NewMechanics.ContextCalculateAbilityParamsBasedOnClasses>();
            c.CharacterClasses = character_classes;
            c.StatType = stat;
            c.archetypes = archetypes == null ? new BlueprintArchetype[0] : archetypes;
            return c;
        }


        // Parses the lowest 64 bits of the Guid (which corresponds to the last 16 characters).
        static ulong ParseGuidLow(String id) => ulong.Parse(id.Substring(id.Length - 16), NumberStyles.HexNumber);
        // Parses the high 64 bits of the Guid (which corresponds to the first 16 characters).
        static ulong ParseGuidHigh(String id) => ulong.Parse(id.Substring(0, id.Length - 16), NumberStyles.HexNumber);
        public static String MergeIds(String guid1, String guid2, String guid3 = null) {
            // Parse into low/high 64-bit numbers, and then xor the two halves.
            ulong low = ParseGuidLow(guid1);
            ulong high = ParseGuidHigh(guid1);

            low ^= ParseGuidLow(guid2);
            high ^= ParseGuidHigh(guid2);

            if (guid3 != null) {
                low ^= ParseGuidLow(guid3);
                high ^= ParseGuidHigh(guid3);
            }
            return high.ToString("x16") + low.ToString("x16");
        }
       
        private class ObjectDeepCopier {
            private class ArrayTraverse {
                public int[] Position;
                private int[] maxLengths;

                public ArrayTraverse(Array array) {
                    maxLengths = new int[array.Rank];
                    for (int i = 0; i < array.Rank; ++i) {
                        maxLengths[i] = array.GetLength(i) - 1;
                    }
                    Position = new int[array.Rank];
                }

                public bool Step() {
                    for (int i = 0; i < Position.Length; ++i) {
                        if (Position[i] < maxLengths[i]) {
                            Position[i]++;
                            for (int j = 0; j < i; j++) {
                                Position[j] = 0;
                            }
                            return true;
                        }
                    }
                    return false;
                }
            }
            private class ReferenceEqualityComparer: EqualityComparer<Object> {
                public override bool Equals(object x, object y) {
                    return ReferenceEquals(x, y);
                }
                public override int GetHashCode(object obj) {
                    if (obj == null) return 0;
                    return obj.GetHashCode();
                }
            }
            private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

            public static bool IsPrimitive(Type type) {
                if (type == typeof(String)) return true;
                return (type.IsValueType & type.IsPrimitive);
            }
            public static Object Clone(Object originalObject) {
                return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
            }
            private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited) {
                if (originalObject == null) return null;
                var typeToReflect = originalObject.GetType();
                if (IsPrimitive(typeToReflect)) return originalObject;
                if (visited.ContainsKey(originalObject)) return visited[originalObject];
                if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
                var cloneObject = CloneMethod.Invoke(originalObject, null);
                if (typeToReflect.IsArray) {
                    var arrayType = typeToReflect.GetElementType();
                    if (IsPrimitive(arrayType) == false) {
                        Array clonedArray = (Array)cloneObject;
                        ForEach(clonedArray, (array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                    }

                }
                visited.Add(originalObject, cloneObject);
                CopyFields(originalObject, visited, cloneObject, typeToReflect);
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
                return cloneObject;

                void ForEach(Array array, Action<Array, int[]> action) {
                    if (array.LongLength == 0) return;
                    ArrayTraverse walker = new ArrayTraverse(array);
                    do action(array, walker.Position);
                    while (walker.Step());
                }
            }
            private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect) {
                if (typeToReflect.BaseType != null) {
                    RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                    CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
                }
            }
            private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null) {
                foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags)) {
                    if (filter != null && filter(fieldInfo) == false) continue;
                    if (IsPrimitive(fieldInfo.FieldType)) continue;
                    var originalFieldValue = fieldInfo.GetValue(originalObject);
                    var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                    fieldInfo.SetValue(cloneObject, clonedFieldValue);
                }
            }
        }
        public static void AddComponent(this BlueprintScriptableObject obj, BlueprintComponent component)
        {
            obj.SetComponents(obj.ComponentsArray.AddToArray(component));
        }

        public static void RemoveComponent(this BlueprintScriptableObject obj, BlueprintComponent component)
        {
            obj.SetComponents(obj.ComponentsArray.RemoveFromArray(component));
        }


        public static void RemoveComponents<T>(this BlueprintScriptableObject obj) where T : BlueprintComponent
        {
            var compnents_to_remove = obj.GetComponents<T>().ToArray();
            foreach (var c in compnents_to_remove)
            {
                obj.SetComponents(obj.ComponentsArray.RemoveFromArray(c));
            }
        }


        public static void RemoveComponents<T>(this BlueprintScriptableObject obj, Predicate<T> predicate) where T : BlueprintComponent
        {
            var compnents_to_remove = obj.GetComponents<T>().ToArray();
            foreach (var c in compnents_to_remove)
            {
                if (predicate(c))
                {
                    obj.SetComponents(obj.ComponentsArray.RemoveFromArray(c));
                }
            }
        }

        public static void AddComponents(this BlueprintScriptableObject obj, IEnumerable<BlueprintComponent> components) => AddComponents(obj, components.ToArray());

        public static void AddComponents(this BlueprintScriptableObject obj, params BlueprintComponent[] components)
        {
            var c = obj.ComponentsArray.ToList();
            c.AddRange(components);
            obj.SetComponents(c.ToArray());
        }

        public static void SetComponents(this BlueprintScriptableObject obj, params BlueprintComponent[] components)
        {
            // Fix names of components. Generally this doesn't matter, but if they have serialization state,
            // then their name needs to be unique.
            var names = new HashSet<string>();
            foreach (var c in components)
            {
                if (string.IsNullOrEmpty(c.name))
                {
                    c.name = $"${c.GetType().Name}";
                }
                if (!names.Add(c.name))
                {
                    String name;
                    for (int i = 0; !names.Add(name = $"{c.name}${i}"); i++) ;
                    c.name = name;
                }
            }

            obj.ComponentsArray = components;
        }

        public static void SetComponents(this BlueprintScriptableObject obj, IEnumerable<BlueprintComponent> components)
        {
            SetComponents(obj, components.ToArray());
        }

        public static T[] RemoveFromArray<T>(this T[] array, T value)
        {
            var list = array.ToList();
            return list.Remove(value) ? list.ToArray() : array;
        }

        public static void ReplaceComponent<T>(this BlueprintScriptableObject obj, BlueprintComponent replacement) where T : BlueprintComponent
        {
            ReplaceComponent(obj, obj.GetComponent<T>(), replacement);
        }


        public static void ReplaceComponent<T>(this BlueprintScriptableObject obj, Action<T> action) where T : BlueprintComponent
        {
            var replacement = CreateCopy(obj.GetComponent<T>());
            action(replacement);
            ReplaceComponent(obj, obj.GetComponent<T>(), replacement);
        }


        public static void MaybeReplaceComponent<T>(this BlueprintScriptableObject obj, Action<T> action) where T : BlueprintComponent
        {
            var replacement = CreateCopy(obj.GetComponent<T>());
            if (replacement == null)
            {
                return;
            }
            action(replacement);
            ReplaceComponent(obj, obj.GetComponent<T>(), replacement);
        }


        public static void ReplaceComponent(this BlueprintScriptableObject obj, BlueprintComponent original, BlueprintComponent replacement)
        {
            // Note: make a copy so we don't mutate the original component
            // (in case it's a clone of a game one).
            var components = obj.ComponentsArray;
            var newComponents = new BlueprintComponent[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                var c = components[i];
                newComponents[i] = c == original ? replacement : c;
            }
            obj.SetComponents(newComponents); // fix up names if needed
        }

        public static void AddFeats( params BlueprintFeature[] feats)
        {
            AddFeats(basicFeatSelection, feats);
        }

        public static void AddFeats( String featSelectionId, params BlueprintFeature[] feats)
        {
            var featGroup = Resources.GetBlueprint<BlueprintFeatureSelection>(featSelectionId);
            var allFeats = featGroup.AllFeatures.ToList();
            allFeats.AddRange(feats);
            featGroup.SetFeatures(allFeats);
        }

        public static void SetFeatures(this BlueprintFeatureSelection selection, IEnumerable<BlueprintFeature> features)
        {
            SetFeatures(selection, features.ToArray());
        }

        public static void SetFeatures(this BlueprintFeatureSelection selection, params BlueprintFeature[] features)
        {
            foreach (BlueprintFeature f in features)
            {
                selection.m_AllFeatures = selection.m_AllFeatures.AddToArray(f.ToReference<BlueprintFeatureReference>());
            }
        }
        public static PrerequisiteNoArchetype prerequisiteNoArchetype(BlueprintCharacterClass character_class, BlueprintArchetype archetype, bool any = false)
        {
            var p = Helpers.Create<PrerequisiteNoArchetype>();
            p.m_Archetype = archetype.ToReference<BlueprintArchetypeReference>();
            p.m_CharacterClass = character_class.ToReference<BlueprintCharacterClassReference>();
            p.Group = any ? Prerequisite.GroupType.Any : Prerequisite.GroupType.All;
            return p;
        }


        public static PrerequisiteNoArchetype prerequisiteNoArchetype(BlueprintArchetype archetype, bool any = false)
        {
            var p = Helpers.Create<PrerequisiteNoArchetype>();
            p.m_Archetype = archetype.ToReference<BlueprintArchetypeReference>();
            p.m_CharacterClass = archetype.GetParentClass().ToReference<BlueprintCharacterClassReference>();
            p.Group = any ? Prerequisite.GroupType.Any : Prerequisite.GroupType.All;
            return p;
        }

    }

    public delegate ref S FastRef<T, S>(T source = default);
    public delegate void FastSetter<T, S>(T source, S value);
    public delegate S FastGetter<T, S>(T source);
    public delegate object FastInvoke(object target, params object[] paramters);


}
