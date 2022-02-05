using Sandbox;
using Sandbox.UI;

using SpeedDial.Classic.Rounds;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public class ClassicHud : Panel {
		public static Color VHS_MAGENTA = new(255f / 255f, 43f / 255f, 112f / 255f, 1.0f);
		public static Color VHS_CYAN = new(0f / 255f, 210f / 255f, 255f / 255f, 1.0f);

		public ClassicHud() {
			if(!Host.IsClient) return;
			AddClass("root");
		}

		public override void Tick() {
			base.Tick();
			// game panels hide in post round
			SetClass("state-visible-game", !Gamemode.Instance.Ending && !Debug.UI);
			// cursor only shows when using mouse input
			SetClass("state-visible-cursor", !Input.UsingController);
		}
	}
}
