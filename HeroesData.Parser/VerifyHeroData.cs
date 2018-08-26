﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HeroesData.Parser
{
    public class VerifyHeroData
    {
        private readonly List<Hero> HeroData = new List<Hero>();
        private readonly string VerifyIgnoreFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "VerifyIgnore.txt");

        private string HeroName;

        private HashSet<string> IgnoreLines = new HashSet<string>();

        private VerifyHeroData(List<Hero> heroData)
        {
            HeroData = heroData;

            ReadIgnoreFile();
            VerifyData();
        }

        public HashSet<string> Warnings { get; private set; } = new HashSet<string>();
        public int WarningsIgnored { get; private set; } = 0;

        /// <summary>
        /// Verifies the all the hero data for missing data.
        /// </summary>
        /// <param name="heroData">A list of all hero data.</param>
        /// <returns></returns>
        public static VerifyHeroData Verify(List<Hero> heroData)
        {
            return new VerifyHeroData(heroData);
        }

        private void VerifyData()
        {
            foreach (Hero hero in HeroData)
            {
                HeroName = hero.Name;

                if (string.IsNullOrEmpty(hero.AttributeId))
                    AddWarning($"{nameof(hero.AttributeId)} is null or empty");

                if (string.IsNullOrEmpty(hero.Description?.RawDescription))
                    AddWarning($"{nameof(hero.Description)} is null or empty");

                if (string.IsNullOrEmpty(hero.Difficulty))
                    AddWarning($"{nameof(hero.Difficulty)} is Unknown");

                if (hero.Franchise == HeroFranchise.Unknown)
                    AddWarning($"{nameof(hero.Franchise)} is Unknown");

                if (hero.Roles.Contains("Unknown"))
                    AddWarning($"{nameof(hero.Roles)} is Unknown");

                if (hero.Abilities.Count < 1)
                    AddWarning("Hero has no abilities");

                if (hero.Talents.Count < 1)
                    AddWarning("Hero has no talents");

                if (hero.Life.LifeMax <= 0)
                    AddWarning($"{nameof(hero.Life)} is 0");

                if (hero.Life.LifeScaling <= 0)
                    AddWarning($"{nameof(hero.Life.LifeScaling)} is 0");

                if (hero.Life.LifeRegenerationRateScaling <= 0)
                    AddWarning($"{nameof(hero.Life.LifeRegenerationRateScaling)} is 0");

                if (hero.Energy.EnergyMax > 0 && string.IsNullOrEmpty(hero.Energy.EnergyType))
                    AddWarning($"{nameof(hero.Energy)} > 0 and {nameof(hero.Energy.EnergyType)} is NONE");

                if (hero.Armor != null && hero.Armor.PhysicalArmor < 1 && hero.Armor.SpellArmor < 1)
                    AddWarning($"{nameof(hero.Armor.PhysicalArmor)} and {nameof(hero.Armor.SpellArmor)} are both 0");

                if (hero.Sight <= 0)
                    AddWarning($"{nameof(hero.Sight)} is 0");

                if (hero.Speed <= 0)
                    AddWarning($"{nameof(hero.Speed)} is 0");

                if (hero.Rarity == HeroRarity.None)
                    AddWarning($"{nameof(hero.Rarity)} is None");

                if (!hero.ReleaseDate.HasValue)
                    AddWarning($"{nameof(hero.ReleaseDate)} is null");

                if (string.IsNullOrEmpty(hero.Type))
                    AddWarning($"{nameof(hero.Type)} is null or emtpy");

                VerifyWeapons(hero);

                if (hero.Weapons.Count < 1)
                    AddWarning("has no weapons");
                else if (hero.Weapons.Count > 1)
                    AddWarning("has more than 1 weapon");

                if (hero.PrimaryAbilities(AbilityTier.Basic).Count < 3)
                    AddWarning($"has less than 3 basic abilities");

                if (hero.PrimaryAbilities(AbilityTier.Basic).Count > 3)
                    AddWarning($"has more than 3 basic abilities");

                VerifyAbilities(hero);

                // hero portraits
                if (string.IsNullOrEmpty(hero.HeroPortrait.HeroSelectPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.HeroSelectPortraitFileName)}]  is null or empty");

                if (string.IsNullOrEmpty(hero.HeroPortrait.LeaderboardPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.LeaderboardPortraitFileName)}]  is null or empty");

                if (string.IsNullOrEmpty(hero.HeroPortrait.LoadingScreenPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.LoadingScreenPortraitFileName)}]  is null or empty");

                if (string.IsNullOrEmpty(hero.HeroPortrait.PartyPanelPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.PartyPanelPortraitFileName)}]  is null or empty");

                if (string.IsNullOrEmpty(hero.HeroPortrait.TargetPortraitFileName))
                    AddWarning($"[{nameof(hero.HeroPortrait.TargetPortraitFileName)}]  is null or empty");

                foreach (var talent in hero.Talents)
                {
                    if (string.IsNullOrEmpty(talent.Value.IconFileName))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.IconFileName)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Name))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Name)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.ReferenceNameId))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.ReferenceNameId)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.FullTooltipNameId))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.FullTooltipNameId)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.ShortTooltipNameId))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.ShortTooltipNameId)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.ShortTooltip?.RawDescription))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.ShortTooltip)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.RawDescription))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.PlainText))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.PlainText)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.PlainTextWithNewlines))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.PlainTextWithScaling)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.PlainTextWithScaling))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.PlainTextWithScaling)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.PlainTextWithScalingWithNewlines))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.PlainTextWithScalingWithNewlines)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.ColoredText))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.ColoredText)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip?.ColoredTextWithScaling))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)}.{nameof(talent.Value.Tooltip.FullTooltip.ColoredTextWithScaling)} is null or empty");

                    if (talent.Value.Tooltip.Cooldown?.CooldownText != null)
                    {
                        if (char.IsDigit(talent.Value.Tooltip.Cooldown.CooldownText.PlainText[0]))
                            AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.Cooldown.CooldownText)} does not have a prefix");
                    }

                    if (talent.Value.Tooltip.Energy?.EnergyText != null)
                    {
                        if (char.IsDigit(talent.Value.Tooltip.Energy.EnergyText.PlainText[0]))
                            AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.Energy.EnergyText)} does not have a prefix");
                    }

                    if (talent.Value.Tooltip.Life?.LifeCostText != null)
                    {
                        if (char.IsDigit(talent.Value.Tooltip.Life.LifeCostText.PlainText[0]))
                            AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.Life.LifeCostText)} does not have a prefix");
                    }
                }

                if (hero.HeroUnits.Count > 0)
                {
                    foreach (Unit additionalUnit in hero.HeroUnits)
                    {
                        if (additionalUnit.Life.LifeMax <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Life)} is 0");

                        if (additionalUnit.Life.LifeScaling <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Life.LifeScaling)} is 0");

                        if (additionalUnit.Life.LifeRegenerationRateScaling <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Life.LifeRegenerationRateScaling)} is 0");

                        if (additionalUnit.Energy.EnergyMax > 0 && string.IsNullOrEmpty(hero.Energy.EnergyType))
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Energy)} > 0 and {nameof(additionalUnit.Energy.EnergyType)} is NONE");

                        if (additionalUnit.Sight <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Sight)} is 0");

                        if (additionalUnit.Speed <= 0)
                            AddWarning($"[{additionalUnit.Name}] {nameof(additionalUnit.Speed)} is 0");

                        VerifyWeapons(hero);
                        VerifyAbilities(hero);
                    }
                }
            }
        }

        private void VerifyWeapons(Unit unit)
        {
            foreach (var weapon in unit.Weapons)
            {
                if (weapon.Damage <= 0)
                    AddWarning($"{weapon.WeaponNameId} {nameof(weapon.Damage)} is 0");

                if (weapon.Period <= 0)
                    AddWarning($"{weapon.WeaponNameId} {nameof(weapon.Period)} is 0");

                if (weapon.Range <= 0)
                    AddWarning($"{weapon.WeaponNameId} {nameof(weapon.Range)} is 0");

                if (weapon.DamageScaling <= 0)
                    AddWarning($"{weapon.WeaponNameId} {nameof(weapon.DamageScaling)} is 0");
            }
        }

        private void VerifyAbilities(Unit unit)
        {
            foreach (var ability in unit.Abilities)
            {
                if (string.IsNullOrEmpty(ability.Value.IconFileName))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.IconFileName)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.Name))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.Name)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.ReferenceNameId))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.ReferenceNameId)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.FullTooltipNameId))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.FullTooltipNameId)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.ShortTooltipNameId))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.ShortTooltipNameId)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.Tooltip.ShortTooltip?.RawDescription))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.ShortTooltip)} is null or empty");

                if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip?.RawDescription))
                    AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)} is null or empty");

                if (!string.IsNullOrEmpty(ability.Value.Tooltip.Cooldown?.CooldownText?.RawDescription))
                {
                    if (char.IsDigit(ability.Value.Tooltip.Cooldown.CooldownText.PlainText[0]))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.Cooldown.CooldownText)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(ability.Value.Tooltip.Energy?.EnergyText?.RawDescription))
                {
                    if (char.IsDigit(ability.Value.Tooltip.Energy.EnergyText.PlainText[0]))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.Energy.EnergyText)} does not have a prefix");
                }

                if (!string.IsNullOrEmpty(ability.Value.Tooltip.Life?.LifeCostText?.RawDescription))
                {
                    if (char.IsDigit(ability.Value.Tooltip.Life.LifeCostText.PlainText[0]))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.Life.LifeCostText)} does not have a prefix");
                }
            }
        }

        private void AddWarning(string message)
        {
            message = $"[{HeroName}] {message}".Trim();

            if (!IgnoreLines.Contains(message))
                Warnings.Add(message);
            else
                WarningsIgnored++;
        }

        private void ReadIgnoreFile()
        {
            if (File.Exists(VerifyIgnoreFileName))
            {
                using (StreamReader reader = new StreamReader(VerifyIgnoreFileName))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                        {
                            IgnoreLines.Add(line);
                        }
                    }
                }
            }
        }
    }
}
