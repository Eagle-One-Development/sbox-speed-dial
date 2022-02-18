using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;

using SpeedDial.Classic;

using SpeedDial.OneChamber.Player;
using SpeedDial.OneChamber.UI;

namespace SpeedDial.OneChamber {
	[Library("onechamber"), Hammer.Skip]
	public partial class OneChamberGamemode : ClassicGamemode {
		public override GamemodeIdentity Identity => GamemodeIdentity.OneChamber;

		protected override void OnClientReady(Client client) {
			client.AssignPawn<OneChamberPlayer>(true);
		}
		protected override void OnPawnKilled(BasePlayer pawn) {
			if(pawn.LastAttacker is OneChamberPlayer player) {
				player.AwardKill();
			}
		}

		public override void CreateGamemodeUI() {
			Hud.SetGamemodeUI(new OneChamberHud());
		}

		protected override void OnStart() {

		}

		public override void EnableEntity(GamemodeEntity ent) {
			// one chamber doesn't have gamemode entities
			return;
		}

		public override void DisableEntity(GamemodeEntity ent) {
			// whatever
			return;
		}

		public override void HandleGamemodeEntity(GamemodeEntity ent) {
			ent.Disable();
		}
	}
}
