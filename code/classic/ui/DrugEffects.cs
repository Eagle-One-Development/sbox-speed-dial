using Sandbox;
using Sandbox.UI;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI {
	public class DrugEffects : Panel {
		public float saturation = 0f;
		public float hue = 0;
		public float contrast = 1;
		public float blur = 0;
		public float brightness = 1f;
		public DrugEffects() {
			StyleSheet.Load("/classic/ui/DrugEffects.scss");
		}

		public override void Tick() {

			if(Local.Pawn is SpeedDialPlayer p) {
				if(p.MedTaken && p.TimeSinceMedTaken < p.MedDuration) {
					if(p.CurrentDrug == Meds.DrugType.Polvo) {
						saturation = saturation.LerpTo(2, Time.Delta * 8f);
						hue += Time.Delta * 32f;
						if(hue >= 360f) {
							hue = 0;
						}
						contrast = contrast.LerpTo(1.2f, Time.Delta * 8f);
					}

					if(p.CurrentDrug == Meds.DrugType.Leaf) {
						contrast = contrast.LerpTo(1.3f, Time.Delta * 0.5f);
						saturation = saturation.LerpTo(0.6f, Time.Delta * 0.5f);
						hue = hue.LerpTo(45f, Time.Delta * 2f);
					}

					if(p.CurrentDrug == Meds.DrugType.Ritindi) {
						contrast = contrast.LerpTo(1.3f, Time.Delta * 2f);
						brightness = brightness.LerpTo(1.5f, Time.Delta * 2f);
					}

					if(p.CurrentDrug == Meds.DrugType.Ollie) {
						contrast = contrast.LerpTo(1.2f, Time.Delta * 2f);
						saturation = saturation.LerpTo(0.3f, Time.Delta * 4f);
						brightness = brightness.LerpTo(2f, Time.Delta * 2f);
					}

				} else {
					saturation = saturation.LerpTo(1, Time.Delta * 4f);
					hue = hue.LerpTo(0, Time.Delta * 4f);
					contrast = contrast.LerpTo(1, Time.Delta * 4f);
					blur = blur.LerpTo(0, Time.Delta * 4f);
					brightness = brightness.LerpTo(1, Time.Delta * 4f);
				}
			}

			Style.BackdropFilterSaturate = Length.Pixels(saturation);
			Style.BackdropFilterHueRotate = Length.Pixels(hue);
			Style.BackdropFilterBlur = Length.Pixels(blur);
			Style.BackdropFilterContrast = Length.Pixels(contrast);
			Style.BackdropFilterBrightness = brightness;
		}
	}
}
