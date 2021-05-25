using Sandbox;
using System;
using System.Linq;

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

			player.InitialSpawn();
		}

		public override void DoPlayerNoclip(Client player) {
			if(!player.HasPermission("noclip"))
				return;

			if(player.Pawn is SpeedDialPlayer basePlayer) {
				if(basePlayer.DevController is NoclipController) {
					Log.Info("Noclip Mode Off");
					basePlayer.DevController = null;
				} else {
					Log.Info("Noclip Mode On");
					basePlayer.DevController = new SpeedDialController();
				}
			}
		}

		public static void MoveToSpawn(SpeedDialPlayer player) {
			if(Host.IsServer) {

				//info_player_start as spawnpoint (Sandbox.SpawnPoint)
				var spawnpoints = Entity.All.Where((e) => e is SpawnPoint);
				var randomSpawn = spawnpoints.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
				if(randomSpawn == null) {
					//no info_player_start found, fall back to world origin
					player.Position = Vector3.Zero;
					return;
				}

				player.Transform = randomSpawn.Transform;
				return;
			}
		}
	}
}
