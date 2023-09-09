//based off tabletop tweaks

using JetBrains.Annotations;
using Kingmaker.Blueprints;
using System.Collections.Generic;
using System;

namespace ArcanistTweaks {
    static class Resources {
        public static readonly Dictionary<string, SimpleBlueprint> ModBlueprints = new Dictionary<string, SimpleBlueprint>();
        public static T GetBlueprint<T>(string id) where T : SimpleBlueprint {
            SimpleBlueprint asset = ResourcesLibrary.TryGetBlueprint(new BlueprintGuid(new Guid(id)));
            T value = asset as T;
            if (value == null) { Main.Error($"COULD NOT LOAD: {id} - {typeof(T)}"); }
            return value;
        }
        public static void AddBlueprint([NotNull] BlueprintScriptableObject blueprint) {
            AddBlueprint(blueprint, blueprint.AssetGuid.ToString());
        }
        public static void AddBlueprint([NotNull] SimpleBlueprint blueprint, string assetId) {
            blueprint.AssetGuid = new BlueprintGuid(new Guid(assetId));
            var loadedBlueprint = ResourcesLibrary.TryGetBlueprint(new BlueprintGuid(new Guid(assetId)));
            if (loadedBlueprint == null) {
                ModBlueprints[assetId] = blueprint;
                ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(new BlueprintGuid(new Guid(assetId)), blueprint);
                Main.LogPatch("Added", blueprint);
            } else {
                Main.Log($"Failed to Add: {blueprint.name}");
                Main.Log($"Asset ID: {assetId} already in use by: {loadedBlueprint.name}");
            }
        }
    }
}