using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoOutfits
{
	public enum SeasonsEnum { All, Spring, Summer, Fall, Winter }
	public static class SeasonConverter
	{
		public static SeasonsEnum? Convert(string seasonInGame)
		{
			switch (seasonInGame)
			{
				case "spring":
					return SeasonsEnum.Spring;
				case "summer":
					return SeasonsEnum.Summer;
				case "fall":
					return SeasonsEnum.Fall;
				case "winter":
					return SeasonsEnum.Winter;
			}
			ModEntry.monitor.Log($"WARNING: there's no season named {seasonInGame}", LogLevel.Debug);
			return null;
		}
	}
	internal class SeasonManager
	{
		internal string currentSeasonString;
		public SeasonsEnum? CurrentSeason => SeasonConverter.Convert(currentSeasonString);

		public void SetCurrentSeason(string _currentSeason)
		{
			currentSeasonString = _currentSeason;
		}
		public SeasonManager(string _currentSeason)
		{
			currentSeasonString = _currentSeason;
		}
	}
}
