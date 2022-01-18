using System;
using System.Linq;

using Sandbox;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.Rounds {
	public partial class WarmupRound : Round {

		public override string RoundText => "Waiting for players...";

		protected override void OnStart() {
			base.OnStart();

			foreach(var client in Client.All.Where(x => x.Pawn is ClassicPlayer)) {
				var pawn = client.Pawn as ClassicPlayer;

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
			Game.Current.ActiveGamemode.SetRound(new PreRound());
		}
	}
}
