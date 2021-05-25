using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Player;

namespace SpeedDial {
	public class PreRound : BaseRound {
		public override string RoundName => "Pre-Round";
		public override int RoundDuration => 3;

		private bool _roundStarted;

		public override void OnPlayerSpawn(SpeedDialPlayer player) {
			if(Players.Contains(player)) return;

			AddPlayer(player);

			if(_roundStarted) {
			}

			base.OnPlayerSpawn(player);
		}

		protected override void OnStart() {
			Log.Info("Started Pre Round");
			//var players = Player.All;
			//foreach(var p in players.ToArray()) {
			//	//Log.Info("BLOCK");
			//	if(p is SpeedDialPlayer jp) {
			//		jp.Respawn();
			//		SpeedDialPlayer jbc = jp.Controller as SpeedDialPlayer;
			//		jbc.CanMove = false;
			//	}
			//}

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
