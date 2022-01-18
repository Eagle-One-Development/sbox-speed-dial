using Sandbox;

using SpeedDial.Classic.UI;
using SpeedDial.Classic.Rounds;

namespace SpeedDial.Classic.Player {
	public partial class ClassicPlayer {
		public void AwardKill(ClassicPlayer killed) {
			if(ClassicGamemode.Current.ActiveRound is not GameRound) return;
			ScorePanel.AwardKill();

			// add to current combo
			Client.SetValue("combo", Client.GetValue("combo", 0) + 1);

			//award score
			var score = 100 * Client.GetValue("combo", 0);
			Client.SetValue("score", Client.GetValue("score", 0) + score);
			WorldHints.AddHint(To.Single(Client), $"+{score} pts", killed.EyePos, 1.5f);

			// if combo is bigger than max combo, we have a new max combo
			if(Client.GetValue("combo", 0) > Client.GetValue("maxcombo", 0)) {
				Client.SetValue("maxcombo", Client.GetValue("combo", 0));
			}
		}
	}
}
