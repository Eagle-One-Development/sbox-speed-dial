using System;
using System.Linq;

using Sandbox;

using SpeedDial.Classic.UI;
using SpeedDial.Classic.Player;

namespace SpeedDial.Classic {
	[Library("sd_classic"), Hammer.Skip]
	public partial class ClassicGamemode : Gamemode {
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
			if(pawn is ClassicPlayer player) {
				var client = player.Client;
				if(player.LastAttacker != null) {
					if(player.LastAttacker.Client != null) {
						KillFeed.AddDeath(player.LastAttacker.Client.PlayerId, player.LastAttacker.Client.Name, client.PlayerId, client.Name, player.DeathCause.ToString());
					} else {
						KillFeed.AddDeath(player.LastAttacker.NetworkIdent, player.LastAttacker.ToString(), client.PlayerId, client.Name, player.DeathCause.ToString());
					}
				} else {
					KillFeed.AddDeath(0, "", client.PlayerId, client.Name, player.DeathCause.ToString());
				}
			}
		}
	}
}
