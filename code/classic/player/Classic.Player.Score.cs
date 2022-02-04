using Sandbox;

using SpeedDial.Classic.UI;
using SpeedDial.Classic.Rounds;

namespace SpeedDial.Classic.Player {
	public partial class ClassicPlayer {
		//should this be split up into points/combo and effects?

		/// <summary>
		/// Apply score and combo awards for a kill and display effects if applicable.
		/// </summary>
		/// <param name="killed">The killed player</param>
		public void AwardKill(ClassicPlayer killed) {
			// only award during main round (not warmup)
			if(!Gamemode.Instance.Running) return;
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
