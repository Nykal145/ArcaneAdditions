using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Controllers;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.Abilities.Components.TargetCheckers;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.ActivatableAbilities.Restrictions;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using HarmonyLib;

namespace ArcanistTweaks.Archetypes
{
    static class SchoolSavant
    { 
        public static void Create()
        {
            var wizard = Resources.GetBlueprint<BlueprintCharacterClass>("ba34257984f4c41408ce1dc2004e342e");
            var arcanist = Resources.GetBlueprint<BlueprintCharacterClass>("52dbfd8505e22f84fad8d702611f60b7");
            
            var archetype = Helpers.Create<BlueprintArchetype>(a =>
            {
                a.name = "SchoolSavantArchetype";
                a.AssetGuid = new BlueprintGuid(new Guid("ae5189fdb2904133a15e74f9e56f9385"));
                a.LocalizedName = Helpers.CreateString($"{a.name}.Name", "School Savant");
                a.LocalizedDescription = Helpers.CreateString($"{a.name}.Description", "Some arcanists specialize in a school of magic and trade flexibility for focus. School savants are able to prepare more spells per day than typical arcanists, but their selection is more limited.");
            });
            Helpers.SetField(archetype, "m_ParentClass", arcanist);

            var school_focus = Resources.GetBlueprint<BlueprintFeatureSelection>("5f838049069f1ac4d804ce0862ab5110");
            school_focus.m_DisplayName = Helpers.CreateString($"{school_focus.name}.Name", "School Focus");
            school_focus.m_Description = Helpers.CreateString($"{school_focus.name}", "At 1st level, a school savant chooses a school of magic. The arcanist gains the abilities granted by that school, as the arcane school class feature of the wizard, treating her arcanist level as her wizard level for these abilities. She can also further specialize by selecting a subschool. In addition, the arcanist can prepare one additional spell per day of each level she can cast, but this spell must be chosen from the selected school.\n"
                                            + "Finally, the arcanist must select two additional schools of magic as her opposition schools. Whenever she prepares spells from one of her opposition schools, the spell takes up two of her prepared spell slots. ");
            school_focus.AssetGuid = new BlueprintGuid(new Guid("194e6d9b7d5049e8ae202a28c41f3c1e"));
                
            archetype.RemoveFeatures = new LevelEntry[] { Helpers.LevelEntry(1, Resources.GetBlueprint<BlueprintFeatureSelection>("b8bf3d5023f2d8c428fdf6438cecaea7")),
                                                          Helpers.LevelEntry(3, Resources.GetBlueprint<BlueprintFeatureSelection>("b8bf3d5023f2d8c428fdf6438cecaea7")),
                                                          Helpers.LevelEntry(7, Resources.GetBlueprint<BlueprintFeatureSelection>("b8bf3d5023f2d8c428fdf6438cecaea7")),
                                                        };

            

            archetype.AddFeatures = new LevelEntry[] { Helpers.LevelEntry(1, school_focus) };
            arcanist.m_Archetypes = arcanist.m_Archetypes.AddToArray(archetype.ToReference<BlueprintArchetypeReference>()).ToArray();
            arcanist.Progression.m_UIDeterminatorsGroup = arcanist.Progression.m_UIDeterminatorsGroup.AddToArray(school_focus.ToReference<BlueprintFeatureBaseReference>()).ToArray();

            archetype.AddComponent(Helpers.Create<PrerequisiteNoClassLevel>(p => p.m_CharacterClass = wizard.ToReference<BlueprintCharacterClassReference>()));
            arcanist.Progression.m_UIDeterminatorsGroup = arcanist.Progression.m_UIDeterminatorsGroup.AddToArray(school_focus.ToReference<BlueprintFeatureBaseReference>()) ;
            wizard.AddComponent(Helpers.prerequisiteNoArchetype(archetype));
            BlueprintArchetype[] arrArchetype = { archetype };
            ClassToProgression.addClassToDomains(arcanist, arrArchetype, ClassToProgression.DomainSpellsType.SpecialList, school_focus, wizard);

            Resources.AddBlueprint(archetype);
            Resources.AddBlueprint(school_focus);
        }
    }
}

