using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UI.Common;

namespace ArcanistTweaks
{
    static class Feats
    {

        public static void add()
        {
            var BullsStrenth = Resources.GetBlueprint<BlueprintAbility>("4c3d08935262b6544ae97599b3a9556d");
            var ASIStrFeature = Helpers.Create<BlueprintFeature>(bp =>
            {
                bp.AssetGuid = new BlueprintGuid(new Guid("1ce730c28e5949069405d77d5024c991"));
                bp.IsClassFeature = false;
                bp.Groups = new FeatureGroup[] { FeatureGroup.None };
                bp.Ranks = 5;
                bp.name = "StrengthASI";
                bp.m_Icon = BullsStrenth.Icon;
                bp.m_DisplayName = Helpers.CreateString($"{bp.name}.Name", "Strength Increase");
                bp.m_Description = Helpers.CreateString($"{bp.name}.Description", "Increase your strength score by 2");
                bp.ReapplyOnLevelUp = false;
                bp.AddComponent(Helpers.Create<NewComponents.IncreaseAttribute>(a =>
                {
                    a.type = StatType.Strength;
                    a.amount = Main.Settings.MythicASIAmount;
                    a.timesApplied = 0;
                }));
            });
            Resources.AddBlueprint(ASIStrFeature);

            var CatsGrace = Resources.GetBlueprint<BlueprintAbility>("de7a025d48ad5da4991e7d3c682cf69d");
            var ASIDexFeature = Helpers.Create<BlueprintFeature>(bp =>
            {
                bp.AssetGuid = new BlueprintGuid(new Guid("231ccd7833784d0d8763ed91fbaa061e"));
                bp.IsClassFeature = false;
                bp.Groups = new FeatureGroup[] { FeatureGroup.None };
                bp.Ranks = 5;
                bp.name = "DexterityASI";
                bp.m_Icon = CatsGrace.Icon;
                bp.m_DisplayName = Helpers.CreateString($"{bp.name}.Name", "Dexterity Increase");
                bp.m_Description = Helpers.CreateString($"{bp.name}.Description", "Increase your dexterity score by 2");
                bp.ReapplyOnLevelUp = false;
                bp.AddComponent(Helpers.Create<NewComponents.IncreaseAttribute>(a =>
                {
                    a.type = StatType.Dexterity;
                    a.amount = Main.Settings.MythicASIAmount;
                    a.timesApplied = 0;
                }));
            });
            Resources.AddBlueprint(ASIDexFeature);

            var BearsEndurance = Resources.GetBlueprint<BlueprintAbility>("a900628aea19aa74aad0ece0e65d091a");
            var ASIConFeature = Helpers.Create<BlueprintFeature>(bp =>
            {
                bp.AssetGuid = new BlueprintGuid(new Guid("72d93b63a0fc4ac880ebdf7c30442e21"));
                bp.IsClassFeature = false;
                bp.Groups = new FeatureGroup[] { FeatureGroup.None };
                bp.Ranks = 5;
                bp.name = "ConstitutionASI";
                bp.m_Icon = BearsEndurance.Icon;
                bp.m_DisplayName = Helpers.CreateString($"{bp.name}.Name", "Constitution Increase");
                bp.m_Description = Helpers.CreateString($"{bp.name}.Description", "Increase your Constitution score by 2");
                bp.ReapplyOnLevelUp = false;
                bp.AddComponent(Helpers.Create<NewComponents.IncreaseAttribute>(a =>
                {
                    a.type = StatType.Constitution;
                    a.amount = Main.Settings.MythicASIAmount;
                    a.timesApplied = 0;
                }));
            });
            Resources.AddBlueprint(ASIConFeature);

            var FoxsCunning = Resources.GetBlueprint<BlueprintAbility>("ae4d3ad6a8fda1542acf2e9bbc13d113");
            var ASIIntFeature = Helpers.Create<BlueprintFeature>(bp =>
            {
                bp.AssetGuid = new BlueprintGuid(new Guid("05d3df4cec234c5089ae31cd86b1ba1a"));
                bp.IsClassFeature = false;
                bp.Groups = new FeatureGroup[] { FeatureGroup.None };
                bp.Ranks = 5;
                bp.name = "IntelligenceASI";
                bp.m_Icon = FoxsCunning.Icon;
                bp.m_DisplayName = Helpers.CreateString($"{bp.name}.Name", "Intelligence Increase");
                bp.m_Description = Helpers.CreateString($"{bp.name}.Description", "Increase your Intelligence score by 2");
                bp.ReapplyOnLevelUp = false;
                bp.AddComponent(Helpers.Create<NewComponents.IncreaseAttribute>(a =>
                {
                    a.type = StatType.Intelligence;
                    a.amount = Main.Settings.MythicASIAmount;
                    a.timesApplied = 0;
                }));
            });
            Resources.AddBlueprint(ASIIntFeature);

            var OwlsWisdom = Resources.GetBlueprint<BlueprintAbility>("f0455c9295b53904f9e02fc571dd2ce1");
            var ASIWisFeature = Helpers.Create<BlueprintFeature>(bp =>
            {
                bp.AssetGuid = new BlueprintGuid(new Guid("638fe833c0524192b8247aa239c654e2"));
                bp.IsClassFeature = false;
                bp.Groups = new FeatureGroup[] { FeatureGroup.None };
                bp.Ranks = 5;
                bp.name = "WisdomASI";
                bp.m_Icon = OwlsWisdom.Icon;
                bp.m_DisplayName = Helpers.CreateString($"{bp.name}.Name", "Wisdom Increase");
                bp.m_Description = Helpers.CreateString($"{bp.name}.Description", "Increase your Wisdom score by 2");
                bp.ReapplyOnLevelUp = false;
                bp.AddComponent(Helpers.Create<NewComponents.IncreaseAttribute>(a =>
                {
                    a.type = StatType.Wisdom;
                    a.amount = Main.Settings.MythicASIAmount;
                    a.timesApplied = 0;
                }));
            });
            Resources.AddBlueprint(ASIWisFeature);

            var EaglesSplendor = Resources.GetBlueprint<BlueprintAbility>("446f7bf201dc1934f96ac0a26e324803");
            var ASIChaFeature = Helpers.Create<BlueprintFeature>(bp =>
            {
                bp.AssetGuid = new BlueprintGuid(new Guid("9e85910ac8684e8b960f8fa3c8fd170f"));
                bp.IsClassFeature = false;
                bp.Groups = new FeatureGroup[] { FeatureGroup.None };
                bp.Ranks = 5;
                bp.name = "CharismaASI";
                bp.m_Icon = EaglesSplendor.Icon;
                bp.m_DisplayName = Helpers.CreateString($"{bp.name}.Name", "Charisma Increase");
                bp.m_Description = Helpers.CreateString($"{bp.name}.Description", "Increase your Charisma score by 2");
                bp.ReapplyOnLevelUp = false;
                bp.AddComponent(Helpers.Create<NewComponents.IncreaseAttribute>(a =>
                {
                    a.type = StatType.Charisma;
                    a.amount = Main.Settings.MythicASIAmount;
                    a.timesApplied = 0;
                }));
            });
            Resources.AddBlueprint(ASIChaFeature);

            var lstFeatures = new List<BlueprintFeatureReference>();
            lstFeatures.Add(ASIStrFeature.ToReference<BlueprintFeatureReference>());
            lstFeatures.Add(ASIDexFeature.ToReference<BlueprintFeatureReference>());
            lstFeatures.Add(ASIConFeature.ToReference<BlueprintFeatureReference>());
            lstFeatures.Add(ASIIntFeature.ToReference<BlueprintFeatureReference>());
            lstFeatures.Add(ASIWisFeature.ToReference<BlueprintFeatureReference>());
            lstFeatures.Add(ASIChaFeature.ToReference<BlueprintFeatureReference>());

            var MythicAbilitySelection = Resources.GetBlueprint<BlueprintFeatureSelection>("ba0e5a900b775be4a99702f1ed08914d");

            var MythicASISelection = Helpers.Create<BlueprintFeatureSelection>(bp =>
            {
                bp.AssetGuid = new BlueprintGuid(new Guid("50f26cf41da0434fb07c55453edb88b9"));
                bp.name = "MythicASISelection";
                bp.m_Icon = MythicAbilitySelection.m_Icon;
                bp.m_DisplayName = Helpers.CreateString($"{bp.name}.Name", "Ability Score");
                bp.m_Description = Helpers.CreateString($"{bp.name}.Description", "Upon reaching the 2nd mythic tier, an ability score of your choice permanently increases by 2. At 4th, 6th, 8th, and 10th tiers, another ability score of your choice permanently increases by 2; this can be an ability score you’ve already increased or a different ability score. ");
                bp.m_AllFeatures = lstFeatures.ToArray();
            });
            Resources.AddBlueprint(MythicASISelection);

        }
    }
}
