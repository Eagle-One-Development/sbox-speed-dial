using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial;

namespace SpeedDial.UI {
	public class GameRoundPanel : Panel {
		public Label timer;
		public Panel container;
		private TimeSince aTime = 0;
		Color vhs_green;
		Color vhs_magenta;

		public GameRoundPanel() {
			StyleSheet.Load("/ui/GameRoundPanel.scss");
			container = Add.Panel("container");
			timer = container.Add.Label("00:00", "timer");
			vhs_green = new Color(28f / 255f, 255f / 255f, 176f / 255f, 1.0f);//new Color(173f/255f,255f/255f,226f/255f,1.0f);
			vhs_magenta = new Color(255f / 255f, 89 / 255f, 255f / 255f, 1.0f);//new Color(255f / 255f, 163f / 255f, 255f / 255f, 1.0f);
		}
		public override void Tick() {
			base.Tick();
			Shadow s1 = new();
			s1.OffsetX = 2f + MathF.Sin(aTime * 2f) * 2f;
			s1.OffsetY = 0f;
			s1.Color = vhs_green;
			s1.Blur = 1f;
			s1.Spread = 20f;

			Shadow s2 = new();
			s2.OffsetX = -2f + MathF.Sin(aTime * 2f) * 2f;
			s2.OffsetY = 0;
			s2.Color = vhs_magenta;
			s2.Blur = 1f;
			s2.Spread = 20f;

			ShadowList shadows = new();
			shadows.Add(s1);
			shadows.Add(s2);

			timer.Style.TextShadow = shadows;
			timer.Style.Dirty();

			if(SpeedDialGame.Instance.Round is GameRound gr) {
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
