﻿using System;
using System.Linq;

using Sandbox;

using SpeedDial.Classic.Player;
using SpeedDial.Classic.UI;

namespace SpeedDial.Koth.Rounds {
	public partial class KothPostRound : TimedRound {
		public override TimeSpan RoundDuration => TimeSpan.FromSeconds(11);
		private KothGamemode classic => Game.Current.ActiveGamemode as KothGamemode;
		public override string RoundText => "";

		protected override void OnStart() {
			base.OnStart();

			classic.SetState(GamemodeState.Ending);

			foreach(var client in Client.All.Where(x => x.Pawn is ClassicPlayer)) {
				var pawn = client.Pawn as ClassicPlayer;

				pawn.Frozen = true;
				CharacterSelect.ForceState(To.Single(client), false);
				WinScreen.SetState(To.Single(client), true);
			}
		}

		protected override void OnFinish() {
			base.OnFinish();
			Game.Current.ActiveGamemode?.ChangeRound(new KothPreRound());

			foreach(var client in Client.All) {
				WinScreen.SetState(To.Single(client), false);
			}
		}

		public override void OnPawnJoined(BasePlayer pawn) {
			base.OnPawnJoined(pawn);
			if(pawn is ClassicPlayer player) {
				player.Frozen = true;
			}
		}
	}
}