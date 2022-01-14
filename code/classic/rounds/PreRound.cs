using System;
using System.Linq;

using Sandbox;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.Rounds {
	public partial class PreRound : TimedRound {
		public override TimeSpan RoundDuration => TimeSpan.FromSeconds(11);
		public override string RoundText => "Round starting.";

		protected override void OnStart() {
			base.OnStart();

			if(Host.IsServer) {
				ClassicGamemode.Current.PickNewSoundtrack();
				// clear kills list to clear domination info
				ClassicGamemode.Current.Kills.Clear();
			}

			foreach(var client in Client.All.Where(x => x.Pawn is ClassicPlayer)) {
				var pawn = client.Pawn as ClassicPlayer;
				pawn.Respawn();

				pawn.StopSoundtrack(To.Single(client), true);
				pawn.PlaySoundtrack(To.Single(client));

				// reset scores etc from warmup
				client.SetValue("score", 0);
				client.SetValue("maxcombo", 0);
				client.SetValue("combo", 0);

				pawn.Frozen = true;
			}
		}

		protected override void OnFinish() {
			base.OnFinish();
			Game.Current.ActiveGamemode?.SetRound(new GameRound());
		}
	}
}
