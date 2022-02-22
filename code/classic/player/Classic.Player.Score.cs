using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Player;

public partial class ClassicPlayer {
	//should this be split up into points/combo and effects?

	/// <summary>
	/// Apply score and combo awards for a kill and display effects if applicable.
	/// </summary>
	/// <param name="killed">The killed player</param>
	public virtual void AwardKill(ClassicPlayer killed) {
		// only award during main round (not warmup)
		if(!Gamemode.Instance.Running) return;
		ScorePanel.AwardKill(To.Single(Client));

		// add to current combo
		Client.AddInt("combo", 1);

		//award score
		var score = 100 * Client.GetValue("combo", 0);
		Client.AddInt("score", score);
		WorldHints.AddHint(To.Single(Client), $"+{score} pts", killed.EyePosition, 1.5f);

		// if combo is bigger than max combo, we have a new max combo
		if(Client.GetValue("combo", 0) > Client.GetValue("maxcombo", 0)) {
			Client.SetValue("maxcombo", Client.GetValue("combo", 0));
		}
	}
}
