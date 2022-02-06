using System;
using System.Linq;

using Sandbox;

using SpeedDial.Koth.Player;
using SpeedDial.Koth.Rounds;

namespace SpeedDial.Koth.Rounds {
	public partial class KothWarmupRound : Round {
		private KothGamemode koth => Game.Current.ActiveGamemode as KothGamemode;
		public override string RoundText => $"Waiting for players... [{Client.All.Count}/{Game.MinPlayers}]";

		protected override void OnStart() {
			base.OnStart();

			koth.SetState(GamemodeState.Waiting);

			foreach(var client in Client.All.Where(x => x.Pawn is KothPlayer)) {
				var pawn = client.Pawn as KothPlayer;

				pawn.Frozen = false;
			}
		}

		protected override void OnThink() {
			base.OnThink();
			if(Client.All.Count >= Game.MinPlayers) {
				Finish();
			}
		}

		protected override void OnFinish() {
			base.OnFinish();
			Game.Current.ActiveGamemode.ChangeRound(new KothPreRound());
		}
	}
}
