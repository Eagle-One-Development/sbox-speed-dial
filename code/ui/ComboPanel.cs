using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;
using SpeedDial.Player;



namespace SpeedDial.UI {
	public partial class ComboPanel : Panel {

		public Label scoreLabel;
		public Label comboLabel;

		public Panel scoreContainer;

		public Panel comboContainer;

		Color vhs_green;
		Color vhs_magenta;

		private float scoreTar;

		private float scalemod;

		private float comboTar;

		private float endScaleTar;

		public static ComboPanel Current;

		public ComboPanel() {
			StyleSheet.Load("/ui/ComboPanel.scss");

			scoreContainer = Add.Panel("container");
			comboContainer = Add.Panel("comboContainer");

			comboLabel = comboContainer.Add.Label("x999", "combo");
			scoreLabel = scoreContainer.Add.Label("0000000", "score");
			vhs_green = new Color(28f / 255f, 255f / 255f, 176f / 255f, 1.0f);//new Color(173f/255f,255f/255f,226f/255f,1.0f);
			vhs_magenta = new Color(255f / 255f, 89 / 255f, 255f / 255f, 1.0f);//new Color(255f / 255f, 163f / 255f, 255f / 255f, 1.0f);
			Current = this;
			comboTar = 0f;

		}

		public void Bump() {
			scalemod = 2.0f;
			comboTar++;
		}

		public override void Tick() {
			Shadow s1 = new();
			s1.OffsetX = 2f + MathF.Sin(Time.Now * 2f) * 2f;
			s1.OffsetY = 0f;
			s1.Color = vhs_green;
			s1.Blur = 4f;

			Shadow s2 = new();
			s2.OffsetX = -2f + MathF.Sin(Time.Now * 2f) * 2f;
			s2.OffsetY = 0;
			s2.Color = vhs_magenta;
			s2.Blur = 4f;

			ShadowList shadows = new();
			shadows.Add(s1);
			shadows.Add(s2);

			float anim = (MathF.Sin(Time.Now * 2f) + 1) / 2;
			float anim2 = MathF.Sin(Time.Now * 1f);
			var transform = new PanelTransform();

			var comboTransform = new PanelTransform();

			float f = 0;
			float k = 0;
			int c = 0;
			if(Local.Pawn is SpeedDialPlayer p) {
				scoreTar = scoreTar.LerpTo(p.KillScore, Time.Delta * 5f);
				scoreLabel.Text = $"{(int)MathF.Round(scoreTar)} pts";


				comboTar = comboTar.LerpTo(p.KillCombo, Time.Delta * 6f);

				c = (int)MathF.Round(comboTar);

				comboLabel.Text = $"x{c}";

				if(c <= 0) {
					endScaleTar = endScaleTar.LerpTo(0,Time.Delta * 8f);
				}else{
					endScaleTar = endScaleTar.LerpTo(1,Time.Delta * 8f);
				}

				f = p.TimeSinceMurdered / SpeedDialGame.ComboTime;
				f = Math.Clamp(f, 0, 1);
				k = c / 7f;
				k = Math.Clamp(k, 0, 1);

			}

			transform.AddScale(0.8f + anim * 0.2f);
			transform.AddRotation(0f, 0f, anim2 * 5f);

			scalemod = scalemod.LerpTo(0, Time.Delta * 8f);

			comboTransform.AddScale((1 + 0.5f * (1 - f) + k * 1.0f + scalemod) * endScaleTar);
			comboTransform.AddRotation(0f, 0f, ((1 - f) * 15f) + k * MathF.Sin(Time.Now * 3f) * 20f);

			comboLabel.Style.TextShadow = shadows;
			scoreLabel.Style.TextShadow = shadows;
			scoreLabel.Style.Transform = transform;
			comboLabel.Style.Transform = comboTransform;

			comboLabel.Style.Dirty();
			scoreLabel.Style.Dirty();

		}
	}
}
