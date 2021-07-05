using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpeedDial.Player;
using SpeedDial.UI;
using SpeedDial.Weapons;

namespace SpeedDial {
	public partial class SpeedDialGame : Game {

		public List<BaseSpeedDialCharacter> characters;

		[Net]
		public BaseRound Round { get; private set; }

		//private BaseRound _lastRound;

		[ServerVar("sdial_min_players", Help = "The minimum players required to start the game.")]
		public static int MinPlayers { get; set; } = 1;

		[ServerVar("sdial_debug_enable", Help = "Enable Speed Dial Debug mode.")]
		public static bool DebugEnabled { get; set; } = false;

		[ServerVar("sdial_score_base", Help = "Set the base value for score calculations.")]
		public static int ScoreBase { get; set; } = 100;

		[ServerVar("sdial_combo_time", Help = "Set the combo time window in seconds.")]
		public static float ComboTime { get; set; } = 5f;

		[ServerVar("sdial_debug_infinite_ammo")]
		[Net]
		public static bool InfiniteAmmo { get; set; } = false;

		[Net]
		public string CurrentSoundtrack { get; set; } = "track01";

		[Net]
		public string[] Soundtracks { get; set; } = {
			"track01",
			"track02",
			"track03",
			"track03"
		};

		public void PickNewSoundtrack() {
			var random = new Random();
			int index = random.Next(0, Soundtracks.Length);
			CurrentSoundtrack = Soundtracks[index];
		}



		[ServerCmd]
		public List<SpeedDialPlayer> SortedPlayerList() {

			return Entity.All.OfType<SpeedDialPlayer>().OrderByDescending(x => x.KillScore).ToList<SpeedDialPlayer>();
		}

		public static SpeedDialGame Instance { get; private set; }

		public SpeedDialGame() {

			Instance = this;

			PrecacheModels();

			Global.PhysicsSubSteps = 2;

			KillFeed = new List<KillFeedEntry>();

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

		public List<KillFeedEntry> KillFeed;


		public override void OnKilled(Client client, Entity pawn) {
			//base.OnKilled(client, pawn);

			if(pawn is SpeedDialPlayer ply) {
				if(ply.Inventory.DropActive() is BaseSpeedDialWeapon dropped && dropped != null) {
					dropped.Position = pawn.EyePos;
					dropped.DespawnAfterTime = true;
					dropped.GlowState = GlowStates.GlowStateOn;
					dropped.GlowDistanceStart = 0;
					dropped.GlowDistanceEnd = 1000;
					if(dropped.AmmoClip > 0)
						dropped.GlowColor = new Color(0.2f, 1, 0.2f, 1);
					else {
						if(dropped.AmmoClip == -1)
							dropped.GlowColor = new Color(1, 1, 1, 1);
						else
							dropped.GlowColor = new Color(1, 0.2f, 0.2f, 1);
					}
					dropped.GlowActive = true;
				}
			}

			var attackerClient = pawn.LastAttacker?.GetClientOwner();

			if(attackerClient == null) {
				return;
			}

			(pawn as SpeedDialPlayer).MedTaken = false;

			#region Domination
			//Two variables set up for determining if we've dominated or taken revenge
			bool revenge = false;
			bool dominating = false;


			//Add a new kill feed entry with the victim as the pawn who just died, and the last attacker as the agressor
			KillFeedEntry entry = new KillFeedEntry(pawn, pawn.LastAttacker);
			KillFeed.Add(entry);

			//Temp variable for counting number of kills
			int numKills = 0;

			//Remove any kill feed entries where the player enttiy is no longer valid, prevents errors when players disconnect
			KillFeed.RemoveAll(item => !item.attacker.IsValid() || !item.victim.IsValid());

			//Iterate through the kill feed and check the number of kills our attacker had against us before we killed them.
			for(int i = 0; i < KillFeed.Count; i++) {

				KillFeedEntry check = KillFeed[i];
				if(check.attacker == pawn && check.victim == pawn.LastAttacker) {
					numKills++;
				}
			}

			//Remove any instance from the kill feed where we, who is now dead, were the attacker, and our kiler was the victim.
			//This basically resets dominations, you need to get 3 kills in a row in order to dominate someone
			KillFeed.RemoveAll(item => item.attacker == pawn && item.victim == pawn.LastAttacker);

			//Basically if the person that killed us is someone we were dominating, tell them they got revenge and remove the icon fro mtheir HUD
			if(numKills >= 3) {
				(pawn.LastAttacker as SpeedDialPlayer).DrugBump(To.Single(pawn.LastAttacker.GetClientOwner()), "REVENGE", "AGAINST " + pawn.GetClientOwner().Name, false);
				(pawn.LastAttacker as SpeedDialPlayer).Revenge(To.Single(pawn.LastAttacker.GetClientOwner()), pawn);
				revenge = true;

			}

			//Reset the number of kills
			numKills = 0;

			//Iterate over the kill feed and check how many times our attacker has killed us
			for(int i = 0; i < KillFeed.Count; i++) {
				KillFeedEntry check = KillFeed[i];

				if(check.attacker == pawn.LastAttacker && check.victim == pawn) {
					numKills++;
				}
			}

			//Log.Info(pawn.LastAttacker.GetClientOwner().Name + " NUMBER OF KILLS: " + numKills.ToString());

			//If it's 3 kills, and they weren't taking revenge on us, then we will say we are dominating them and add an icon to their hud
			if(numKills == 3 && !revenge) {
				(pawn.LastAttacker as SpeedDialPlayer).DrugBump(To.Single(pawn.LastAttacker.GetClientOwner()), "DOMINATING", pawn.GetClientOwner().Name, false);
				(pawn as SpeedDialPlayer).Dominate(To.Single(pawn.GetClientOwner()), pawn.LastAttacker);
				dominating = true;
				Log.Info(pawn.LastAttacker.GetClientOwner().Name + " IS DOMINATING " + pawn.GetClientOwner().Name);
			}

			//Moved the kill messages here so that we can let players know who and when someone is dominating them
			if(dominating) {
				(pawn as SpeedDialPlayer).DrugBump(To.Single(pawn), pawn.LastAttacker.GetClientOwner().Name, "IS DOMINATING YOU", false);
			} else {
				(pawn as SpeedDialPlayer).DrugBump(To.Single(pawn), pawn.LastAttacker.GetClientOwner().Name, "KILLED YOU", false);
			}

			#endregion

			if(attackerClient != null) {
				var attacker = attackerClient.Pawn as SpeedDialPlayer;
				if(IsServer) {
					Log.Info($"{attackerClient.Name} killed {client.Name}");

					attacker.KillScore += ScoreBase + (ScoreBase * attacker.KillCombo);
					attacker.GetClientOwner().SetScore("score", attacker.KillScore);
					//attacker.KillCombo++;

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

		public static void MoveToSpawn(SpeedDialPlayer respawnPlayer) {
			if(Host.IsServer) {

				//info_player_start as spawnpoint (Sandbox.SpawnPoint)
				var spawnpoints = All.Where((s) => s is SpawnPoint);
				Entity optimalSpawn = spawnpoints.ToList()[0];
				float optimalDistance = 0;

				foreach(var spawn in spawnpoints) {
					float smallestDistance = 999999;
					foreach(var player in All.Where((p) => p is SpeedDialPlayer)) {
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

				respawnPlayer.Transform = optimalSpawn.Transform;
				return;
			}
		}
	}

	public class KillFeedEntry {
		public Entity victim;
		public Entity attacker;

		public KillFeedEntry(Entity v, Entity a) {
			victim = v;
			attacker = a;
		}
	}
}
