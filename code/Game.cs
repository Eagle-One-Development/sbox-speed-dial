using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpeedDial.Player;
using SpeedDial.UI;
using SpeedDial.Weapons;

namespace SpeedDial {
	[Library("speed-dial")]
	public partial class SpeedDialGame : Game {

		public List<BaseSpeedDialCharacter> characters;

		[Net]
		public BaseRound Round { get; private set; }

		private BaseRound _lastRound;

		[ServerVar("sdial_min_players", Help = "The minimum players required to start the game.")]
		public static int MinPlayers { get; set; } = 1;

		[ServerVar("sdial_debug_enable", Help = "Enable Speed Dial Debug mode.")]
		public static bool DebugEnabled { get; set; } = false;

		[ServerVar("sdial_score_base", Help = "Set the base value for score calculations.")]
		public static int ScoreBase { get; set; } = 100;

		[ServerVar("sdial_combo_time", Help = "Set the combo time window in seconds.")]
		public static float ComboTime { get; set; } = 3f;

		[ServerCmd]
		public List<SpeedDialPlayer> SortedPlayerList()
		{
			
			return Entity.All.OfType<SpeedDialPlayer>().OrderByDescending( x => x.KillScore ).ToList<SpeedDialPlayer>();
		}

		public static SpeedDialGame Instance => (SpeedDialGame)Current;

		public SpeedDialGame() {

			PrecacheModels();

			Global.PhysicsSubSteps = 2;

			if(IsServer) {
				Log.Info("[SV] Gamemode created!");
				new SpeedDialHud();
			}

			PopulateData();

			if(IsClient) {
				Log.Info("[CL] Gamemode created!");
			}
		}

		public override void DoPlayerSuicide(Client cl) {
			if(cl.Pawn.LifeState != LifeState.Alive || (cl.Pawn as SpeedDialPlayer).TimeSinceDied < 2) return;
			Log.Info($"{cl.Name} committed suicide.");
			cl.Pawn.TakeDamage(DamageInfo.Generic(int.MaxValue));
		}

		public override void OnKilled(Client client, Entity pawn) {
			//base.OnKilled(client, pawn);

			var attackerClient = pawn.LastAttacker?.GetClientOwner();

			if(attackerClient == null) {
				return;
			}

			if(attackerClient != null) {
				var attacker = attackerClient.Pawn as SpeedDialPlayer;
				if(IsServer) {
					Log.Info($"{attackerClient.Name} killed {client.Name}");

					attacker.KillScore += ScoreBase + (ScoreBase * attacker.KillCombo);
					attacker.GetClientOwner().SetScore("score", attacker.KillScore);
					attacker.KillCombo++;

					// fuck ammo
					//(attacker.ActiveChild as BaseSpeedDialWeapon).AwardAmmo();
					//attacker.IncreaseWeaponClip();

					attacker.TimeSinceMurdered = 0;
				}
			}
		}

		[ServerCmd("give_weapon")]
		public static void GiveWeapon(string entityName) {

			if(ConsoleSystem.Caller.Pawn is SpeedDialPlayer player) {
				BaseSpeedDialWeapon weapon = Library.Create<BaseSpeedDialWeapon>(entityName);
				player.Inventory.Add(weapon, true);
			}
		}

		[ServerCmd("set_character")]
		public static void SetCharacter(int index) {
			if(ConsoleSystem.Caller.Pawn is SpeedDialPlayer player) {
				player.character = SpeedDialGame.Instance.characters[index];
			}
		}

		[ServerCmd("spawn_entity")]
		public static void SpawnEntity(string entName) {
			var owner = ConsoleSystem.Caller.Pawn;

			if(owner == null)
				return;



			var ent = Library.Create<Entity>(entName);
			if(ent is BaseCarriable && owner.Inventory != null) {
				if(owner.Inventory.Add(ent, true))
					return;
			}

			ent.Position = owner.EyePos;


			//Log.Info( $"ent: {ent}" );
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
				await GameTask.NextPhysicsFrame();
				OnTick();
			}
		}

		private void OnTick() {
			Round?.OnTick();
		}

		public async Task StartSecondTimer() {
			while(true) {
				await GameTask.DelaySeconds(1f);
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
			// who needs noclip anyways
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
