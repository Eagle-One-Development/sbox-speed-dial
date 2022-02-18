using System;
using System.Linq;
using System.Threading.Tasks;

using Sandbox;

using SpeedDial.OneChamber.Player;
using SpeedDial.Classic.UI;

namespace SpeedDial.OneChamber.Rounds {
	public partial class OneChamberGameRound : TimedRound {
		public override TimeSpan RoundDuration => TimeSpan.FromMinutes(5);
		private OneChamberGamemode onechamber => Game.Current.ActiveGamemode as OneChamberGamemode;
		public override string RoundText => "";

		protected override void OnStart() {
			base.OnStart();

			onechamber.SetState(GamemodeState.Running);

			foreach(var client in Client.All.Where(x => x.Pawn is OneChamberPlayer)) {
				var pawn = client.Pawn as OneChamberPlayer;

				pawn.Frozen = false;
			}

			// start climax track 10 seconds before round ends
			_ = PlayClimaxMusic((int)RoundDuration.TotalSeconds - 10);
		}

		protected override void OnFinish() {
			base.OnFinish();
			Game.Current.ActiveGamemode?.ChangeRound(new OneChamberPostRound());

			foreach(var client in Client.All.Where(x => x.Pawn is OneChamberPlayer)) {
				WinScreen.UpdatePanels(To.Single(client));
			}
		}

		private async Task PlayClimaxMusic(int delay) {
			await GameTask.DelaySeconds(delay);
			foreach(var client in Client.All.Where(x => x.Pawn is OneChamberPlayer)) {
				var pawn = client.Pawn as OneChamberPlayer;
				pawn.PlayRoundendClimax(To.Single(client));
			}
		}

		public override void OnPawnJoined(BasePlayer pawn) {
			base.OnPawnJoined(pawn);
			if(pawn is OneChamberPlayer player) {
				player.PlaySoundtrack(To.Single(player.Client));
			}
		}
	}
}
