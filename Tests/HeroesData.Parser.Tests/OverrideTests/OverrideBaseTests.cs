﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.UnitData.Overrides;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests
{
    public abstract class OverrideBaseTests
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");
        private readonly string HeroOverrideTestFolder = "HeroOverrideTest.xml";

        public OverrideBaseTests()
        {
            GameData gameData = GameData.Load(ModsTestFolder);
            OverrideData = OverrideData.Load(gameData, HeroOverrideTestFolder);

            HeroOverride = OverrideData.HeroOverride(CHeroId);
            LoadInitialValues();
        }

        protected abstract string CHeroId { get; }
        protected OverrideData OverrideData { get; }
        protected HeroOverride HeroOverride { get; }
        protected Ability TestAbility { get; } = new Ability();
        protected Talent TestTalent { get; } = new Talent();
        protected UnitWeapon TestWeapon { get; } = new UnitWeapon();
        protected HeroPortrait TestPortrait { get; } = new HeroPortrait();

        [Fact]
        public void HeroIdDoesntExistTest()
        {
            Assert.Null(OverrideData.HeroOverride("KaboomBaby"));
        }

        protected void LoadOverrideIntoTestAbility(string abilityName)
        {
            if (HeroOverride.PropertyAbilityOverrideMethodByAbilityId.TryGetValue(abilityName, out Dictionary<string, Action<Ability>> valueOverrideMethods))
            {
                foreach (var propertyOverride in valueOverrideMethods)
                {
                    // execute each property override
                    propertyOverride.Value(TestAbility);
                }
            }
        }

        protected void LoadOverrideIntoTestTalent(string talentName)
        {
            if (HeroOverride.PropertyTalentOverrideMethodByTalentId.TryGetValue(talentName, out Dictionary<string, Action<Talent>> valueOverrideMethods))
            {
                foreach (var propertyOverride in valueOverrideMethods)
                {
                    // execute each property override
                    propertyOverride.Value(TestTalent);
                }
            }
        }

        protected void LoadOverrideIntoTestWeapon(string weaponName)
        {
            if (HeroOverride.PropertyWeaponOverrideMethodByWeaponId.TryGetValue(weaponName, out Dictionary<string, Action<UnitWeapon>> valueOverrideMethods))
            {
                foreach (var propertyOverride in valueOverrideMethods)
                {
                    // execute each property override
                    propertyOverride.Value(TestWeapon);
                }
            }
        }

        protected void LoadOverrideIntoTestPortrait(string heroName)
        {
            if (HeroOverride.PropertyPortraitOverrideMethodByCHeroId.TryGetValue(heroName, out Dictionary<string, Action<HeroPortrait>> valueOverrideMethods))
            {
                foreach (var propertyOverride in valueOverrideMethods)
                {
                    // execute each property override
                    propertyOverride.Value(TestPortrait);
                }
            }
        }

        private void LoadInitialValues()
        {
            TestAbility.Tooltip.Life.LifeCostTooltip = new TooltipDescription("10");

            TestTalent.Tooltip.Energy.EnergyTooltip = new TooltipDescription("500");

            TestWeapon.Damage = 500;
            TestWeapon.Range = 5;
        }
    }
}