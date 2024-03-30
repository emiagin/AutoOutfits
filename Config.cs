using GenericModConfigMenu;
using Microsoft.VisualBasic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using static AutoOutfits.Config;

namespace AutoOutfits
{
	class Config
	{
		public enum SeasonsEnum { All, Spring, Summer, Fall, Winter }
		internal class FarmerOutfit
		{
			public PlayerInfoConfig FarmerInfo { get; private set; }
			public SeasonOutfit[] SeasonOutfits { get; set; }

			public SeasonOutfit All => Outfit(SeasonsEnum.All);
			public SeasonOutfit Spring => Outfit(SeasonsEnum.Spring);
			public SeasonOutfit Summer => Outfit(SeasonsEnum.Summer);
			public SeasonOutfit Fall => Outfit(SeasonsEnum.Fall);
			public SeasonOutfit Winter => Outfit(SeasonsEnum.Winter);

			public FarmerOutfit(PlayerInfoConfig farmerInfo)
			{
				FarmerInfo = farmerInfo;
				SeasonOutfits = new SeasonOutfit[Enum.GetNames(typeof(SeasonsEnum)).Length];
				for(int i = 0; i < SeasonOutfits.Length; i++)
				{
					SeasonOutfits[i] = new SeasonOutfit((SeasonsEnum)i);
				}
			}
			
			public SeasonOutfit Outfit(SeasonsEnum season)
			{
				return SeasonOutfits[(int)season];
			}

			public string GetOutfit(SeasonsEnum season, LocationsEnum location)
			{
				return Outfit(season).Values[location];
			}
		}
		internal class SeasonOutfit
		{
			public SeasonsEnum Season { get; set; }
			public string Home => Values[LocationsEnum.Home];
			public string Village => Values[LocationsEnum.Village];
			public string Desert => Values[LocationsEnum.Desert];
			public string Island => Values[LocationsEnum.Island];
			public Dictionary<LocationsEnum, string> Values { get; set; } = new Dictionary<LocationsEnum, string>();

			public SeasonOutfit(SeasonsEnum season)
			{
				Season = season;
				for (int i = 0; i < Enum.GetNames(typeof(LocationsEnum)).Length; i++)
				{
					Values.Add((LocationsEnum)i, "off");
				}
			}

			public void ChangeValue(LocationsEnum location, string outfitId)
			{
				Values[location] = outfitId;
			}
		}
		public List<FarmerOutfit> FarmerOutfits { get; set; } = new List<FarmerOutfit>();

		public void ResetToDefault()
		{
			var playerInfos = ModEntry.playerInfo.SaveFileInfos;
			FarmerOutfits = new List<FarmerOutfit>();
			foreach(var info in playerInfos)
			{
				FarmerOutfits.Add(new FarmerOutfit(info));
			}
		}

		public void ApplyConfig()
		{
			ModEntry.helper.WriteConfig(this);
		}

		public void UpdateModConfigMenu()
		{
			var outfitIds = ModEntry.outfitManager.OutfitIds;

			configMenu.Unregister(ModEntry.modManifest);
			configMenu.Register(ModEntry.modManifest, ResetToDefault, ApplyConfig);

			foreach(var farmer in FarmerOutfits)
			{
				configMenu.AddPageLink(ModEntry.modManifest, "page_" + farmer.FarmerInfo.PlayerID, () => farmer.FarmerInfo.FarmerName);
			}
			foreach (var farmer in FarmerOutfits)
			{
				configMenu.AddPage(ModEntry.modManifest, "page_" + farmer.FarmerInfo.PlayerID, () => farmer.FarmerInfo.FarmerName);
				CreatePageMenu(farmer, outfitIds[farmer.FarmerInfo.PlayerID].ToArray());
			}
		}

		private void CreatePageMenu(FarmerOutfit farmer, string[] outfitIds)
		{
			foreach(var seasonOutfits in farmer.SeasonOutfits)
			{
				configMenu.AddSectionTitle(ModEntry.modManifest, () => seasonOutfits.Season.ToString());
				foreach (var season in seasonOutfits.Values)
				{
					configMenu.AddTextOption(
						ModEntry.modManifest,
						() => season.Value,
						(value) => seasonOutfits.ChangeValue(season.Key, value),
						() => season.Key.ToString(),
						null,
						outfitIds);
				}
			}
		}

		internal IGenericModConfigMenuApi configMenu = null;
		public Config()
		{
			configMenu = ModEntry.helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (configMenu is null)
			{
				ModEntry.monitor.Log($"WARNING: configMenu is null", LogLevel.Debug);
				return;
			}
			configMenu.Register(ModEntry.modManifest, ResetToDefault, ApplyConfig);
			
			ResetToDefault();
			UpdateModConfigMenu();
		}
	}
}