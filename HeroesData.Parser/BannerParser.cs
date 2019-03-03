﻿using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class BannerParser : ParserBase, IParser<Banner, BannerParser>
    {
        public BannerParser(GameData gameData, DefaultData defaultData)
            : base(gameData, defaultData)
        {
        }

        public IList<string[]> Items
        {
            get
            {
                List<string[]> items = new List<string[]>();

                IEnumerable<XElement> cBannerElements = GameData.XmlGameData.Root.Elements("CBanner").Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                foreach (XElement bannerElement in cBannerElements)
                {
                    string id = bannerElement.Attribute("id").Value;
                    if (bannerElement.Element("AttributeId") != null && id != "RandomBanner" && id != "NeutralMercCamp")
                        items.Add(new string[] { id });
                }

                return items;
            }
        }

        public BannerParser GetInstance()
        {
            return new BannerParser(GameData, DefaultData);
        }

        public Banner Parse(params string[] ids)
        {
            if (ids == null || ids.Count() < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement bannerElement = GameData.XmlGameData.Root.Elements("CBanner").Where(x => x.Attribute("id")?.Value == id).FirstOrDefault();
            if (bannerElement == null)
                return null;

            Banner banner = new Banner()
            {
                Id = id,
            };

            SetDefaultValues(banner);
            SetBannerData(bannerElement, banner);

            if (banner.ReleaseDate == DefaultData.HeroReleaseDate)
                banner.ReleaseDate = DefaultData.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(banner.HyperlinkId))
                banner.HyperlinkId = id;

            return banner;
        }

        private void SetBannerData(XElement bannerElement, Banner banner)
        {
            // parent lookup
            string parentValue = bannerElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.XmlGameData.Root.Elements("CBanner").FirstOrDefault(x => x.Attribute("id")?.Value == parentValue);
                if (parentElement != null)
                    SetBannerData(parentElement, banner);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.BannerDescription.Replace(DefaultData.IdPlaceHolder, bannerElement.Attribute("id")?.Value));
                if (!string.IsNullOrEmpty(desc))
                    banner.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in bannerElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "DESCRIPTION")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        banner.Description = new TooltipDescription(text);
                }
                else if (elementName == "SORTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        banner.SortName = text;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.BannerReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.BannerReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.BannerReleaseDate.Year;

                    banner.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    banner.AttributeId = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    banner.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out Rarity heroRarity))
                        banner.Rarity = heroRarity;
                    else
                        banner.Rarity = Rarity.Unknown;
                }
                else if (elementName == "NAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        banner.Name = text;
                }
                else if (elementName == "COLLECTIONCATEGORY")
                {
                    banner.CollectionCategory = element.Attribute("value")?.Value;
                }
                else if (elementName == "EVENTNAME")
                {
                    banner.EventName = element.Attribute("value")?.Value;
                }
            }
        }

        private void SetDefaultValues(Banner banner)
        {
            banner.Name = GameData.GetGameString(DefaultData.BannerName.Replace(DefaultData.IdPlaceHolder, banner.Id));
            banner.SortName = GameData.GetGameString(DefaultData.BannerSortName.Replace(DefaultData.IdPlaceHolder, banner.Id));
            banner.Description = new TooltipDescription(GameData.GetGameString(DefaultData.BannerDescription.Replace(DefaultData.IdPlaceHolder, banner.Id)));
            banner.ReleaseDate = DefaultData.BannerReleaseDate;
            banner.Rarity = Rarity.None;
        }
    }
}
