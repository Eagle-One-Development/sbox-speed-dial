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
		private float phase;

		public KillFeedEntry() {
			Left = Add.Label("", "left");
			Icon = Add.Image("materials/ui/killfeed_generic.png", "icon");
			Right = Add.Label("", "right");

			phase = Rand.Float(100f);

			_ = RunAsync();
		}

		public override void Tick() {

			Shadow shadow_cyan = new();
			shadow_cyan.OffsetX = 2f + MathF.Sin(Time.Now * 2f) * 2f;
			shadow_cyan.OffsetY = 0f;
			shadow_cyan.Color = SpeedDialHud.VHS_CYAN;
			shadow_cyan.Blur = 4f;
			shadow_cyan.Blur = 1f;
			shadow_cyan.Spread = 20f;

			Shadow shadow_magenta = new();
			shadow_magenta.OffsetX = -2f + MathF.Sin(Time.Now * 2f) * 2f;
			shadow_magenta.OffsetY = 0;
			shadow_magenta.Color = SpeedDialHud.VHS_MAGENTA;
			shadow_magenta.Blur = 4f;
			shadow_magenta.Blur = 1f;
			shadow_magenta.Spread = 20f;

			ShadowList shadows = new();
			shadows.Add(shadow_cyan);
			shadows.Add(shadow_magenta);

			var comboTransform = new PanelTransform();
			float anim = (MathF.Sin(Time.Now * 2f) + 1) / 2;
			float anim2 = MathF.Sin((Time.Now + phase) * 2f);
			comboTransform.AddScale(1f + anim * 0.2f);
			comboTransform.AddRotation(0f, 0f, anim2 * 25f);


			if(IsMultiKill && !IsDominating) {
				Left.Style.Transform = comboTransform;
				Left.Style.TextShadow = shadows;
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
