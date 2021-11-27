using Sandbox;

using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Player {
	public partial class ClassicPlayer {
		public void AwardKill() {
			ScorePanel.AwardKill();

			// add to current combo
			Client.SetValue("combo", Client.GetValue("combo", 0) + 1);

			//award score
			Client.SetValue("score", Client.GetValue("score", 0) + 100 * Client.GetValue("combo", 0));

			// if combo is bigger than max combo, we have a new max combo
			if(Client.GetValue("combo", 0) > Client.GetValue("maxcombo", 0)) {
				Client.SetValue("maxcombo", Client.GetValue("combo", 0));
			}
		}
	}
}
