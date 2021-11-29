using Sandbox;
using Sandbox.UI;

namespace SpeedDial.Classic.UI {
	public class ClassicHud : HudEntity<RootPanel> {
		public static Color VHS_MAGENTA = new(255f / 255f, 43f / 255f, 112f / 255f, 1.0f);
		public static Color VHS_CYAN = new(0f / 255f, 210f / 255f, 255f / 255f, 1.0f);
		public ClassicHud() {
			if(!IsClient) return;
			RootPanel.AddChild<ChatBox>();

			RootPanel.AddChild<WorldHints>();

			RootPanel.AddChild<WeaponPanel>();
			RootPanel.AddChild<ScorePanel>();
			RootPanel.AddChild<DrugPanel>();

			RootPanel.AddChild<ScreenHints>();


			RootPanel.AddChild<Crosshair>();
			RootPanel.AddChild<ClassicScoreboard<ClassicScoreboardEntry>>();
		}
	}
}
