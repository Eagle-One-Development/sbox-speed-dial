using SpeedDial.Classic.Player;

using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class LivesPanel : Panel {
	private Label LivesLabel { get; set; }

	public override void Tick() {
		SetClass("visible", Local.Client.Pawn is not ClassicSpectator);

		if(Local.Client.Pawn is OneChamberPlayer player) {
			LivesLabel.Text = $"{player.Lives}";
		}
	}
}
