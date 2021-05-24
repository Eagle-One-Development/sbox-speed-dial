using Sandbox;
using SpeedDial.Player;
using SpeedDial.UI;

namespace SpeedDial {
	[Library("speed-dial")]
	public partial class SpeedDialGame : Game {
		public SpeedDialGame() {
			if(IsServer) {
				Log.Info("[SV] Gamemode created!");
				new SpeedDialHud();
			}

			if(IsClient) {
				Log.Info("[CL] Gamemode created!");
			}
		}

		public override void ClientJoined(Client client) {
			base.ClientJoined(client);

			var player = new SpeedDialPlayer();
			client.Pawn = player;

			player.Respawn();
		}
	}
}
