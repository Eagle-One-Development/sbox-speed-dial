using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpeedDial.Player;
using SpeedDial.UI;

namespace SpeedDial {
	[Library("speed-dial")]
	public partial class SpeedDialGame : Game {

		public List<BaseSpeedDialCharacter> characters;

		[Net]
		public BaseRound Round { get; private set; }

		private BaseRound _lastRound;

		[ServerVar("sdial_min_players", Help = "The minimum players required to start the game.")]
		public static int MinPlayers { get; set; } = 1;

		[ServerVar("sdial_debug_enable", Help = "Enable JBall Debug mode.")]
		public static bool DebugEnabled { get; set; } = false;

		public static SpeedDialGame Instance => (SpeedDialGame)Current;

		public SpeedDialGame() {

			PrecacheModels();

			if(IsServer) {
				Log.Info("[SV] Gamemode created!");
				new SpeedDialHud();
			}

			PopulateData();

			if(IsClient) {
				Log.Info("[CL] Gamemode created!");
			}
		}

		public override void OnKilled(Client client, Entity pawn) {
			base.OnKilled(client, pawn);
			var attackerClient = pawn.LastAttacker?.GetClientOwner();
			if(attackerClient != null) {
				Log.Info($"{attackerClient.Name} killed {client.Name}");
				(attackerClient.Pawn as SpeedDialPlayer).ProcessKill();
			}
		}

		private void PopulateData() {
			characters = new();
			characters.Add(new SD_Jack());
			characters.Add(new SD_Maria());
			characters.Add(new SD_Dial_Up());
			characters.Add(new SD_Highway());
		}

		public override void ClientJoined(Client client) {
			base.ClientJoined(client);

			var player = new SpeedDialPlayer();
			client.Pawn = player;

			player.InitialSpawn();
		}

		public async Task StartTickTimer() {
			while(true) {
				await Task.NextPhysicsFrame();
				OnTick();
			}
		}

		private void OnTick() {
			Round?.OnTick();

			if(IsClient) {
				// We have to hack around this for now until we can detect changes in net variables.
				//if ( _lastRound != Round )
				//{
				//	_lastRound?.Finish();
				//	_lastRound = Round;
				//	_lastRound.Start();
				//}
			}
		}

		public async Task StartSecondTimer() {
			while(true) {
				await Task.DelaySeconds(1f);
				OnSecond();
			}
		}

		private void OnSecond() {
			CheckMinimumPlayers();
			Round.OnSecond();
		}

		public override void PostLevelLoaded() {
			_ = StartSecondTimer();
			base.PostLevelLoaded();
		}

		private void CheckMinimumPlayers() {
			if(All.Count >= MinPlayers) {
				if(Round == null || Round is WarmUpRound) {
					ChangeRound(new PreRound());
				}
			} else if(Round is not WarmUpRound) {
				ChangeRound(new WarmUpRound());
			}
		}

		public void ChangeRound(BaseRound round) {
			Assert.NotNull(round);

			Round?.Finish();
			Round = round;
			Round?.Start();
		}

		public override void DoPlayerNoclip(Client player) {
			return;

			// if(!player.HasPermission("noclip"))
			// 	return;

			// if(player.Pawn is SpeedDialPlayer basePlayer) {
			// 	if(basePlayer.DevController is NoclipController) {
			// 		//Log.Info("Noclip Mode Off");
			// 		basePlayer.DevController = null;
			// 	} else {
			// 		//Log.Info("Noclip Mode On");
			// 		basePlayer.DevController = new SpeedDialController();
			// 	}
			// }
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
