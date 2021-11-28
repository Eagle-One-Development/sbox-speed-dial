using Sandbox;
using Sandbox.UI;

namespace SpeedDial.Classic.UI {
	public partial class Crosshair : Panel {

		public readonly Panel cross;

		public readonly Panel[] hairs;

		public static Crosshair Current;

		Vector2 Mouse;

		public float bumpScale;

		public Crosshair() {
			hairs = new Panel[4];
			StyleSheet.Load("/classic/ui/crosshair/Crosshair.scss");
			cross = Add.Panel("cross");

			// TODO: character select
			//SetClass("active", !CharacterSelect.Current.open);
			BindClass("active", () => true);

			for(int i = 0; i < 4; i++) {
				hairs[i] = cross.Add.Panel("hair");
				hairs[i].BindClass("inactive", () => Local.Pawn.ActiveChild == null);
			}

			Current = this;
		}

		[ClientRpc]
		public static void Fire() {
			Current.bumpScale = 0f;
		}

		[ClientRpc]
		public static void UpdateMouse(Vector2 mouse) {
			// floor to prevent fucky pixel snapping
			Current.Mouse = new Vector2(mouse.x.Floor(), mouse.y.Floor());
		}

		public override void Tick() {
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
				hairs[i].Style.Transform = pt;
			}
		}
	}
}
