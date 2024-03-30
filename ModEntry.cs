using System;
using System.Threading;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AutoOutfits
{
    internal sealed class ModEntry : Mod
    {
		internal static Config config;
		internal static IModHelper helper;
        internal static IMonitor monitor;
		internal static OutfitManager outfitManager;
		internal static IManifest modManifest;
		internal static LocationManager locationManager;
		internal static PlayerInfo playerInfo;

		public override void Entry(IModHelper helper)
        {
			ModEntry.helper = Helper;
            monitor = Monitor;
			modManifest = ModManifest;
			playerInfo = new PlayerInfo();
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.Player.Warped += OnPlayerWarped;
		}

		private void OnPlayerWarped(object sender, WarpedEventArgs e)
		{
			monitor.Log($"old location = {e.OldLocation.Name}\nnew location = {e.NewLocation.Name}", LogLevel.Debug);
			locationManager.SetLocation(e.NewLocation.Name);
			foreach(var farmer in config.FarmerOutfits)
			{
				if(farmer.FarmerInfo.PlayerID == playerInfo.CurrentPlayerInfo.PlayerID)
				{
					outfitManager.SetOutfit(farmer.GetOutfit(Config.SeasonsEnum.All, locationManager.CurrentPlayerLocation.Value));
				}
			}
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs ev)
        {
			outfitManager = new OutfitManager();
			config = helper.ReadConfig<Config>();
		}

		private void OnSaveLoaded(object sender, SaveLoadedEventArgs ev)
		{
			playerInfo.OnSaveLoaded(sender, ev);
			var playerStartLocation = Game1.player.currentLocation.Name;
			locationManager = new LocationManager(playerStartLocation);
			outfitManager.UpdateOutfitIds();
			config.UpdateModConfigMenu();
		}

	}
}