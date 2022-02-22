namespace SpeedDial;

public partial class Hud : HudEntity<BaseRootPanel> {
	public static Hud Instance { get; private set; }
	public static Hud Current => Instance;

	public Hud() {
		if(!IsClient) return;
		Instance = this;
	}

	public static void SetGamemodeUI(Panel panel) {
		Host.AssertClient();
		if(Instance is null || Instance.RootPanel is null) return;

		ClearGamemodeUI();
		Instance.RootPanel.GamemodePanel = panel;
		Instance.RootPanel.AddChild(panel);
	}

	public static void ClearGamemodeUI() {
		Host.AssertClient();
		if(Instance is null || Instance.RootPanel is null) return;

		Instance.RootPanel.GamemodePanel?.Delete();
	}
}
