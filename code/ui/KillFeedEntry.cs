using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;
using System;

namespace SpeedDial.UI {
	public partial class KillFeedEntry : Panel {
		public Label Left { get; internal set; }
		public Label Right { get; internal set; }
		public Image Icon { get; internal set; }
		public bool IsDominating;
		public bool IsMultiKill;
		public bool IsRevenge;

		float phase = 0;

		Color vhs_green;
		Color vhs_magenta;

		public KillFeedEntry() {
			Left = Add.Label("", "left");
			Icon = Add.Image("materials/ui/killfeed_generic.png", "icon");
			Right = Add.Label("", "right");

			vhs_green = new Color(28f / 255f, 255f / 255f, 176f / 255f, 1.0f);
			vhs_magenta = new Color(255f / 255f, 89 / 255f, 255f / 255f, 1.0f);

			phase = Rand.Float(100f);

			_ = RunAsync();
		}

		public override void Tick() {

			Shadow s1 = new();
			s1.OffsetX = 2f + MathF.Sin(Time.Now * 2f) * 2f;
			s1.OffsetY = 0f;
			s1.Color = vhs_green;
			s1.Blur = 4f;
			s1.Blur = 1f;
			s1.Spread = 20f;
			Shadow s2 = new();
			s2.OffsetX = -2f + MathF.Sin(Time.Now * 2f) * 2f;
			s2.OffsetY = 0;
			s2.Color = vhs_magenta;
			s2.Blur = 4f;
			s2.Blur = 1f;
			s2.Spread = 20f;

			ShadowList shadows = new();
			shadows.Add(s1);
			shadows.Add(s2);

			var comboTransform = new PanelTransform();
			float anim = (MathF.Sin(Time.Now * 2f) + 1) / 2;
			float anim2 = MathF.Sin((Time.Now + phase) * 2f);
			comboTransform.AddScale(1f + anim * 0.2f);
			comboTransform.AddRotation(0f, 0f, anim2 * 25f);


			if(IsMultiKill && !IsDominating) {
				Left.Style.Transform = comboTransform;
				Left.Style.TextShadow = shadows;
				Left.Style.Dirty();
			}

			if(IsRevenge && !IsDominating) {
				Right.SetClass("revenge", true);
			}

			if(IsDominating) {
				Left.SetClass("domination", true);
			}

		}

		async Task RunAsync() {
			await Task.Delay(4000);
			Delete();
		}

	}
}
