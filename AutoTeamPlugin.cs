using System;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoTeam
{
	[ApiVersion(2, 1)]
	public class AutoTeamPlugin : TerrariaPlugin
	{
		public override string Author => "Metacinnabar";

		public override string Description => "Automatically joins players to a configurable team when they join the server.";

		public override string Name => "Auto Team";

		public override Version Version => new Version(1, 0, 0);

		private AutoTeamConfigFile config;

		public AutoTeamPlugin(Main game) : base(game) { }

		public override void Initialize()
		{
			// create a new config
			config = new AutoTeamConfigFile();
			// "load" the config with the determined values at the file path (if it exists)
			config.Read(AutoTeamConfig.FilePath, out bool notFound);

			// check if the files exists or not
			if (notFound)
			{
				// create directory structure for the config
				Directory.CreateDirectory(AutoTeamConfig.DirectoryPath);
				// create the default file at the file path
				config.Write(AutoTeamConfig.FilePath);
				// tell the console (and server logs) that the file has been generated
				TShock.Log.ConsoleError($"[AutoTeam] Config file not present. Generated a config file at {AutoTeamConfig.FilePath}");
			}

			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// deregister the getdata hook
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
				// unload the config
				config = null;
			}

			base.Dispose(disposing);
		}

		public enum TeamId
		{
			None = 0,
			Red = 1,
			Green = 2,
			Blue = 3,
			Yellow = 4,
			Pink = 5
		}

		private void OnGetData(GetDataEventArgs args)
		{
			// check if the packet sent is the player update packet
			if (args.MsgID == PacketTypes.PlayerUpdate)
			{
				// create a memory stream used to read information from the packet
				using (MemoryStream data = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length))
				{
					// player variable read from the first byte of the packet
					TSPlayer player = TShock.Players[data.ReadByte()];
					// quick local bool to check if the config is within range of the teams (so there isnt a super long code line)
					bool correctConfig = config.Settings.Team >= (byte)TeamId.Red && config.Settings.Team <= (byte)TeamId.Pink;
					// local team variable defaulting to red team if the config is incorrect
					byte team = correctConfig ? config.Settings.Team : (byte)TeamId.Red;

					// check if the player has been teamed or not
					if (player.GetData<bool>("autoTeamed") == false)
					{
						// change the player's team
						player.SetTeam(team);
						// set the player to be teamed with TSPlayer's custom data setter
						player.SetData("autoTeamed", true);
						// log this information to the console, notifing the server host that a player's team has been changed
						TShock.Log.ConsoleInfo($"[AutoTeam] Set {player.Name}'s team to {(TeamId)team}.");
					}
				}
			}
		}
	}
}
