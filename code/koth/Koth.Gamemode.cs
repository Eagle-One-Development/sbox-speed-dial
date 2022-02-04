using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;

using SpeedDial.Classic;
using SpeedDial.Koth.Player;
using SpeedDial.Koth.UI;

namespace SpeedDial.OneChamber {
	[Library("koth"), Hammer.Skip]
	public partial class KothGamemode : ClassicGamemode {
		public override GamemodeIdentity Identity => GamemodeIdentity.Koth;

		protected override void OnClientReady(Client client) {
			client.AssignPawn<KothPlayer>(true);
		}

		public override void CreateGamemodeUI() {
			GamemodeUI = new KothHud();
			Local.Hud = GamemodeUI;
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
