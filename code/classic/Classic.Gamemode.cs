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

		protected override void OnStart() {
			SetRound(new TestRound());
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
			}

			// handle domination/revenge stuff
			var killevent = HandleDomination(pawn);

			ScreenHints.FireEvent(To.Single(pawn.Client), "WHACKED", killevent == KillEvent.Domination ? "+DOMINATED" : "+WIP");
			// TODO: tell killer if he's taken revenge when killevent == KillEvent.Revenge
			// TODO: tell victim he's being dominated by someone when killevent == KillEvent.Domination
		}

		public enum KillEvent {
			Domination,
			Revenge,
			None
		}

		public List<Kill> Kills { get; set; } = new();

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

			// dominate on exactly 3 consecutive kills
			if(attackerkills == 3) {
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
