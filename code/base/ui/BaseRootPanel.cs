namespace SpeedDial;

public partial class BaseRootPanel : RootPanel {

	public Panel GamemodePanel { get; set; }

	public BaseRootPanel() {
		if(!Host.IsClient) return;

		StyleSheet.Load("/base/ui/root.scss");
		AddChild<VotingScreen>();
		AddChild<DevInfo>();
		AddChild<DevMenu>();
	}
}
