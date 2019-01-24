﻿using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.HeroData.Overrides
{
    public class AbilityOverride : PropertyOverrideBase<Ability>
    {
        public AbilityOverride(GameData gameData)
            : base(gameData)
        {
        }

        public AbilityOverride(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<Ability>> propertyOverrides)
        {
            if (propertyName == nameof(Ability.ParentLink))
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    ability.ParentLink = propertyValue;
                });
            }
            else if (propertyName == "AbilityTier")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (Enum.TryParse(propertyValue, out AbilityTier abilityTier))
                        ability.Tier = abilityTier;
                    else
                        ability.Tier = AbilityTier.Basic;
                });
            }
            else if (propertyName == "AbilityType")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    if (Enum.TryParse(propertyValue, out AbilityType abilityType))
                        ability.AbilityType = abilityType;
                    else
                        ability.AbilityType = AbilityType.Q;
                });
            }
            else if (propertyName == "CooldownTooltip")
            {
                propertyOverrides.Add(propertyName, (ability) =>
                {
                    ability.Tooltip.Cooldown.CooldownTooltip = new Heroes.Models.TooltipDescription(propertyValue);
                });
            }
        }
    }
}