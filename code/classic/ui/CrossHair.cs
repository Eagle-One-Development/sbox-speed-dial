using Sandbox;
using Sandbox.UI;

namespace SpeedDial.Classic.UI {
	public partial class CrossHair : Panel {

		public Panel cross;

		public Panel[] hairs;

		public static CrossHair Current;

		Vector2 Mouse;

		public float bumpScale;

		public CrossHair() {
			hairs = new Panel[4];
			StyleSheet.Load("/classic/ui/CrossHair.scss");
			cross = Add.Panel("cross");
			for(int i = 0; i < 4; i++) {
				hairs[i] = cross.Add.Panel("hair");
			}

			Current = this;

		}

		public void Bump() {
			bumpScale = 0f;
		}

		[ClientRpc]
		public static void UpdateMouse(Vector2 mouse) {
			Current.Mouse = mouse;
		}

		public override void Tick() {
			base.Tick();

			SetClass("active", !CharacterSelect.Current.open);

			cross.Style.Left = Length.Fraction(Mouse.x / Screen.Width);
			cross.Style.Top = Length.Fraction(Mouse.y / Screen.Height);

			float f = bumpScale;
			bumpScale = bumpScale.LerpTo(1f, Time.Delta * 6f);

			for(int i = 0; i < 4; i++) {
				PanelTransform pt = new();

				pt.AddRotation(0, 0, i * 90f);

				float pixel = 18f + 20f * (1 - f);
				if(i == 0) {
					pt.AddTranslateY(Length.Pixels(pixel));
				}

				if(i == 1) {
					pt.AddTranslateX(Length.Pixels(pixel));
				}

				if(i == 2) {
					pt.AddTranslateY(Length.Pixels(-pixel));
				}

				if(i == 3) {
					pt.AddTranslateX(Length.Pixels(-pixel));
				}
				hairs[i].SetClass("inactive", Local.Pawn.ActiveChild == null);
				hairs[i].Style.Transform = pt;
			}
		}
	}
}
