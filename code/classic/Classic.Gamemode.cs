using System;
using System.Linq;
using System.Collections.Generic;

using Sandbox;

using SpeedDial.Classic.UI;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Rounds;

namespace SpeedDial.Classic {
	[Library("classic"), Hammer.Skip]
	public partial class ClassicGamemode : Gamemode {

		public override GamemodeIdentity Identity => GamemodeIdentity.Classic;

		/// <summary>
		/// Name of the current soundtrack as listed in the Soundtracks array.
		/// </summary>
		[Net] public string CurrentSoundtrack { get; set; } = "track01";

		/// <summary>
		/// Abailable soundtracks.
		/// </summary>
		public string[] Soundtracks { get; } = {
			"track01",
			"track02",
			"track03",
			"track03"
		};

		/// <summary>
		/// Set ClassicGamemode.CurrentSoundtrack to a random soundtrack from the Soundtracks array
		/// </summary>
		public void PickNewSoundtrack() {
			int index = Rand.Int(0, Soundtracks.Length - 1);
			CurrentSoundtrack = Soundtracks[index];
		}

		public override void MoveToSpawnpoint(BasePlayer pawn) {
			Host.AssertServer();
			var spawnpoints = All.Where((s) => s is SpawnPoint);
			Entity optimalSpawn = spawnpoints.ToList()[0];
			float optimalDistance = 0;
			foreach(var spawn in spawnpoints) {
				float smallestDistance = 999999;
				foreach(var player in All.Where((p) => p is BasePlayer)) {
					var distance = Vector3.DistanceBetween(spawn.Position, player.Position);
					if(distance < smallestDistance) {
						smallestDistance = distance;
					}
				}
				if(smallestDistance > optimalDistance) {
					optimalSpawn = spawn;
					optimalDistance = smallestDistance;
				}
			}
			pawn.Transform = optimalSpawn.Transform;
		}

		public static ClassicGamemode Current => Instance as ClassicGamemode;

		protected override void OnStart() {
			ChangeRound(new WarmupRound());
			PickNewSoundtrack();
		}

		protected override void OnFinish() {
			foreach(var client in Client.All.Where(x => x.Pawn is ClassicPlayer)) {
				// lol
				(client.Pawn as ClassicPlayer).StopSoundtrack(To.Single(client));
			}
		}

		protected override void OnClientReady(Client client) {
			Log.Debug("gamemode client ready");
			client.AssignPawn<ClassicPlayer>(true);
		}

		public override void CreateGamemodeUI() {
			Hud.SetGamemodeUI(new ClassicHud());
		}

		public override bool OnClientSuicide(Client client) {
			if(client.Pawn is ClassicPlayer player) {
				player.DeathCause = ClassicPlayer.CauseOfDeath.Suicide;
			}
			return true;
		}
	}
}
