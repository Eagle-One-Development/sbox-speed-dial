using Sandbox;
using Sandbox.UI.Construct;

using System;
using System.IO;
using System.Threading.Tasks;

using SpeedDial.Base;

namespace SpeedDial {

	public partial class GamemodesGame : Game {

		[Net] public BaseGamemode Gamemode { get; set; }
		[Net] private bool Abort { get; set; } = false;
		public static string GamemodeName { get; } = "sd_classic";
		public static GamemodesGame Instance { get; protected set; }

		public GamemodesGame() {
			Instance = this;
			if(IsServer) {
				Log.Info("[SV] Game created");
				Gamemode = Library.Create<BaseGamemode>(GamemodeName);
				if(Gamemode is null) {
					Log.Error("GAMEMODES", $"COULDN'T INITIALIZE GAMEMODE {GamemodeName}");
					Log.Info("GAMEMODES", $"COULDN'T INITIALIZE GAMEMODE {GamemodeName}");
					Abort = true;
					return;
				}
				Gamemode.GamemodeInitializeUI();
			}

			if(IsClient) {
				Log.Info("[CL] Game created");
			}
		}

		public override void Simulate(Client cl) {
			base.Simulate(cl);
			Gamemode.GamemodeSimulate(cl);
		}

		public override void ClientJoined(Client client) {
			if(Abort) {
				client.Kick();
				return;
			}
			base.ClientJoined(client);
			Gamemode.GamemodeClientJoined(client);
		}

		public override void DoPlayerSuicide(Client cl) {
			//base.DoPlayerSuicide();
			Gamemode.GamemodeDoPlayerSuicide(cl);
		}

		public override void DoPlayerNoclip(Client player) {
			//base.DoPlayerNoclip();
		}

		public override void OnKilled(Client client, Entity pawn) {
			base.OnKilled();
			Gamemode.GamemodeOnKilled(client, pawn);
		}

		public override void PostLevelLoaded() {
			base.PostLevelLoaded();
			Gamemode.GamemodePostLevelLoaded();
		}
	}
}
