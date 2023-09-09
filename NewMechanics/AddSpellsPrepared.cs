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

using System;

namespace ArcanistTweaks.NewComponents
{
    [AllowMultipleComponents]
    [AllowedOn(typeof(BlueprintFeatureSelection))]
    [TypeId("f67b4681d113454eb87f3641f8c7597c")]
    class AddSpellsPrepared : UnitFactComponentDelegate,
        IUnitGainFactHandler
    {

        public StatType type;
        public int amount;

        public int timesApplied;

        public AddSpellsPrepared()
        {
            type = new StatType();
            amount = 0;
            timesApplied = 0;
        }

        public AddSpellsPrepared(StatType Type, int Amount)
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
    }
}
