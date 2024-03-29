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

		public override void Entry(IModHelper helper)
        {
			ModEntry.helper = Helper;
            monitor = Monitor;
			modManifest = ModManifest;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs ev)
        {
			outfitManager = new OutfitManager();
			config = helper.ReadConfig<Config>();
		}

		private void OnSaveLoaded(object sender, SaveLoadedEventArgs ev)
		{
			outfitManager.UpdateOutfitIds();
			config.UpdateModConfigMenu();
		}

	}
}