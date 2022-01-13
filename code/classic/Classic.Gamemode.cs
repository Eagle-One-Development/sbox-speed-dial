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

		[Net] public string CurrentSoundtrack { get; set; } = "track01";

		public string[] Soundtracks { get; } = {
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

		public static ClassicGamemode Current => Instance as ClassicGamemode;

		protected override void OnStart() {
			SetRound(new TestRound());
			PickNewSoundtrack();
		}

		protected override void OnClientReady(Client client) {
			client.AssignPawn<ClassicPlayer>(true);
		}

		public override void CreateGamemodeUI() {
			GamemodeUI = new ClassicHud();
		}

		public override bool OnClientSuicide(Client client) {
			if(client.Pawn is ClassicPlayer player) {
				player.DeathCause = ClassicPlayer.CauseOfDeath.Suicide;
			}
			return true;
		}

		protected override void OnPawnKilled(BasePlayer pawn) {
			// killfeed population
			if(pawn is ClassicPlayer player) {
				var client = player.Client;
				if(player.LastAttacker != null) {
					if(player.LastAttacker.Client != null) {
						KillFeed.AddDeath(player.LastAttacker.Client.PlayerId, player.LastAttacker.Client.Name, client.PlayerId, client.Name, player.DeathCause.ToString());
					} else {
						KillFeed.AddDeath(player.LastAttacker.NetworkIdent, player.LastAttacker.ToString(), client.PlayerId, client.Name, player.DeathCause.ToString());
					}
				} else {
					KillFeed.AddDeath(client.PlayerId, client.Name, 0, "", player.DeathCause.ToString());
				}

				// handle domination/revenge stuff
				var killevent = HandleDomination(pawn);

				string killtext;
				if(killevent == KillEvent.Domination) {
					// last attacker dominates pawn, highlight them for pawn
					ClassicPlayer.UpdateGlow(To.Single(pawn.Client), player.LastAttacker as ModelEntity, GlowStates.On, Color.Red);
					killtext = "+DOMINATION";
				} else if(killevent == KillEvent.Revenge) {
					// last attacker has taken revenge against pawn; pawn no longer dominates last attacker
					ClassicPlayer.UpdateGlow(To.Single(player.LastAttacker.Client), pawn, GlowStates.Off, Color.Black);
					killtext = "+REVENGE";
				} else {
					killtext = "";
				}

				// only show killer if we got killed by a player
				if(player.DeathCause == ClassicPlayer.CauseOfDeath.Suicide || player.LastAttacker is null) {
					ScreenHints.FireEvent(To.Single(pawn.Client), "WHACKED", killtext);
				} else {
					ScreenHints.FireEvent(To.Single(pawn.Client), "WHACKED", killtext, true, player.LastAttacker.Client, killevent == KillEvent.Domination || killevent == KillEvent.Revenge);
				}
			}
		}

		public enum KillEvent {
			Domination,
			Revenge,
			None
		}

		public List<Kill> Kills { get; set; } = new();

		/// <summary>
		/// Handle current domination and return the dimination event for the passed killed pawn
		/// </summary>
		/// <param name="pawn">killed pawn</param>
		/// <returns>returns KillEvent.Domination if the pawn is dominated by its last attacker. returns KillEvent.Revenge if the last attacker has taken revenge against pawn. returns KillEvent.None on a normal kill</returns>
		public KillEvent HandleDomination(BasePlayer pawn) {

			// add new kill to list
			var kill = new Kill(pawn.LastRecievedDamage.Attacker, pawn);
			Kills.Add(kill);

			// clear out kills with disconnected/invalid attacker or victim entity
			Kills.RemoveAll(x => !x.Attacker.IsValid() || !x.Victim.IsValid);

			// amount of times last attacker has killed us recently
			var attackerkills = Kills.Where(x => x.Attacker == pawn.LastRecievedDamage.Attacker && x.Victim == pawn).Count();
			// amount of times we killed the attacker recently
			var victimkills = Kills.Where(x => x.Attacker == pawn && x.Victim == pawn.LastRecievedDamage.Attacker).Count();

			// remove all kill entries where the pawn was the attacker and the current attacker was the victim, ends possible domination streaks
			Kills.RemoveAll(x => x.Attacker == pawn && x.Victim == pawn.LastRecievedDamage.Attacker);

			if(victimkills >= 3) {
				Debug.Log($"REVENGE FROM {pawn.LastRecievedDamage.Attacker.Client.Name} AGAINST {pawn.Client.Name}");
				return KillEvent.Revenge;
			}

			// domination on three or more kills
			if(attackerkills >= 3) {
				Debug.Log($"DOMINATION FROM {pawn.LastRecievedDamage.Attacker.Client.Name} AGAINST {pawn.Client.Name}");
				return KillEvent.Domination;
			}

			return KillEvent.None;
		}
	}

	public class Kill {
		public Kill(Entity attacker, Entity victim) {
			Attacker = attacker;
			Victim = victim;
		}

		public Entity Attacker;
		public Entity Victim;
	}
}
