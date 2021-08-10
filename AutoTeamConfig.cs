using System.IO;
using TShockAPI;
using TShockAPI.Configuration;

namespace AutoTeam
{
	public class AutoTeamConfig
	{
		public static string DirectoryPath = Path.Combine(TShock.SavePath, "AutoTeam");

		public static string FilePath = Path.Combine(DirectoryPath, "AutoTeam.json");

		public byte Team { get; set; } = (byte)AutoTeamPlugin.TeamId.Red;
	}

	public class AutoTeamConfigFile : ConfigFile<AutoTeamConfig> { }
}