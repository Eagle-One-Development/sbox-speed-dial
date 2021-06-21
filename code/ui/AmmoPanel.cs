using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;
using SpeedDial.Player;
using SpeedDial.Meds;


namespace SpeedDial.UI {
	public class AmmoPanel : Panel {
		public Panel ammoCounter;
		public Label ammoLabel;

		public Label clipLabel;

		public Panel pickUpPanel;
		public Label pickUpLabel;

		public Panel drugPanel;
		public Image drugImage;

		public Label medLabel;
		public Label medFlavor;
		private TimeSince aTime = 0;
		public static AmmoPanel Current;

		Color vhs_green;
		Color vhs_magenta;

		private float outscale;
		public float pickedup;
		private float pickeduptar;

		private float scale;

		private float wideScale;
		private float totalDrugScale;
		private float old;
		private float oldScale;
		public Label preRoundCountDownLabel;
		public Label preRoundMenuLabel;

		public AmmoPanel() {
			StyleSheet.Load("/ui/AmmoPanel.scss");
			ammoCounter = Add.Panel("counter");
			clipLabel = ammoCounter.Add.Label("0", "ammoLabel");

			pickUpPanel = Add.Panel("pickuppanel");
			pickUpLabel = pickUpPanel.Add.Label("Right Click To Pick Up", "pickuplabel");

			medLabel = pickUpPanel.Add.Label("DRUG TAKEN", "medlabel");
			medFlavor = medLabel.Add.Label("you feel better", "medFlavor");

			Current = this;
			scale = 0;

			var panel = Add.Panel("countdown");
			preRoundCountDownLabel = panel.Add.Label("10", "timer");
			preRoundMenuLabel = panel.Add.Label("PRESS " + Input.GetKeyWithBinding("+iv_duck").ToUpper() + " TO OPEN CHARACTER SELECT","char");

			vhs_green = new Color(28f / 255f, 255f / 255f, 176f / 255f, 1.0f);//new Color(173f/255f,255f/255f,226f/255f,1.0f);
			vhs_magenta = new Color(255f / 255f, 89 / 255f, 255f / 255f, 1.0f);//new Color(255f / 255f, 163f / 255f, 255f / 255f, 1.0f);

			drugPanel = Add.Panel("drug");
			drugImage = drugPanel.Add.Image("materials/ui/smile.png", "drugImage");

		}

		public void Bump() {
			scale = 0.7f;
		}



		public void DrugBump(string s, string f) {
			wideScale = 0.5f;
			totalDrugScale = 1.0f;
			medLabel.Text = s + " TAKEN";
			medFlavor.Text = f;
		}

		public override void Tick() {
			Shadow s1 = new();
			s1.OffsetX = 2f + MathF.Sin(aTime * 2f) * 2f;
			s1.OffsetY = 0f;
			s1.Color = vhs_green;
			s1.Blur = 4f;

			Shadow s2 = new();
			s2.OffsetX = -2f + MathF.Sin(aTime * 2f) * 2f;
			s2.OffsetY = 0;
			s2.Color = vhs_magenta;
			s2.Blur = 4f;

			ShadowList shadows = new();
			shadows.Add(s1);
			shadows.Add(s2);

			float anim = (MathF.Sin(aTime * 2f) + 1) / 2;
			float anim2 = MathF.Sin(aTime * 1f);
			PanelTransform transform = new();

			scale = scale.LerpTo(0, Time.Delta * 8f);

			transform.AddScale((0.8f + anim * 0.2f + scale) * outscale);
			transform.AddRotation(0f, 0f, anim2 * 5f);

			PanelTransform transform2 = new();
			transform2.AddScale(pickeduptar);
			transform2.AddRotation(0f, 0f, anim2 * 5f + (360f * 1 - pickeduptar));

			clipLabel.Style.TextShadow = shadows;
			clipLabel.Style.Transform = transform;
			clipLabel.Style.Dirty();

			pickeduptar = pickeduptar.LerpTo(pickedup, Time.Delta * 8f);

			pickUpLabel.Style.Transform = transform2;
			pickUpLabel.Style.TextShadow = shadows;
			pickUpLabel.Style.Dirty();

			PanelTransform transform3 = new();
			transform3.AddScale(new Vector3(totalDrugScale + wideScale, totalDrugScale, totalDrugScale));

			wideScale = wideScale.LerpTo(0, Time.Delta * 4f);
			if(wideScale <= 0.01f) {
				totalDrugScale = totalDrugScale.LerpTo(0, Time.Delta * 8f);
			}

			medLabel.Style.Transform = transform3;
			medLabel.Style.TextShadow = shadows;
			medLabel.Style.Dirty();

			if(SpeedDialGame.Instance.Round is PreRound gr) {
				if(gr.TimeLeft >= 0) {
					preRoundCountDownLabel.Text = MathF.Round(gr.TimeLeft).ToString();
					preRoundMenuLabel.Text = "PRESS " + Input.GetKeyWithBinding("+iv_duck").ToUpper() + " TO OPEN CHARACTER SELECT";
				} else {
					preRoundCountDownLabel.Text = "";
					preRoundMenuLabel.Text = "";
				}
			} else {
				preRoundCountDownLabel.Text = "";
				preRoundMenuLabel.Text = "";
			}

			if(SpeedDialGame.Instance.Round is GameRound || SpeedDialGame.Instance.Round is PreRound) {
				SetClass("active", false);
			} else {
				SetClass("active", true);
			}

			var player = Local.Pawn;
			if(player == null) return;
			if(player.ActiveChild is BaseSpeedDialWeapon weapon && weapon.AmmoClip >= 0) {
				if(weapon == null) return;
				outscale = outscale.LerpTo(1f, Time.Delta * 2f);
				clipLabel.Text = $"{weapon.AmmoClip}";
				
			} else {
				outscale = outscale.LerpTo(0f, Time.Delta * 8f);
				clipLabel.Text = "0";
				
			}

			var screenPos = player.EyePos.ToScreen();




			drugPanel.Style.Left = Length.Fraction(screenPos.x);
			drugPanel.Style.Top = Length.Fraction(screenPos.y);

			float f = Math.Clamp((MathF.Round((player as SpeedDialPlayer).TimeSinceMedTaken)) / (player as SpeedDialPlayer).MedDuration, 0f, 1f);
			if((player as SpeedDialPlayer).MedTaken == true && f >= 0.95f) {
				DrugType dt = (player as SpeedDialPlayer).CurrentDrug;
				switch(dt) {
					case DrugType.Ollie:
						drugImage.Texture = Texture.Load("materials/ui/ollie.png");
						break;
					case DrugType.Polvo:
						drugImage.Texture = Texture.Load("materials/ui/polvo.png");
						break;
					case DrugType.Ritindi:
						drugImage.Texture = Texture.Load("materials/ui/pill.png");

						break;
					case DrugType.Leaf:
						drugImage.Texture = Texture.Load("materials/ui/leaf.png");
						break;
				}
			}


			PanelTransform pt = new PanelTransform();
			if(f != old) {
				oldScale = 0.5f;
			}

			oldScale = oldScale.LerpTo(0, Time.Delta * 4f);

			pt.AddScale((1 - f) + oldScale);

			old = f;

			drugImage.Style.TransformOriginX = Length.Fraction(-0.0f);
			drugImage.Style.TransformOriginY = Length.Fraction(-1.0f);


			drugImage.Style.Transform = pt;

			drugImage.Style.Dirty();
			drugPanel.Style.Dirty();


		}
	}
}
