using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpeedDial.Player;
using SpeedDial.UI;
using SpeedDial.Weapons;
using SpeedDial.Settings;

namespace SpeedDial {
	public partial class SpeedDialGame : Game {

		[Net] public BaseRound Round { get; private set; }
		[ServerVar("sdial_min_players", Help = "The minimum players required to start the game.")]
		public static int MinPlayers { get; set; } = 1;
		[ServerVar("sdial_bot_debug_enable", Help = "Enable Speed Dial Bot Debug mode.")]
		public static bool BotDebugEnabled { get; set; } = false;

		[ServerVar("sdial_bot_difficulty", Help = "Difficulty of bots.")]
		public static BotDifficulties BotDifficulty { get; set; } = BotDifficulties.Medium;
		[ServerCmd]
		public static void SetBotDifficulty(BotDifficulties difficulty) {
			BotDifficulty = difficulty;
			ConsoleSystem.SetValue("sdial_bot_difficulty", difficulty);
			//Log.Error(difficulty);
		}
		[ServerCmd]
		public static void AddBot() {
			ConsoleSystem.Run("bot_add");
		}

		[ServerVar("sdial_score_base", Help = "Set the base value for score calculations.")]
		public static int ScoreBase { get; set; } = 100;
		[ServerVar("sdial_combo_time", Help = "Set the combo time window in seconds.")]
		public static float ComboTime { get; set; } = 5f;
		[Net] public string CurrentSoundtrack { get; set; } = "track01";
		[Net]
		public string[] Soundtracks { get; set; } = {
			"track01",
			"track02",
			"track03",
			"track03"
		};
		[Net] public bool SniperCanPenetrate { get; set; } = false;

		public void PickNewSoundtrack() {
			var random = new Random();
			int index = random.Next(0, Soundtracks.Length);
			CurrentSoundtrack = Soundtracks[index];
		}

		[ClientRpc]
		public void OnKilledMessage(long leftid, string left, long rightid, string right, string method, bool IsDom, bool IsMult, bool IsRevenge, COD cod) {
			UI.KillFeed.Instance?.AddEntry(leftid, left, rightid, right, method, IsDom, IsMult, IsRevenge, cod);
		}

		[ServerCmd]
		public List<SpeedDialPlayer> SortedPlayerList() {
			return All.OfType<SpeedDialPlayer>().OrderByDescending(x => x.Client.GetValue("score", 0)).ToList();
		}

		public static SpeedDialGame Instance { get; private set; }

		public SpeedDialGame() {

			Instance = this;

			PrecacheModels();

			Global.PhysicsSubSteps = 2;

			KillFeed = new List<KillFeedEntry>();

			if(IsServer) {
				Log.Info("[SV] Gamemode created!");
				_ = new SpeedDialHud();
			}

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

			var attackerClient = pawn.LastAttacker?.Client;

			if(attackerClient == null) {
				OnKilledMessage(0, "", client.PlayerId, client.Name, "died");
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
				GamePanel.ScreenEvent(To.Single(pawn.LastAttacker.Client), "REVENGE", "AGAINST " + pawn.Client.Name, false);
				GamePanel.RemoveDominator(To.Single(pawn.LastAttacker.Client), pawn);
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

			//Log.Info(pawn.LastAttacker.Client.Name + " NUMBER OF KILLS: " + numKills.ToString());

			//If it's 3 kills, and they weren't taking revenge on us, then we will say we are dominating them and add an icon to their hud
			if(numKills == 3 && !revenge) {
				GamePanel.ScreenEvent(To.Single(pawn.LastAttacker.Client), "DOMINATING", pawn.Client.Name, false);
				GamePanel.AddDominator(To.Single(pawn.Client), pawn.LastAttacker);
				dominating = true;
				Log.Info(pawn.LastAttacker.Client.Name + " IS DOMINATING " + pawn.Client.Name);
			}

			//Moved the kill messages here so that we can let players know who and when someone is dominating them
			if(dominating) {
				GamePanel.ScreenEvent(To.Single(pawn), pawn.LastAttacker.Client.Name, "IS DOMINATING YOU", false);
			} else {
				GamePanel.ScreenEvent(To.Single(pawn), pawn.LastAttacker.Client.Name, "KILLED YOU", false);
			}

			bool multiKill = false;
			#endregion

			if(attackerClient != null) {
				var attacker = attackerClient.Pawn as SpeedDialPlayer;
				if(IsServer) {
					Log.Info($"{attackerClient.Name} killed {client.Name}");

					attacker.Client.SetValue("score", attacker.Client.GetValue("score", 0) + ScoreBase + (ScoreBase * attacker.Client.GetValue("killcombo", 0)));
					attacker.Client.SetValue("score", attacker.Client.GetValue("score", 0));

					attacker.TimeSinceMurdered = 0;

					if(attacker.Client.GetValue("killcombo", 0) >= 2) {
						multiKill = true;
					}
				}
			}

			Log.Info($"LAST ATTACKER: {(pawn.LastAttacker as SpeedDialPlayer).ActiveChild}");
			if((pawn.LastAttacker as SpeedDialPlayer).ActiveChild != null) {

				if((pawn.LastAttacker as SpeedDialPlayer).ActiveChild.ToString() == "sd_bat") {
					(pawn as SpeedDialPlayer).CauseOfDeath = COD.Melee;
				}
			}

			if(pawn.LastAttacker != null) {

				if(attackerClient != null) {
					OnKilledMessage(attackerClient.PlayerId, attackerClient.Name, client.PlayerId, client.Name, pawn.LastAttackerWeapon?.ClassInfo?.Name, dominating, multiKill, revenge, (pawn as SpeedDialPlayer).CauseOfDeath);
				} else {
					OnKilledMessage((long)pawn.LastAttacker.NetworkIdent, pawn.LastAttacker.ToString(), client.PlayerId, client.Name, "killed", false, false, false, (pawn as SpeedDialPlayer).CauseOfDeath);
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
				if(index > Character.All.Count) return;
				player.character = Character.All.ElementAtOrDefault(index);
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
		}

		public override void ClientJoined(Client client) {
			base.ClientJoined(client);

			var player = client.IsBot ? new SpeedDialBotPlayer() : new SpeedDialPlayer();
			client.Pawn = player;

			player.InitialSpawn();
		}

		public override void ClientSpawn() {
			base.ClientSpawn();
			Host.AssertClient();
			ReloadSettingsAfterDelay();
		}
		private static async void ReloadSettingsAfterDelay() {
			await GameTask.NextPhysicsFrame();
			SettingsManager.ReloadSettings();
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
