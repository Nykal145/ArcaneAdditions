using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.EntitySystem.Entities;
using System.Collections.Generic;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;


namespace ArcanistTweaks.NewComponents
{
    [AllowMultipleComponents]
    [AllowedOn(typeof(BlueprintFeatureSelection))]
    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowedOn(typeof(BlueprintUnit))]

    [TypeId("d3e58478e2e946e6bfc6c3cdad224bf9")]
    class IncreaseAttribute : UnitFactComponentDelegate,
        IUnitGainFactHandler,
        IUnitLevelUpHandler
        
    {

        public StatType type;
        public int amount;

        public int timesApplied;

        public IncreaseAttribute()
        {
            type = new StatType();
            amount = 0;
            timesApplied = 0;
        }

        public IncreaseAttribute(StatType Type, int Amount)
        {
            type = Type;
            amount = Amount;
        }

        public void HandleUnitGainFact(EntityFact fact)
        {   
            if (fact == Fact)
            {
                for (int i = 0; i < Owner.Stats.Attributes.Count; i++)
                {
                    if (Owner.Stats.Attributes[i].Type == type)
                    {
                        Owner.Stats.Attributes[i].m_BaseValue += amount;
                        Owner.Stats.Attributes[i].UpdateValue();
                        timesApplied = 1;
                    }
                }
            }
        }

        public void HandleUnitAfterLevelUp(UnitEntityData unit, LevelUpController controller)
        {
            if (Fact.GetRank() > timesApplied)
            {
                for (int i = 0; i < unit.Stats.Attributes.Count; i++)
                {
                    if (unit.Stats.Attributes[i].Type == type)
                    {
                        unit.Stats.Attributes[i].m_BaseValue += amount;
                        unit.Stats.Attributes[i].UpdateValue();
                        timesApplied += 1;
                    }
                }
            }
        }
        public void HandleUnitBeforeLevelUp(UnitEntityData unit)
        {

        }


    }
}
