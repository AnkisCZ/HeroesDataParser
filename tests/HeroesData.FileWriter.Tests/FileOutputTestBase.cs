﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace HeroesData.FileWriter.Tests
{
    public class FileOutputTestBase
    {
        public FileOutputTestBase()
        {
            SetTestHeroData();
            FileOutputNoBuildNumber = FileOutput.SetHeroData(Heroes);
            FileOutputHasBuildNumber = FileOutput.SetHeroData(Heroes, BuildNumber);
            FileOutputFalseSettings = FileOutput.SetHeroData(Heroes, Path.Combine("Configs", "WriterConfigFalseSettings.xml"));
            FileOutputFileSplit = FileOutput.SetHeroData(Heroes, Path.Combine("Configs", "WriterConfigFileSplit.xml"));
            FileOutputRawDescription = FileOutput.SetHeroData(Heroes, Path.Combine("Configs", "WriterConfig0.xml"));
            FileOutputPlainText = FileOutput.SetHeroData(Heroes, Path.Combine("Configs", "WriterConfig1.xml"));
            FileOutputPlainTextWithNewlines = FileOutput.SetHeroData(Heroes, Path.Combine("Configs", "WriterConfig2.xml"));
            FileOutputPlainTextWithScaling = FileOutput.SetHeroData(Heroes, Path.Combine("Configs", "WriterConfig3.xml"));
            FileOutputPlainTextWithScalingWithNewlines = FileOutput.SetHeroData(Heroes, Path.Combine("Configs", "WriterConfig4.xml"));
            FileOutputColoredTextWithScaling = FileOutput.SetHeroData(Heroes, Path.Combine("Configs", "WriterConfig6.xml"));
            FileOutputIsEnabledFalse = FileOutput.SetHeroData(Heroes, Path.Combine("Configs", "WriterConfigEnabledFalse.xml"));
        }

        protected FileOutput FileOutputNoBuildNumber { get; }
        protected FileOutput FileOutputHasBuildNumber { get; }
        protected FileOutput FileOutputFalseSettings { get; }
        protected FileOutput FileOutputFileSplit { get; }
        protected FileOutput FileOutputRawDescription { get; }
        protected FileOutput FileOutputPlainText { get; }
        protected FileOutput FileOutputPlainTextWithNewlines { get; }
        protected FileOutput FileOutputPlainTextWithScaling { get; }
        protected FileOutput FileOutputPlainTextWithScalingWithNewlines { get; }
        protected FileOutput FileOutputColoredTextWithScaling { get; }
        protected FileOutput FileOutputIsEnabledFalse { get; }

        protected int? BuildNumber => 12345;
        protected string OutputFileFolder => "OutputFiles";

        protected List<Hero> Heroes { get; set; } = new List<Hero>();

        [Fact]
        public void FileOuputIsEnabledTrueTest()
        {
            Assert.True(FileOutputNoBuildNumber.IsJsonEnabled);
            Assert.True(FileOutputNoBuildNumber.IsXmlEnabled);
        }

        [Fact]
        public void FileOuputIsEnabledFalseTest()
        {
            Assert.False(FileOutputIsEnabledFalse.IsJsonEnabled);
            Assert.False(FileOutputIsEnabledFalse.IsXmlEnabled);
        }

        protected void CompareFile(string outputFilePath, string testFilePath)
        {
            List<string> output = new List<string>();
            List<string> outputTest = new List<string>();

            // actual created output
            using (StreamReader reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, outputFilePath)))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    output.Add(line);
                }
            }

            using (StreamReader reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, OutputFileFolder, testFilePath)))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputTest.Add(line);
                }
            }

            Assert.Equal(outputTest.Count, output.Count);

            if (outputTest.Count == output.Count)
            {
                for (int i = 0; i < outputTest.Count; i++)
                {
                    Assert.Equal(outputTest[i], output[i]);
                }
            }
        }

        private void SetTestHeroData()
        {
            Hero alarakHero = new Hero
            {
                ShortName = "Alarak",
                Name = "Alarak",
                CHeroId = "HeroAlarak",
                CUnitId = "Alar",
                Difficulty = HeroDifficulty.Hard,
                Franchise = HeroFranchise.Starcraft,
                Gender = HeroGender.Male,
                Radius = 0.875,
                ReleaseDate = new DateTime(2016, 9, 13),
                Sight = 12,
                Speed = 4.3984,
                Type = UnitType.Melee,
                Rarity = HeroRarity.Legendary,
                Description = new TooltipDescription("A Tank who specializes against Mages thanks in part to his innate Spell Armor.<n/><n/><img path=\"@UI / StormTalentInTextArmorIcon\" alignment=\"uppermiddle\" color=\"e12bfc\" width=\"20\" height=\"22\"/><c val=\"#TooltipNumbers\">20 Spell Armor</c>"),
                HeroPortrait = new HeroPortrait()
                {
                    HeroSelectPortraitFileName = "storm_ui_ingame_heroselect_btn_alarak.png",
                    LeaderboardPortraitFileName = "storm_ui_ingame_hero_leaderboard_alarak.png",
                    LoadingScreenPortraitFileName = "storm_ui_ingame_hero_loadingscreen_alarak.png",
                    PartyPanelPortraitFileName = "storm_ui_ingame_partypanel_btn_alarak.png",
                    TargetPortraitFileName = "ui_targetportrait_hero_alarak.png",
                },
                Life = new UnitLife
                {
                    LifeMax = 1900,
                    LifeScaling = 0.04,
                    LifeRegenerationRate = 3.957,
                    LifeRegenerationRateScaling = 0.04,
                },
                Energy = new UnitEnergy
                {
                    EnergyMax = 500,
                    EnergyType = UnitEnergyType.Mana,
                    EnergyRegenerationRate = 3,
                },
                Roles = new List<HeroRole> { HeroRole.Assassin, HeroRole.Warrior },
                Ratings = new HeroRatings()
                {
                    Complexity = 8,
                    Damage = 7,
                    Survivability = 6,
                    Utility = 7,
                },
                Weapons = new List<UnitWeapon>
                {
                    new UnitWeapon
                    {
                        WeaponNameId = "HeroWeaponAlarak",
                        Range = 1.5,
                        Period = 1.2,
                        Damage = 140,
                        DamageScaling = 0.04,
                    },
                    new UnitWeapon
                    {
                        WeaponNameId = "HeroWeaponDestructionAlarak",
                        Range = 2,
                        Period = 1.2,
                        Damage = 340,
                        DamageScaling = 0.05,
                    },
                },
                Abilities = new Dictionary<string, Ability>
                {
                    {
                        "AlarakDiscordStrike",
                        new Ability
                        {
                            ReferenceNameId = "AlarakDiscordStrike",
                            Name = "Discord Strike",
                            ShortTooltipNameId = "AlarakDiscordStrike",
                            FullTooltipNameId = "AlarakDiscordStrike",
                            IconFileName = "storm_ui_icon_alarak_discordstrike.png",
                            Tier = AbilityTier.Basic,
                            Tooltip = new AbilityTalentTooltip()
                            {
                                Energy = new TooltipEnergy
                                {
                                    EnergyCost = 45,
                                },
                                Cooldown = new TooltipCooldown()
                                {
                                    CooldownValue = 8,
                                },
                                ShortTooltip = new TooltipDescription("Damage and silence enemies in an area"),
                                FullTooltip = new TooltipDescription("After a <c val=\"#TooltipNumbers\">0.5</c> second delay, enemies in front of Alarak take <c val=\"#TooltipNumbers\">175</c> damage and are silenced for <c val=\"#TooltipNumbers\">1.5</c> seconds."),
                            },
                        }
                    },
                    {
                        "AlarakSadismDummyUI",
                        new Ability
                        {
                            ReferenceNameId = "AlarakSadismDummyUI",
                            Name = "Sadism",
                            ShortTooltipNameId = "AlarakSadismDummyUI",
                            FullTooltipNameId = "AlarakSadismDummyUI",
                            IconFileName = "storm_ui_icon_alarak_sadism.png",
                            Tier = AbilityTier.Trait,
                            Tooltip = new AbilityTalentTooltip()
                            {
                                ShortTooltip = new TooltipDescription("Alarak deals increased damage and has increased self-healing against enemy Heroes"),
                                FullTooltip = new TooltipDescription("Alarak's Ability damage and self-healing are increased by <c val=\"#TooltipNumbers\">100%</c> against enemy Heroes.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Repeatable Quest:</c> Takedowns increase Sadism by <c val=\"#TooltipNumbers\">3%</c>, up to <c val=\"#TooltipNumbers\">30%</c>. Sadism gained from Takedowns is lost on death."),
                            },
                        }
                    },
                    {
                        "HeroicAbility",
                        new Ability
                        {
                            ReferenceNameId = "HeroicAbility",
                            Name = "Heroic",
                            Tier = AbilityTier.Heroic,
                        }
                    },
                    {
                        "MountAbility",
                        new Ability
                        {
                            ReferenceNameId = "MountAbility",
                            Name = "Mount",
                            Tier = AbilityTier.Mount,
                        }
                    },
                    {
                        "ActivableAbility",
                        new Ability
                        {
                            ReferenceNameId = "ActivableAbility",
                            Name = "Activable",
                            Tier = AbilityTier.Activable,
                        }
                    },
                },
                Talents = new Dictionary<string, Talent>
                {
                    {
                        "AlarakSustainingPower",
                        new Talent
                        {
                            ReferenceNameId = "AlarakSustainingPower",
                            Name = "Sustaining Power",
                            ShortTooltipNameId = "AlarakSustainingPower",
                            FullTooltipNameId = "AlarakSustainingPower",
                            IconFileName = "storm_ui_icon_alarak_lightningsurge_a.png",
                            Tooltip = new AbilityTalentTooltip()
                            {
                                ShortTooltip = new TooltipDescription("Increase Lightning Surge healing"),
                                FullTooltip = new TooltipDescription("Increase the healing received from damaging Heroes with Lightning Surge by <c val=\"#TooltipNumbers\">40%</c>."),
                            },
                            Column = 1,
                            Tier = TalentTier.Level1,
                        }
                    },
                    {
                        "AlarakExtendedLightning",
                        new Talent
                        {
                            ReferenceNameId = "AlarakExtendedLightning",
                            Name = "Extended Lightning",
                            ShortTooltipNameId = "AlarakExtendedLightning",
                            FullTooltipNameId = "AlarakExtendedLightning",
                            IconFileName = "storm_ui_icon_alarak_lightningsurge.png",
                            Tooltip = new AbilityTalentTooltip()
                            {
                                ShortTooltip = new TooltipDescription("<c val=\"#TooltipQuest\">Quest:</c> Reduce Sadism, empower Lightning Surge"),
                                FullTooltip = new TooltipDescription("Reduce Sadism by <c val=\"#TooltipNumbers\">10%</c>.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Quest:</c> Hit Heroes with the center of Lightning Surge.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Reward:</c> After hitting <c val=\"#TooltipNumbers\">5</c> Heroes, increase Lightning Surge's range by <c val=\"#TooltipNumbers\">20%</c>.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Reward:</c> After hitting <c val=\"#TooltipNumbers\">15</c> Heroes, Lightning Surge's center also Slows enemies by <c val=\"#TooltipNumbers\">40%</c> for <c val=\"#TooltipNumbers\">2</c> seconds.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Reward:</c> After hitting <c val=\"#TooltipNumbers\">3</c> Heroes with the center of a single cast, increase Sadism by <c val=\"#TooltipNumbers\">10%</c> and instantly gain all other Rewards."),
                            },
                            Column = 2,
                            Tier = TalentTier.Level1,
                        }
                    },
                    {
                        "Level4Talent",
                        new Talent
                        {
                            ReferenceNameId = "Level4Talent",
                            Name = "Level4Talent",
                            Tier = TalentTier.Level4,
                            Tooltip = new AbilityTalentTooltip()
                            {
                                FullTooltip = new TooltipDescription("Burrow to the target location, dealing <c val=\"#TooltipNumbers\">96~~0.04~~</c> damage and briefly stunning enemies in a small area upon surfacing, slowing them by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2.5</c> seconds.<n/><n/>Burrow Charge can be reactivated to surface early."),
                            },
                        }
                    },
                    {
                        "Level7Talent",
                        new Talent
                        {
                            ReferenceNameId = "Level7Talent",
                            Name = "Level4Talent",
                            Tier = TalentTier.Level7,
                        }
                    },
                    {
                        "AlarakHeroicAbilityDeadlyCharge",
                        new Talent
                        {
                            ReferenceNameId = "AlarakHeroicAbilityDeadlyCharge",
                            Name = "Deadly Charge",
                            ShortTooltipNameId = "AlarakDeadlyCharge",
                            FullTooltipNameId = "AlarakDeadlyCharge",
                            IconFileName = "storm_ui_icon_alarak_recklesscharge.png",
                            Tooltip = new AbilityTalentTooltip()
                            {
                                Cooldown = new TooltipCooldown()
                                {
                                    CooldownValue = 45,
                                },
                                Energy = new TooltipEnergy()
                                {
                                    EnergyCost = 60,
                                },
                                ShortTooltip = new TooltipDescription("Channel to charge a long distance"),
                                FullTooltip = new TooltipDescription("After channeling, Alarak charges forward dealing <c val=\"#TooltipNumbers\">200</c> damage to all enemies in his path. Distance is increased based on the amount of time channeled, up to <c val=\"#TooltipNumbers\">1.6</c> seconds.<n/><n/>Issuing a Move order while this is channeling will cancel it at no cost. Taking damage will interrupt the channeling."),
                            },
                            Column = 1,
                            Tier = TalentTier.Level10,
                        }
                    },
                    {
                        "AlarakHeroicAbilityCounterStrike",
                        new Talent
                        {
                            ReferenceNameId = "AlarakHeroicAbilityCounterStrike",
                            Name = "Counter-Strike",
                            ShortTooltipNameId = "AlarakCounterStrikeTargeted",
                            FullTooltipNameId = "AlarakCounterStrikeTargeted",
                            IconFileName = "storm_ui_icon_alarak_counterstrike.png",
                            Tooltip = new AbilityTalentTooltip()
                            {
                                Energy = new TooltipEnergy()
                                {
                                    EnergyCost = 50,
                                },
                                Cooldown = new TooltipCooldown()
                                {
                                    CooldownValue = 30,
                                },
                                ShortTooltip = new TooltipDescription("Prevents damage to deal damage in a large area"),
                                FullTooltip = new TooltipDescription("Alarak targets an area and channels for <c val=\"#TooltipNumbers\">1</c> second, becoming Protected and Unstoppable. After, if he took damage from an enemy Hero, he sends a shockwave that deals <c val=\"#TooltipNumbers\">275</c> damage."),
                            },
                            Column = 2,
                            Tier = TalentTier.Level10,
                        }
                    },
                    {
                        "Leve13Talent",
                        new Talent
                        {
                            ReferenceNameId = "Leve13Talent",
                            Name = "Leve13Talent",
                            Tier = TalentTier.Level13,
                        }
                    },
                    {
                        "Level16Talent",
                        new Talent
                        {
                            ReferenceNameId = "Level16Talent",
                            Name = "Level16Talent",
                            Tier = TalentTier.Level16,
                        }
                    },
                    {
                        "Level20Talent",
                        new Talent
                        {
                            ReferenceNameId = "Level20Talent",
                            Name = "Level20Talent",
                            Tier = TalentTier.Level20,
                        }
                    },
                },
            };

            Heroes.Add(alarakHero);

            Hero alexstraszaHero = new Hero
            {
                ShortName = "Alexstrasza",
                Name = "Alexstrasza",
                CHeroId = "Alexstrasza",
                CUnitId = "HeroAlexstrasza",
                AttributeId = "Alex",
                Difficulty = HeroDifficulty.Medium,
                Franchise = HeroFranchise.Warcraft,
                Gender = HeroGender.Female,
                InnerRadius = 0.75,
                Radius = 0.75,
                ReleaseDate = new DateTime(2017, 11, 14),
                Sight = 12,
                Speed = 4.3984,
                Type = UnitType.Ranged,
                Rarity = HeroRarity.Legendary,
                Description = new TooltipDescription("A Healer who shares her Health with allies and can transform into a Dragon to empower her Abilities."),
                Life = new UnitLife
                {
                    LifeMax = -1,
                    LifeScaling = 0.04,
                    LifeRegenerationRate = 3.957,
                    LifeRegenerationRateScaling = 0.04,
                },
                Energy = new UnitEnergy
                {
                    EnergyMax = -1,
                    EnergyType = UnitEnergyType.Mana,
                    EnergyRegenerationRate = 3,
                },
                Abilities = new Dictionary<string, Ability>
                {
                    {
                        "TychusOdinAnnihilate",
                        new Ability()
                        {
                            ReferenceNameId = "TychusOdinAnnihilate",
                            Name = "Annihilate",
                            ShortTooltipNameId = "TychusCommandeerOdinAnnihilate",
                            FullTooltipNameId = "TychusCommandeerOdinAnnihilate",
                            IconFileName = "storm_ui_icon_tychus_annihilate.png",
                            Tier = AbilityTier.Basic,
                            ParentLink = "TychusOdinNoHealth",
                            Tooltip = new AbilityTalentTooltip()
                            {
                                 FullTooltip = new TooltipDescription("Burrow to the target location, dealing <c val=\"#TooltipNumbers\">96~~0.04~~</c> damage and briefly stunning enemies in a small area upon surfacing, slowing them by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2.5</c> seconds.<n/><n/>Burrow Charge can be reactivated to surface early."),
                            },
                        }
                    },
                    {
                        "SubAbilHeroic",
                        new Ability
                        {
                            ReferenceNameId = "SubAbilHeroic",
                            Name = "SubAbilHeroic",
                            Tier = AbilityTier.Heroic,
                            ParentLink = "HeroAlexstraszaDragon",
                        }
                    },
                    {
                        "SubAbilMount",
                        new Ability
                        {
                            ReferenceNameId = "SubAbilMount",
                            Name = "SubAbilMount",
                            Tier = AbilityTier.Mount,
                            ParentLink = "HeroAlexstraszaDragon",
                        }
                    },
                    {
                        "SubAbilTrait",
                        new Ability
                        {
                            ReferenceNameId = "SubAbilTrait",
                            Name = "SubAbilTrait",
                            Tier = AbilityTier.Trait,
                            ParentLink = "HeroAlexstraszaDragon",
                        }
                    },
                    {
                        "SubAbilActivable",
                        new Ability
                        {
                            ReferenceNameId = "SubAbilActivable",
                            Name = "SubAbilActivable",
                            Tier = AbilityTier.Activable,
                            ParentLink = "HeroAlexstraszaDragon",
                        }
                    },
                },
                HeroUnits = new List<Unit>
                {
                    new Unit
                    {
                        ShortName = "AlexstraszaDragon",
                        Name = "Alexstrasza",
                        CUnitId = "HeroAlexstraszaDragon",
                        InnerRadius = 1,
                        Radius = 1.25,
                        Sight = 12,
                        Speed = 4.3984,
                        Life = new UnitLife
                        {
                                LifeMax = 1787,
                                LifeScaling = 0.04,
                                LifeRegenerationRate = 3.7226,
                                LifeRegenerationRateScaling = 0.04,
                        },
                        Abilities = new Dictionary<string, Ability>
                        {
                            {
                                "AlexstraszaBreathOfLife",
                                new Ability
                                {
                                    ReferenceNameId = "AlexstraszaBreathOfLife",
                                    Name = "Breath of Life",
                                    ShortTooltipNameId = "AlexstraszaBreathOfLife",
                                    FullTooltipNameId = "AlexstraszaBreathOfLife",
                                    IconFileName = "storm_ui_icon_alexstrasza_breath_of_life.png",
                                    Tier = AbilityTier.Basic,
                                    ParentLink = "HeroAlexstraszaDragon",
                                    Tooltip = new AbilityTalentTooltip()
                                    {
                                        Cooldown = new TooltipCooldown()
                                        {
                                            CooldownValue = 3,
                                            RecastCooldown = 1,
                                        },
                                        Life = new TooltipLife()
                                        {
                                            LifeCost = 50,
                                            IsLifePercentage = true,
                                        },
                                        Charges = new TooltipCharges()
                                        {
                                            CountMax = 3,
                                            CountStart = 3,
                                            CountUse = 1,
                                            IsHideCount = false,
                                        },
                                    },
                                }
                            },
                            {
                                "DragonAbilHeroic",
                                new Ability
                                {
                                    ReferenceNameId = "DragonAbilHeroic",
                                    Name = "DragonAbilHeroic",
                                    Tier = AbilityTier.Heroic,
                                    ParentLink = "HeroAlexstraszaDragon",
                                }
                            },
                            {
                                "DragonAbilMount",
                                new Ability
                                {
                                    ReferenceNameId = "DragonAbilMount",
                                    Name = "DragonAbilMount",
                                    Tier = AbilityTier.Mount,
                                    ParentLink = "HeroAlexstraszaDragon",
                                }
                            },
                            {
                                "DragonAbilTrait",
                                new Ability
                                {
                                    ReferenceNameId = "DragonAbilTrait",
                                    Name = "DragonAbilTrait",
                                    Tier = AbilityTier.Trait,
                                    ParentLink = "HeroAlexstraszaDragon",
                                    Tooltip = new AbilityTalentTooltip()
                                    {
                                         FullTooltip = new TooltipDescription("Burrow to the target location, dealing <c val=\"#TooltipNumbers\">96~~0.04~~</c> damage and briefly stunning enemies in a small area upon surfacing, slowing them by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2.5</c> seconds.<n/><n/>Burrow Charge can be reactivated to surface early."),
                                    },
                                }
                            },
                            {
                                "DragonAbilActivable",
                                new Ability
                                {
                                    ReferenceNameId = "DragonAbilActivable",
                                    Name = "DragonAbilActivable",
                                    Tier = AbilityTier.Activable,
                                    ParentLink = "HeroAlexstraszaDragon",
                                }
                            },
                        },
                    },
                },
            };

            Heroes.Add(alexstraszaHero);
        }
    }
}
