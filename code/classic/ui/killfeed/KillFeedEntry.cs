namespace SpeedDial.Classic.UI;

[UseTemplate]
public partial class KillFeedEntry : Panel {
	public Label Left { get; set; }
	public Label Right { get; set; }
	public Image Method { get; set; }
	public bool Important { get; set; }

	public RealTimeSince TimeSinceCreated = 0;

	public override void Tick() {
		base.Tick();

		if(TimeSinceCreated > (Important ? 12 : 6)) {
			Delete();
		}
	}
}
