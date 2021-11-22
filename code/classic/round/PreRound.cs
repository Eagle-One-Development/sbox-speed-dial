using System.Linq;

using Sandbox;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic {
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

					ClassicGamemode.Instance.PickNewSoundtrack();

					jp.StopSoundtrack(To.Single(jp), true);
					jp.PlaySoundtrack(To.Single(jp));
				}
			}
		}

		protected override void OnTimeUp() {
			ClassicGamemode.Instance.ChangeRound(new GameRound());

			base.OnTimeUp();
		}
	}
}
