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
	}
}
