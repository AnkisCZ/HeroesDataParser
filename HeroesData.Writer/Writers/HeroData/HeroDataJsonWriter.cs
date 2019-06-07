﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using HeroesData.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.FileWriter.Writers.HeroData
{
    internal class HeroDataJsonWriter : HeroDataWriter<JProperty, JObject, Hero>
    {
        public HeroDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Hero hero)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(hero);

            JObject heroObject = new JObject();

            if (!string.IsNullOrEmpty(hero.Name) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("name", hero.Name);
            if (!string.IsNullOrEmpty(hero.CUnitId) && hero.CHeroId != StormHero.CHeroId)
                heroObject.Add("unitId", hero.CUnitId);
            if (!string.IsNullOrEmpty(hero.HyperlinkId) && hero.CHeroId != StormHero.CHeroId)
                heroObject.Add("hyperlinkId", hero.HyperlinkId);
            if (!string.IsNullOrEmpty(hero.AttributeId))
                heroObject.Add("attributeId", hero.AttributeId);

            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(hero.Difficulty))
                heroObject.Add("difficulty", hero.Difficulty);

            if (hero.CHeroId != StormHero.CHeroId)
                heroObject.Add("franchise", hero.Franchise.ToString());

            if (hero.Gender.HasValue)
                heroObject.Add("gender", hero.Gender.Value.ToString());
            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(hero.Title))
                heroObject.Add("title", hero.Title);
            if (hero.InnerRadius > 0)
                heroObject.Add("innerRadius", hero.InnerRadius);
            if (hero.Radius > 0)
                heroObject.Add("radius", hero.Radius);
            if (hero.ReleaseDate.HasValue)
                heroObject.Add("releaseDate", hero.ReleaseDate.Value.ToString("yyyy-MM-dd"));
            if (hero.Sight > 0)
                heroObject.Add("sight", hero.Sight);
            if (hero.Speed > 0)
                heroObject.Add("speed", hero.Speed);
            if (!string.IsNullOrEmpty(hero.Type) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("type", hero.Type);
            if (hero.Rarity.HasValue)
                heroObject.Add("rarity", hero.Rarity.Value.ToString());
            if (!string.IsNullOrEmpty(hero.ScalingBehaviorLink))
                heroObject.Add(new JProperty("scalingLinkId", hero.ScalingBehaviorLink));
            if (!FileOutputOptions.IsLocalizedText && !string.IsNullOrEmpty(hero.SearchText))
                heroObject.Add("searchText", hero.SearchText);
            if (!string.IsNullOrEmpty(hero.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add("description", GetTooltip(hero.Description, FileOutputOptions.DescriptionType));
            if (hero.HeroDescriptorsCount > 0)
                heroObject.Add(new JProperty("descriptors", hero.HeroDescriptors.OrderBy(x => x)));
            if (hero.UnitIdsCount > 0)
                heroObject.Add(new JProperty("units", hero.UnitIds.OrderBy(x => x)));

            JProperty portraits = HeroPortraits(hero);
            if (portraits != null)
                heroObject.Add(portraits);

            JProperty life = UnitLife(hero);
            if (life != null)
                heroObject.Add(life);

            JProperty energy = UnitEnergy(hero);
            if (energy != null)
                heroObject.Add(energy);

            JProperty armor = UnitArmor(hero);
            if (armor != null)
                heroObject.Add(armor);

            if (hero.RolesCount > 0 && !FileOutputOptions.IsLocalizedText)
                heroObject.Add(new JProperty("roles", hero.Roles));

            if (!string.IsNullOrEmpty(hero.ExpandedRole) && !FileOutputOptions.IsLocalizedText)
                heroObject.Add(new JProperty("expandedRole", hero.ExpandedRole));

            JProperty ratings = HeroRatings(hero);
            if (ratings != null)
                heroObject.Add(ratings);

            JProperty weapons = UnitWeapons(hero);
            if (weapons != null)
                heroObject.Add(weapons);

            JProperty abilities = UnitAbilities(hero, false);
            if (abilities != null)
                heroObject.Add(abilities);

            JProperty subAbilities = UnitSubAbilities(hero);
            if (subAbilities != null)
                heroObject.Add(subAbilities);

            JProperty talents = HeroTalents(hero);
            if (talents != null)
                heroObject.Add(talents);

            return new JProperty(hero.Id, heroObject);
        }

        protected override JProperty GetArmorObject(Unit unit)
        {
            JObject armorObject = new JObject();

            foreach (UnitArmor armor in unit.Armor)
            {
                armorObject.Add(new JProperty(
                    armor.Type.ToLower(),
                    new JObject(
                        new JProperty("basic", armor.BasicArmor),
                        new JProperty("ability", armor.AbilityArmor),
                        new JProperty("splash", armor.SplashArmor))));
            }

            return new JProperty("armor", armorObject);
        }

        protected override JProperty GetLifeObject(Unit unit)
        {
            return new JProperty(
                "life",
                new JObject(
                    new JProperty("amount", unit.Life.LifeMax),
                    new JProperty("scale", unit.Life.LifeScaling),
                    new JProperty("regenRate", unit.Life.LifeRegenerationRate),
                    new JProperty("regenScale", unit.Life.LifeRegenerationRateScaling)));
        }

        protected override JProperty GetEnergyObject(Unit unit)
        {
            JObject energyObject = new JObject
            {
                new JProperty("amount", unit.Energy.EnergyMax),
            };

            if (!string.IsNullOrEmpty(unit.Energy.EnergyType))
                energyObject.Add(new JProperty("type", unit.Energy.EnergyType));

            energyObject.Add(new JProperty("regenRate", unit.Energy.EnergyRegenerationRate));

            return new JProperty("energy", energyObject);
        }

        protected override JProperty GetAbilitiesObject(Unit unit, bool isSubAbilities)
        {
            JObject abilityObject = new JObject();

            if (isSubAbilities)
            {
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Basic), "basic");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Heroic), "heroic");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Trait), "trait");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Mount), "mount");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Activable), "activable");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Hearth), "hearth");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Taunt), "taunt");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Dance), "dance");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Spray), "spray");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Voice), "voice");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.MapMechanic), "mapMechanic");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Interact), "interact");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Action), "action");
                SetAbilities(abilityObject, unit.SubAbilities(AbilityTier.Unknown), "unknown");
            }
            else
            {
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Basic), "basic");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Heroic), "heroic");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Trait), "trait");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Mount), "mount");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Activable), "activable");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Hearth), "hearth");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Taunt), "taunt");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Dance), "dance");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Spray), "spray");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Voice), "voice");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.MapMechanic), "mapMechanic");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Interact), "interact");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Action), "action");
                SetAbilities(abilityObject, unit.PrimaryAbilities(AbilityTier.Unknown), "unknown");
            }

            return new JProperty("abilities", abilityObject);
        }

        protected override JProperty GetAbilityTalentChargesObject(TooltipCharges tooltipCharges)
        {
            JObject charges = new JObject
            {
                { "countMax", tooltipCharges.CountMax },
            };

            if (tooltipCharges.CountUse.HasValue)
                charges.Add("countUse", tooltipCharges.CountUse.Value);

            if (tooltipCharges.CountStart.HasValue)
                charges.Add("countStart", tooltipCharges.CountStart.Value);

            if (tooltipCharges.IsHideCount.HasValue)
                charges.Add("hideCount", tooltipCharges.IsHideCount.Value);

            if (tooltipCharges.RecastCooldown.HasValue)
                charges.Add("recastCooldown", tooltipCharges.RecastCooldown.Value);

            return new JProperty("charges", charges);
        }

        protected override JProperty GetAbilityTalentCooldownObject(TooltipCooldown tooltipCooldown)
        {
            return new JProperty("cooldownTooltip", GetTooltip(tooltipCooldown.CooldownTooltip, FileOutputOptions.DescriptionType));
        }

        protected override JProperty GetAbilityTalentEnergyCostObject(TooltipEnergy tooltipEnergy)
        {
            return new JProperty("energyTooltip", GetTooltip(tooltipEnergy.EnergyTooltip, FileOutputOptions.DescriptionType));
        }

        protected override JProperty GetAbilityTalentLifeCostObject(TooltipLife tooltipLife)
        {
            return new JProperty("lifeTooltip", GetTooltip(tooltipLife.LifeCostTooltip, FileOutputOptions.DescriptionType));
        }

        protected override JObject AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(abilityTalentBase);

            JObject info = new JObject
            {
                { "nameId", abilityTalentBase.ReferenceNameId },
            };

            if (!string.IsNullOrEmpty(abilityTalentBase.Name) && !FileOutputOptions.IsLocalizedText)
                info.Add("name", abilityTalentBase.Name);

            if (!string.IsNullOrEmpty(abilityTalentBase.ShortTooltipNameId))
                info.Add("shortTooltipId", abilityTalentBase.ShortTooltipNameId);

            if (!string.IsNullOrEmpty(abilityTalentBase.FullTooltipNameId))
                info.Add("fullTooltipId", abilityTalentBase.FullTooltipNameId);

            if (!string.IsNullOrEmpty(abilityTalentBase.IconFileName))
                info.Add("icon", Path.ChangeExtension(abilityTalentBase.IconFileName?.ToLower(), ".png"));

            if (abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.HasValue)
                info.Add("toggleCooldown", abilityTalentBase.Tooltip.Cooldown.ToggleCooldown.Value);

            JProperty life = UnitAbilityTalentLifeCost(abilityTalentBase.Tooltip.Life);
            if (life != null)
                info.Add(life);

            JProperty energy = UnitAbilityTalentEnergyCost(abilityTalentBase.Tooltip.Energy);
            if (energy != null)
                info.Add(energy);

            JProperty charges = UnitAbilityTalentCharges(abilityTalentBase.Tooltip.Charges);
            if (charges != null)
                info.Add(charges);

            JProperty cooldown = UnitAbilityTalentCooldown(abilityTalentBase.Tooltip.Cooldown);
            if (cooldown != null)
                info.Add(cooldown);

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip.ShortTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                info.Add("shortTooltip", GetTooltip(abilityTalentBase.Tooltip.ShortTooltip, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(abilityTalentBase.Tooltip.FullTooltip?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                info.Add("fullTooltip", GetTooltip(abilityTalentBase.Tooltip.FullTooltip, FileOutputOptions.DescriptionType));

            info.Add("abilityType", abilityTalentBase.AbilityType.ToString());

            if (abilityTalentBase.IsActive)
                info.Add("isActive", abilityTalentBase.IsActive);

            if (abilityTalentBase.IsPassive)
                info.Add("isPassive", abilityTalentBase.IsPassive);

            if (abilityTalentBase.IsQuest)
                info.Add("isQuest", abilityTalentBase.IsQuest);

            return info;
        }

        protected override JProperty GetSubAbilitiesObject(ILookup<string, Ability> linkedAbilities)
        {
            JObject parentLinkObject = new JObject();

            IEnumerable<string> parentLinks = linkedAbilities.Select(x => x.Key);
            foreach (string parent in parentLinks)
            {
                JObject abilities = new JObject();

                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Basic), "basic");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Heroic), "heroic");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Trait), "trait");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Mount), "mount");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Activable), "activable");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Hearth), "hearth");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Taunt), "taunt");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Dance), "dance");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Spray), "spray");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Voice), "voice");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.MapMechanic), "mapMechanic");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Interact), "interact");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Action), "action");
                SetAbilities(abilities, linkedAbilities[parent].Where(x => x.Tier == AbilityTier.Unknown), "unknown");

                parentLinkObject.Add(new JProperty(parent, abilities));
            }

            return new JProperty("subAbilities", new JArray(new JObject(parentLinkObject)));
        }

        protected override JProperty GetRatingsObject(Hero hero)
        {
            return new JProperty(
                "ratings",
                new JObject(
                    new JProperty("complexity", hero.Ratings.Complexity),
                    new JProperty("damage", hero.Ratings.Damage),
                    new JProperty("survivability", hero.Ratings.Survivability),
                    new JProperty("utility", hero.Ratings.Utility)));
        }

        protected override JObject AbilityInfoElement(Ability ability)
        {
            JObject jObject = AbilityTalentInfoElement(ability);

            return jObject;
        }

        protected override JObject TalentInfoElement(Talent talent)
        {
            JObject jObject = AbilityTalentInfoElement(talent);
            jObject.Add(new JProperty("sort", talent.Column));

            if (talent.AbilityTalentLinkIdsCount > 0)
                jObject.Add(new JProperty("abilityTalentLinkIds", talent.AbilityTalentLinkIds));

            return jObject;
        }

        protected override JProperty GetTalentsObject(Hero hero)
        {
            JObject talantObject = new JObject();

            SetTalents(talantObject, hero.TierTalents(TalentTier.Level1), "level1");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level4), "level4");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level7), "level7");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level10), "level10");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level13), "level13");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level16), "level16");
            SetTalents(talantObject, hero.TierTalents(TalentTier.Level20), "level20");

            return new JProperty("talents", talantObject);
        }

        protected override JProperty GetWeaponsObject(Unit unit)
        {
            JArray weaponArray = new JArray();

            foreach (UnitWeapon weapon in unit.Weapons)
            {
                JObject weaponObject = new JObject
                {
                    new JProperty("nameId", weapon.WeaponNameId),
                    new JProperty("range", weapon.Range),
                    new JProperty("period", weapon.Period),
                    new JProperty("damage", weapon.Damage),
                    new JProperty("damageScale", weapon.DamageScaling),
                };

                if (weapon.AttributeFactors.Any())
                {
                    JObject attributeFactorOjbect = new JObject();

                    foreach (WeaponAttributeFactor item in weapon.AttributeFactors)
                    {
                        attributeFactorOjbect.Add(item.Type.ToLower(), item.Value);
                    }

                    weaponObject.Add("damageFactor", attributeFactorOjbect);
                }

                weaponArray.Add(weaponObject);
            }

            return new JProperty("weapons", weaponArray);
        }

        protected override JProperty GetPortraitObject(Hero hero)
        {
            JObject portrait = new JObject();

            if (!string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName))
                portrait.Add("heroSelect", Path.ChangeExtension(hero.HeroPortrait.HeroSelectPortraitFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName))
                portrait.Add("leaderboard", Path.ChangeExtension(hero.HeroPortrait.LeaderboardPortraitFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName))
                portrait.Add("loading", Path.ChangeExtension(hero.HeroPortrait.LoadingScreenPortraitFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName))
                portrait.Add("partyPanel", Path.ChangeExtension(hero.HeroPortrait.PartyPanelPortraitFileName?.ToLower(), StaticImageExtension));
            if (!string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName))
                portrait.Add("target", Path.ChangeExtension(hero.HeroPortrait.TargetPortraitFileName?.ToLower(), StaticImageExtension));

            return new JProperty("portraits", portrait);
        }

        protected void SetAbilities(JObject abilityObject, IEnumerable<Ability> abilities, string propertyName)
        {
            if (abilities.Any())
            {
                abilityObject.Add(new JProperty(
                    propertyName,
                    new JArray(
                        from ability in abilities
                        orderby ability.AbilityType ascending
                        select new JObject(AbilityInfoElement(ability)))));
            }
        }

        protected void SetTalents(JObject talentObject, IEnumerable<Talent> talents, string propertyName)
        {
            if (talents.Any())
            {
                talentObject.Add(new JProperty(
                    propertyName,
                    new JArray(
                        from talent in talents
                        orderby talent.Column ascending
                        select new JObject(TalentInfoElement(talent)))));
            }
        }
    }
}
