using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class RoundPanel : Panel {
		private readonly Panel Timer;
		private readonly Label TimeLabel;

		public RoundPanel() {
			StyleSheet.Load("/classic/ui/round/RoundPanel.scss");
			Timer = Add.Panel("timer");
			TimeLabel = Timer.Add.Label("00:00", "timelabel");
		}

		public override void Tick() {
			if(Game.Current.GetGamemode() is Gamemode gamemode) {
				if(gamemode.GetRound() is TimedRound timedRound) {
					TimeLabel.Text = $"-{timedRound.TimeLeftFormatted}";
				} else if(gamemode.GetRound() is Round round) {
					TimeLabel.Text = $"+{round.TimeElapsedFormatted}";
				}
			}
		}
	}
}
