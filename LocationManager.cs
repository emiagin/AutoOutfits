using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoOutfits
{
	public enum LocationsEnum { Home, Village, Desert, Island }
	public static class LocationConverter
	{
		public static LocationsEnum? Convert(string locationInGame)
		{
			switch (locationInGame)
			{
				case "FarmHouse":
					return LocationsEnum.Home;
				case "Farm":
				case "BusStop":
				case "Town":
				case "Beach":
				case "Forest":
				case "Woods":
				case "Mountains":
				case "Railroad":
					return LocationsEnum.Village;
				case "Desert":
					return LocationsEnum.Desert;
				case "IslandSouth":
					return LocationsEnum.Island;
			}
			ModEntry.monitor.Log($"WARNING: there's no location named {locationInGame}", LogLevel.Debug);
			return null;
		}
	}
	internal class LocationManager
	{
		internal string currentPlayerLocationString;
		public LocationsEnum? CurrentPlayerLocation => LocationConverter.Convert(currentPlayerLocationString);

		public void SetCurrentLocation(string _currentPlayerLocation)
		{
			currentPlayerLocationString = _currentPlayerLocation;
		}
		public LocationManager(string _currentPlayerLocation = "FarmHouse")
		{
			currentPlayerLocationString = _currentPlayerLocation;
		}
	}
}
