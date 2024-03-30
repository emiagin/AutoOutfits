using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using StardewModdingAPI.Events;

namespace AutoOutfits
{
	public class PlayerInfoConfig
	{
		public long PlayerID { get; set; }
		public string FarmerName { get; set; }
		public string[] OutfitIds { get; set; }
	}

	internal class SaveFileHandler
	{
		public static List<PlayerInfoConfig> GetAllSaveFileInfos()
		{
			List<PlayerInfoConfig> saveFileInfos = new List<PlayerInfoConfig>();
			string saveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves");
			if (!Directory.Exists(saveFolderPath))
			{
				ModEntry.monitor.Log($"WARNING: Save folder path does not exist", LogLevel.Debug);
				return saveFileInfos;
			}

			foreach (var directory in Directory.GetDirectories(saveFolderPath))
			{
				string saveFileName = Path.GetFileName(directory);
				string saveFilePath = Path.Combine(directory, saveFileName);

				if (File.Exists(saveFilePath))
				{
					XDocument saveGame = XDocument.Load(saveFilePath);

					// Retrieve the player's ID and name
					long playerID = Convert.ToInt64(saveGame.Root.Element("player").Element("UniqueMultiplayerID").Value);
					string farmerName = saveGame.Root.Element("player").Element("name").Value;
					//ModEntry.monitor.Log($"farmerId = {playerID} farmerName = {farmerName}", LogLevel.Debug);
					string[] outfitIds = GetOutfitIds(saveGame);
					//ModEntry.monitor.Log($"outfitIds = {outfitIds}", LogLevel.Debug);

					saveFileInfos.Add(new PlayerInfoConfig
					{
						PlayerID = playerID,
						FarmerName = farmerName,
						OutfitIds =	outfitIds
					});
				}
			}

			return saveFileInfos;
		}
		private static string[] GetOutfitIds(XDocument saveGame)
		{
			List<string> outfitIds = new List<string>();
			IEnumerable<XElement> xItems;
			try
			{
				xItems = saveGame.Root.Element("player").Element("modData").Elements("item");
			}
			catch(Exception e) 
			{
				//ModEntry.monitor.Log($"WARNING: xItems = null, error = {e.Message}", LogLevel.Debug);
				return outfitIds.ToArray();
			}

			//ModEntry.monitor.Log($"xItems = {xItems}", LogLevel.Debug);
			foreach (var xItem in xItems)
			{
				string key = xItem.Element("key").Element("string").Value;
				//ModEntry.monitor.Log($"item = {xItem}\nkey = {key}", LogLevel.Debug);
				if (key == "FashionSense.Outfit.Collection")
				{
					string value = xItem.Element("value").Element("string").Value;
					//ModEntry.monitor.Log($"item = {xItem}\nkey = {key}\nvalue = {value}", LogLevel.Debug);

					dynamic outfitsJson = JArray.Parse(value);
					//ModEntry.monitor.Log($"outfitsJson = {outfitsJson}, outfitsJson[0].Name = {outfitsJson[0].Name}", LogLevel.Debug);
					for (int i = 0; i < outfitsJson.Count; i++)
					{
						string outfitId = outfitsJson[i].Name;
						outfitIds.Add(outfitId);
					}
				}
			}

			return outfitIds.ToArray();
		}
	}

	internal class PlayerInfo
	{
		internal List<PlayerInfoConfig> saveFileInfos;
		public List<PlayerInfoConfig> SaveFileInfos
		{
			get
			{
				if(saveFileInfos == null)
					saveFileInfos = SaveFileHandler.GetAllSaveFileInfos();
				return saveFileInfos;
			}

		}
		public PlayerInfoConfig CurrentPlayerInfo { get; private set; } = null;

		public string GetName(long playerId)
		{
			var info = SaveFileInfos.FirstOrDefault(p => p.PlayerID == playerId);
			return info.FarmerName;
		}
		public void OnSaveLoaded(object sender, SaveLoadedEventArgs ev)
		{
			long currentPlayerId = Game1.player.UniqueMultiplayerID;
			CurrentPlayerInfo = SaveFileInfos.FirstOrDefault(p => p.PlayerID == currentPlayerId);
		}

		public PlayerInfo(long? currentPlayerId = null)
		{
			if(currentPlayerId != null)
				CurrentPlayerInfo = SaveFileInfos.FirstOrDefault(p => p.PlayerID == currentPlayerId);
		}
	}
}
