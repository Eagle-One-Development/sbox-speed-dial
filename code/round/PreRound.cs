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
		public override int RoundDuration => 5;

		//private bool _roundStarted;

		public override void OnPlayerSpawn(SpeedDialPlayer player) {
			if(Players.Contains(player)) return;

			AddPlayer(player);

			//if(_roundStarted) {
			//}

			base.OnPlayerSpawn(player);
		}

		protected override void OnStart() {
			Log.Info("Pre Round Started");
			var players = Client.All;
			foreach(var p in players.ToArray()) {

				if(p.Pawn is SpeedDialPlayer jp) {
					jp.Respawn();
					(jp.Controller as SpeedDialController).Freeze = true;
					jp.Frozen = true;
					jp.StopSoundtrack(To.Single(jp));
					jp.PlaySoundtrack(To.Single(jp), "track01");
				}
			}
			SpeedDialHud.Scoreboard?.UpdateScoreboard();
		}

		protected override void OnFinish() {
			Log.Info("Finished Pre Round");

		}

		protected override void OnTimeUp() {
			Log.Info("Pre Round Up!");

			SpeedDialGame.Instance.ChangeRound(new GameRound());

			base.OnTimeUp();
		}
	}
}
