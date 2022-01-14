using Sandbox;
using Sandbox.UI;

using SpeedDial.Classic.Rounds;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public class ClassicHud : RootPanel {
		public static Color VHS_MAGENTA = new(255f / 255f, 43f / 255f, 112f / 255f, 1.0f);
		public static Color VHS_CYAN = new(0f / 255f, 210f / 255f, 255f / 255f, 1.0f);

		public Panel GameCanvas { get; set; }

		public ClassicHud() {
			if(!Host.IsClient) return;
		}

		public override void Tick() {
			base.Tick();
			GameCanvas?.SetClass("state-visible", ClassicGamemode.Current.ActiveRound is not PostRound);
		}
	}
}
