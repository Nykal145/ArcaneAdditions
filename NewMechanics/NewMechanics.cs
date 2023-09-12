using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Parts;
using UnityEngine;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.Visual.HitSystem;
using System;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Controllers.Units;
using Kingmaker.Designers;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Enums;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Utility;
using Kingmaker.UI.GenericSlot;
using Kingmaker.Items;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.EntitySystem.Entities;
using System.Collections.Generic;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.ElementsSystem;
using Kingmaker.Controllers;
using Kingmaker;
using static Kingmaker.UnitLogic.Abilities.Components.AbilityCustomMeleeAttack;
using Kingmaker.UnitLogic.Mechanics.ContextData;
using Kingmaker.Controllers.Projectiles;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.EntitySystem;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.UnitLogic.ActivatableAbilities.Restrictions;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.EntitySystem.Persistence.Versioning;
using JetBrains.Annotations;
using Kingmaker.Enums.Damage;
using Kingmaker.Inspect;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Designers.EventConditionActionSystem.ContextData;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.Visual.Animation.Kingmaker;
using Kingmaker.Blueprints.Area;
using Kingmaker.Items.Slots;
using Kingmaker.Blueprints.Items.Components;
using Kingmaker.Controllers.Combat;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using System.Text;
using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.AreaLogic.SummonPool;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UI.LevelUp;

namespace ArcanistTweaks
{
    namespace NewMechanics
    {


        public class ContextCalculateAbilityParamsBasedOnClasses : ContextAbilityParamsCalculator
        {
            public bool use_kineticist_main_stat;
            public StatType StatType = StatType.Charisma;
            public BlueprintCharacterClass[] CharacterClasses = new BlueprintCharacterClass[0];
            public BlueprintArchetype[] archetypes = new BlueprintArchetype[0];
            public BlueprintUnitProperty property = null;

            public override AbilityParams Calculate(MechanicsContext context)
            {
                UnitEntityData maybeCaster = context?.MaybeCaster;
                if (maybeCaster == null)
                {
                    return context?.Params;
                }
                StatType statType = this.StatType;
                if (this.use_kineticist_main_stat)
                {
                    UnitPartKineticist unitPartKineticist = context.MaybeCaster?.Get<UnitPartKineticist>();
                    StatType? mainStatType = unitPartKineticist?.MainStatType;
                    statType = !mainStatType.HasValue ? this.StatType : mainStatType.Value;
                }

                var stat_property_getter = property?.GetComponent<StatPropertyValueGetter>();
                if (stat_property_getter != null)
                {
                    statType = stat_property_getter.GetStat(maybeCaster);
                }

                AbilityData ability = context.SourceAbilityContext?.Ability;
                RuleCalculateAbilityParams rule = !(ability != (AbilityData)null) ? new RuleCalculateAbilityParams(maybeCaster, context.AssociatedBlueprint, (Spellbook)null) : new RuleCalculateAbilityParams(maybeCaster, ability);
                rule.ReplaceStat = new StatType?(statType);

                int class_level = 0;
                foreach (var c in this.CharacterClasses)
                {
                    var class_archetypes = archetypes.Where(a => a.GetParentClass() == c);

                    if (class_archetypes.Empty() || class_archetypes.Any(a => maybeCaster.Descriptor.Progression.IsArchetype(a)))
                    {
                        class_level += maybeCaster.Descriptor.Progression.GetClassLevel(c);
                    }

                }
                rule.ReplaceCasterLevel = new int?(class_level);
                rule.ReplaceSpellLevel = new int?(class_level / 2);
                return context.TriggerRule<RuleCalculateAbilityParams>(rule).Result;
            }
        }


        class StatPropertyValueGetter : PropertyValueGetter
        {
            public override int GetBaseValue(UnitEntityData unit)
            {
                return 0;
            }

            public virtual StatType GetStat(UnitEntityData unit)
            {
                return StatType.Charisma;
            }
        }

    }
}
