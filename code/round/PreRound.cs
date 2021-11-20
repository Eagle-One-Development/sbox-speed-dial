using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Player;
using SpeedDial.UI;

namespace SpeedDial {
	public class PreRound : BaseRound {
		public override string RoundName => "Pre-Round";
		public override int RoundDuration => 8;//8

		//private bool _roundStarted;

		public override void OnPlayerSpawn(SpeedDialPlayer player) {
			if(Players.Contains(player)) return;

			AddPlayer(player);

			base.OnPlayerSpawn(player);
		}

		protected override void OnStart() {
			var players = Client.All;
			foreach(var p in players.ToArray()) {

				if(p.Pawn is SpeedDialPlayer jp) {
					jp.Respawn();
					jp.Freeze = true;

					SpeedDialGame.Instance.PickNewSoundtrack();

					jp.StopSoundtrack(To.Single(jp), true);
					jp.PlaySoundtrack(To.Single(jp));
				}
			}
		}

		protected override void OnTimeUp() {
			SpeedDialGame.Instance.ChangeRound(new GameRound());

			base.OnTimeUp();
		}
	}
}
