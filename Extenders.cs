using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ArcanistTweaks
{
    public static partial class Extenders
    {
        public enum ContextRankBaseValueTypeExtender
        {
            None,
            MasterClassLevel, //ClassLevel or Master class level if pet
            MasterMaxClassLevelWithArchetype, //MaxClassLevelWithArchetype or Master class level if pet
            MasterFeatureRank,
            SummClassLevelWithArchetypes,
            MaxClassLevelWithArchetypes,
            ClassLevelPlusStatValue,
        }

        public class ContextRankConfigArchetypeList : BlueprintComponent
        {
            public BlueprintArchetype[] archetypes;
        }

        public static ContextRankBaseValueType ToContextRankBaseValueType(this ContextRankBaseValueTypeExtender base_value)
        {
            int value = EnumUtils.GetMaxValue<ContextRankBaseValueType>() + (int)base_value;
            return (ContextRankBaseValueType)value;
        }

        public static ContextRankBaseValueTypeExtender ToContextRankBaseValueTypeExtender(this ContextRankBaseValueType base_value)
        {
            int value = (int)base_value - (int)ContextRankBaseValueTypeExtender.None.ToContextRankBaseValueType();
            if (value <= 0)
            {
                return 0;
            }
            return (ContextRankBaseValueTypeExtender)(value);
        }

    }
}
