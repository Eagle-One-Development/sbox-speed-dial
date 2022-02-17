using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;

using SpeedDial.Classic;
using SpeedDial.Koth.Player;
using SpeedDial.Koth.UI;
using SpeedDial.Koth.Rounds;

namespace SpeedDial.Koth {
	[Library("koth"), Hammer.Skip]
	public partial class KothGamemode : ClassicGamemode {
		public override GamemodeIdentity Identity => GamemodeIdentity.Koth;
		public override string BotType => "koth";

		protected override void OnClientReady(Client client) {
			client.AssignPawn<KothPlayer>(true);
		}

		public override void CreateGamemodeUI() {
			Hud.SetGamemodeUI(new KothHud());
		}

		protected override void OnStart() {
			ChangeRound(new KothWarmupRound());
		}
	}
}
