using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;

namespace SpeedDial.UI {
	public class AmmoPanel : Panel {
		public Panel ammoCounter;
		public Label ammoLabel;
		public Label clipLabel;
		public Panel pickUpPanel;
		public Label pickUpLabel;
		public Label medLabel;
		public Label medFlavor;
		private TimeSince aTime;
		public static AmmoPanel Current;

		Color vhs_green;
		Color vhs_magenta;

		private float outscale;
		public float pickedup;
		private float pickeduptar;

		private float scale;

		private float wideScale;
		private float totalDrugScale;

		public AmmoPanel() {
			StyleSheet.Load("/ui/AmmoPanel.scss");
			ammoCounter = Add.Panel("counter");
			clipLabel = ammoCounter.Add.Label("000", "ammoLabel");

			pickUpPanel = Add.Panel("pickuppanel");
			pickUpLabel = pickUpPanel.Add.Label("Right Click To Pick Up", "pickuplabel");

			medLabel = pickUpPanel.Add.Label( "DRUG TAKEN", "medlabel" );
			medFlavor = medLabel.Add.Label( "you feel better", "medFlavor" );

			Current = this;
			scale = 0;

			vhs_green = new Color(28f / 255f, 255f / 255f, 176f / 255f, 1.0f);//new Color(173f/255f,255f/255f,226f/255f,1.0f);
			vhs_magenta = new Color(255f / 255f, 89 / 255f, 255f / 255f, 1.0f);//new Color(255f / 255f, 163f / 255f, 255f / 255f, 1.0f);
		}

		public void Bump() {
			scale = 0.7f;
		}
		

		
		public void DrugBump(string s, string f)
		{
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
			transform3.AddScale( new Vector3( totalDrugScale + wideScale, totalDrugScale, totalDrugScale ) );

			wideScale = wideScale.LerpTo( 0, Time.Delta * 4f );
			if(wideScale <= 0.01f )
			{
				totalDrugScale = totalDrugScale.LerpTo( 0, Time.Delta * 8f);
			}

			medLabel.Style.Transform = transform3;
			medLabel.Style.TextShadow = shadows;
			medLabel.Style.Dirty();



			var player = Local.Pawn;
			if(player == null) return;
			if(player.ActiveChild is BaseSpeedDialWeapon weapon) {
				if(weapon == null) return;
				outscale = outscale.LerpTo(1f, Time.Delta * 4f);
				clipLabel.Text = $"{weapon.AmmoClip}";
			} else {
				outscale = outscale.LerpTo(0f, Time.Delta * 8f);
				clipLabel.Text = "00";
			}
		}
	}
}
