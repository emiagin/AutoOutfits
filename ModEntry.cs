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
		internal static SeasonManager seasonManager;
		internal static PlayerInfo playerInfo;

		public override void Entry(IModHelper helper)
        {
			ModEntry.helper = Helper;
            monitor = Monitor;
			modManifest = ModManifest;
			playerInfo = new PlayerInfo();
			config = helper.ReadConfig<Config>();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.Player.Warped += OnPlayerWarped;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			monitor.Log($"location = {Game1.player.currentLocation.Name}\nseason = {Game1.currentSeason}", LogLevel.Debug);
			if(SeasonConverter.Convert(Game1.currentSeason) != seasonManager.CurrentSeason)
				SetNewOutfit(Game1.currentSeason, Game1.player.currentLocation.Name);
		}

		private void OnPlayerWarped(object sender, WarpedEventArgs e)
		{
			//monitor.Log($"old location = {e.OldLocation.Name}\nnew location = {e.NewLocation.Name}", LogLevel.Debug);
			SetNewOutfit(Game1.currentSeason, e.NewLocation.Name);
		}

		private void SetNewOutfit(string season, string location)
		{
			try
			{
				locationManager.SetCurrentLocation(location);
				seasonManager.SetCurrentSeason(season);
				foreach (var farmer in config.FarmerOutfits)
				{
					if (farmer.PlayerID == playerInfo.CurrentPlayerInfo.PlayerID)
					{
						outfitManager.SetOutfit(farmer, seasonManager.CurrentSeason.Value, locationManager.CurrentPlayerLocation.Value);
					}
				}
			}
			catch(Exception ex)
			{
				monitor.Log($"ERROR to setting outfit: {ex.Message}", LogLevel.Debug);
			}
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs ev)
        {
			outfitManager = new OutfitManager();
			config.SetConfigMenu();
		}

		private void OnSaveLoaded(object sender, SaveLoadedEventArgs ev)
		{
			playerInfo.OnSaveLoaded(sender, ev);
			locationManager = new LocationManager(Game1.player.currentLocation.Name);
			seasonManager = new SeasonManager(Game1.currentSeason);
			outfitManager.UpdateOutfitIds();
			config.UpdateModConfigMenu();
		}

	}
}