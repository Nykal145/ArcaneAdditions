﻿using JetBrains.Annotations;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Root;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.LevelUp;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kingmaker.Blueprints.Area.FactHolder;

namespace ArcanistTweaks.LevelUpMechanics
{
    public class addSelection : UnitFactComponentDelegate, ILevelUpCompleteUIHandler
    {
        [JsonProperty]
        //bool applied;
        public BlueprintFeatureSelection selection;

        public void HandleLevelUpComplete(UnitEntityData unit, bool isChargen)
        {
        }

        public override void OnActivate()
        {
            try
            {
                var levelUp = Game.Instance.LevelUpController;
                if (Owner == levelUp.Preview || Owner == levelUp.Unit)
                {
                    int index = levelUp.State.Selections.Count<FeatureSelectionState>((Func<FeatureSelectionState, bool>)(s => s.Selection == selection));
                    FeatureSelectionState featureSelectionState = new FeatureSelectionState(null, new FeatureSource(), selection, index, 0);
                    levelUp.State.Selections.Add(featureSelectionState);
                }
                //applied = true;
            }
            catch (Exception e)
            {
                Main.Log(e.ToString());
            }
        }
    }


    public interface ILevelUpStartHandler : IGlobalSubscriber
    {
        void HandleLevelUpStart(UnitDescriptor unit);
    }


    //[HarmonyLib.HarmonyPatch(typeof(LevelUpController))]
    //[HarmonyLib.HarmonyPatch("StartPreviewThread", HarmonyLib.MethodType.Normal)]
    //class LevelUpController__StartPreviewThread__Patch
    //{
    //    static bool Prefix(LevelUpController __instance, [CanBeNull] JToken unitJson)
    //    {
    //        EventBus.RaiseEvent<ILevelUpStartHandler>((Action<ILevelUpStartHandler>)(h => h.HandleLevelUpStart(__instance.Unit)));
    //        return true;
    //    }
    //}


    [ComponentName("Add feature on class level range")]
    [AllowMultipleComponents]
    [AllowedOn(typeof(BlueprintUnitFact))]
    public class AddFeatureOnClassLevelRange : UnitFactComponentDelegate, IGlobalSubscriber
    {
        public int min_level = 0;
        public int max_level = 100;
        public BlueprintFeature Feature;
        public BlueprintCharacterClass[] classes;
        public BlueprintArchetype[] archetypes = new BlueprintArchetype[0];
        public int[] class_bonuses = new int[0];
        public BlueprintFeature[] required_facts = new BlueprintFeature[0];
        [JsonProperty]
        private Fact m_AppliedFact;

        public override void OnActivate()
        {
            this.Apply();
        }

        public override void OnDeactivate()
        {
            this.Owner.RemoveFact(this.m_AppliedFact);
        }

        public void HandleUnitGainLevel(UnitDescriptor unit, BlueprintCharacterClass @class)
        {
            this.Apply();
        }

        private void Apply()
        {
            if (this.IsFeatureShouldBeApplied())
            {
                if (this.m_AppliedFact != null)
                    return;
                this.m_AppliedFact = (Fact)Owner.AddFact((BlueprintUnitFact)this.Feature, (MechanicsContext)null, (FeatureParam)null);
            }
            else
            {
                if (this.m_AppliedFact == null)
                    return;
                this.Owner.RemoveFact(this.m_AppliedFact);
                this.m_AppliedFact = (Fact)null;
            }
        }

        private bool IsFeatureShouldBeApplied()
        {
            if (!required_facts.Empty() && !required_facts.Any(f => this.Owner.HasFact(f)))
            {
                return false;
            }

            int class_level = ReplaceCasterLevelOfAbility.CalculateClassLevel(this.classes[0], this.classes.Skip(1).ToArray(), this.Owner, this.archetypes);
            if (!class_bonuses.Empty())
            {
                for (int i = 0; i < class_bonuses.Length; i++)
                {
                    if (this.Owner.Progression.Classes.Any(cd => cd.CharacterClass == classes[i]))
                    {
                        class_level += class_bonuses[i];
                    }
                }
            }

            if (class_level < min_level || class_level > max_level)
            {
                return false;
            }

            return true;
        }

        public override void OnPostLoad()
        {
            base.OnPostLoad();

            bool flag = this.m_AppliedFact != null && !this.Owner.HasFact(this.m_AppliedFact);
            if (flag)
            {
                this.m_AppliedFact.Dispose();
                this.m_AppliedFact = (Fact)null;
            }
            if (!flag || !BlueprintRoot.Instance.PlayerUpgradeActions.AllowedForRestoreFeatures.HasReference(this.Feature.ToReference<BlueprintFeatureBaseReference>()))
                return;
            this.Apply();
        }

        //    ////allow multiple spontaneous spell choices
        //    //[HarmonyLib.HarmonyPatch(typeof(CharBSelectionSwitchSpells))]
        //    //[HarmonyLib.HarmonyPatch("ParseSpellSelection", HarmonyLib.MethodType.Normal)]
        //    //class CharBSelectionSwitchSpells__ParseSpellSelection__Patch
        //    //{
        //    //    static bool Prefix(CharBSelectionSwitchSpells __instance, List<SpellSelectionData> ___m_ShowedSpellsCollections, ref bool ___HasEmptyCollections)
        //    //    {
        //    //        var tr = Harmony12.Traverse.Create(__instance);
        //    //        int num = 0;
        //    //        ___HasEmptyCollections = false;
        //    //        foreach (SpellSelectionData spellsCollection in ___m_ShowedSpellsCollections)
        //    //        {
        //    //            int prev = num;
        //    //            num = tr.Method("TryParseMemorizersCollections", spellsCollection, num).GetValue<int>(); //num = __instance.TryParseMemorizersCollections(spellsCollection, num);
        //    //            if (num == prev)
        //    //            {
        //    //                num = tr.Method("TryParseSpontaneuosCastersCollections", spellsCollection, num).GetValue<int>();//num = this.TryParseSpontaneuosCastersCollections(spellsCollection, num);
        //    //            }
        //    //        }
        //    //        ___HasEmptyCollections = num > 0;
        //    //        __instance.HideSelectionViewsFrom(num);
        //    //        if (__instance.HasSelections)
        //    //        {
        //    //            if (___HasEmptyCollections)
        //    //                tr.Method("ActivateNextEmptyItem").GetValue();    //__instance.ActivateNextEmptyItem();
        //    //            else
        //    //                __instance.ActivateCurrentItem();
        //    //        }
        //    //        else
        //    //            __instance.Hide();
        //    //        return false;
        //    //    }
        //    //}
        }
    }
