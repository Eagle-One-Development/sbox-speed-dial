using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public class GameRoundPanel : Panel {
		public Label timer;
		public Panel container;
		private TimeSince aTime = 0;

		public GameRoundPanel() {
			StyleSheet.Load("/classic/ui/GameRoundPanel.scss");
			container = Add.Panel("container");
			timer = container.Add.Label("00:00", "timer");
		}

		public override void Tick() {
			base.Tick();
			Shadow shadow_cyan = new();
			shadow_cyan.OffsetX = 2f + MathF.Sin(aTime * 2f) * 2f;
			shadow_cyan.OffsetY = 0f;
			shadow_cyan.Color = SpeedDialHud.VHS_CYAN;
			shadow_cyan.Blur = 1f;
			shadow_cyan.Spread = 20f;

			Shadow shadow_magenta = new();
			shadow_magenta.OffsetX = -2f + MathF.Sin(aTime * 2f) * 2f;
			shadow_magenta.OffsetY = 0;
			shadow_magenta.Color = SpeedDialHud.VHS_MAGENTA;
			shadow_magenta.Blur = 1f;
			shadow_magenta.Spread = 20f;

			ShadowList shadows = new();
			shadows.Add(shadow_cyan);
			shadows.Add(shadow_magenta);

			timer.Style.TextShadow = shadows;
			timer.Style.Dirty();

			if(ClassicGamemode.Instance.Round is GameRound gr) {
				container.SetClass("active", true);
				if(gr.TimeLeftFormatted != null) {
					timer.Text = gr.TimeLeftFormatted.ToString();
				}
			} else {
				container.SetClass("active", false);
			}
		}
	}
}
