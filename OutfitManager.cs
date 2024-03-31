using FashionSense.Framework.Interfaces.API;
using GenericModConfigMenu;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutoOutfits.Config;

namespace AutoOutfits
{
	internal class OutfitManager
	{
		public Dictionary<long, List<string>> OutfitIds { get; private set; } = new Dictionary<long, List<string>>();
		public string CurrentOutfitId { get; private set; }

		internal IFashionSenseApi fashionSense;
		public void UpdateOutfitIds()
		{
			var saveFileInfos = ModEntry.playerInfo.SaveFileInfos;
			foreach (var fileInfo in saveFileInfos)
			{
				List<string> outfitIds = new List<string>();
				var list = fileInfo.OutfitIds.ToList();
				list.Add("off");
				if (!OutfitIds.TryGetValue(fileInfo.PlayerID, out outfitIds))
				{
					OutfitIds.Add(fileInfo.PlayerID, list);
				}
				else
				{
					if (list != outfitIds)
					{
						OutfitIds[fileInfo.PlayerID] = list;
					}
				}
			}
		}

		public void SetOutfit(FarmerOutfit farmer, SeasonsEnum season, LocationsEnum location)
		{
			string seasonOutfitId = farmer.GetOutfit(season, location);
			string allOutfitId = farmer.GetOutfit(SeasonsEnum.All, location);
			string outfitId = allOutfitId == "off" ? seasonOutfitId : allOutfitId;
			if(outfitId != "off")
				fashionSense.SetCurrentOutfitId(outfitId, ModEntry.modManifest);
		}

		public OutfitManager()
		{
			fashionSense = ModEntry.helper.ModRegistry.GetApi<IFashionSenseApi>("PeacefulEnd.FashionSense");
			if (fashionSense is null)
			{
				ModEntry.monitor.Log($"WARNING: fashionSense is null", LogLevel.Debug);
				return;
			}

			UpdateOutfitIds();
		}
	}
}
