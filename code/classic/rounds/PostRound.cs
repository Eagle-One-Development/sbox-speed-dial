using System;
using System.Linq;

using Sandbox;

using SpeedDial.Classic.Player;
using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Rounds {
	public partial class PostRound: TimedRound {
		public override TimeSpan RoundDuration => TimeSpan.FromSeconds(11);
		public override string RoundText => "";

		protected override void OnStart() {
			base.OnStart();

			foreach(var client in Client.All.Where(x => x.Pawn is ClassicPlayer)) {
				var pawn = client.Pawn as ClassicPlayer;

				pawn.Frozen = true;
				CharacterSelect.ForceState(To.Single(client), false);
			}
		}

		protected override void OnFinish() {
			base.OnFinish();
			Game.Current.ActiveGamemode?.SetRound(new PreRound());
		}
	}
}
