
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;
using SpeedDial.Player;
using System.Collections.Generic;


namespace SpeedDial.UI {
	public partial class WorldScore : Panel {
		public Vector3 position;
		public Vector2 screenPosition;
		public float life;
		public TimeSince lifetime;
		public Label lb;
		public int amount;
		public TimeSince deathTime;
		private float scale;
		public float ang;
		public float tarAng;
		public string quip = String.Empty;

		public WorldScore(int amt, Vector3 pos, Panel parent) {
			Parent = parent;
			position = pos;

			lb = Add.Label($"{amt}", "label");
			screenPosition = position.ToScreen();

			Style.Left = Length.Fraction(screenPosition.x);
			Style.Top = Length.Fraction(screenPosition.y);
			Style.Dirty();

			life = 1f;
			lifetime = 0;
			UpdateNumberPosition();
		}

		public WorldScore() {
			lb = Add.Label("0", "scoreLabel");
			amount = 0;
			position = Vector3.Zero;
			screenPosition = Vector2.Zero;
			life = 1f;
		}

		public override void Tick() {
			base.Tick();

			Shadow shadow_cyan = new();
			shadow_cyan.OffsetX = 2f + MathF.Sin(Time.Now * 2f) * 2f;
			shadow_cyan.OffsetY = 0f;
			shadow_cyan.Color = SpeedDialHud.VHS_CYAN;
			shadow_cyan.Blur = 1f;
			shadow_cyan.Spread = 20f;

			Shadow shadow_magenta = new();
			shadow_magenta.OffsetX = -2f + MathF.Sin(Time.Now * 2f) * 2f;
			shadow_magenta.OffsetY = 0;
			shadow_magenta.Color = SpeedDialHud.VHS_MAGENTA;
			shadow_magenta.Blur = 1f;
			shadow_magenta.Spread = 20f;

			ShadowList shadows = new();
			shadows.Add(shadow_cyan);
			shadows.Add(shadow_magenta);

			UpdateNumberPosition();
			if(quip == String.Empty) {
				lb.Text = $"{amount}pts";
			} else {
				lb.Text = quip;
			}

			lb.Style.TextShadow = shadows;
			lb.Style.Dirty();

			screenPosition = position.ToScreen();
			Style.Left = Length.Fraction(screenPosition.x);
			Style.Top = Length.Fraction(screenPosition.y);
			Style.Dirty();
		}

		public void UpdateNumberPosition() {

			float f2 = Math.Clamp(lifetime / life, 0, 1f);
			float emptyMod = 1f;
			float quipMod = 0f;

			if(quip != String.Empty) {
				emptyMod = 0;
				quipMod = 1f;
			}

			var transform = new PanelTransform();
			transform.AddTranslateX(Length.Pixels(-200));
			if(quip == String.Empty) {
				transform.AddTranslateY(Length.Pixels(-100 - 25 * EaseOutCubic(f2)));
			} else {
				transform.AddTranslateY(Length.Pixels(-100 - 100 * EaseOutCubic(f2)));
			}

			transform.AddScale(scale);
			transform.AddRotation(0, 0, ang * emptyMod);
			ang = ang.LerpTo(tarAng, Time.Delta * 2f);

			Style.Transform = transform;

			float lifeActual = life * (1 + 0.5f * quipMod);

			if(lifetime > lifeActual) {
				Style.Opacity = 1 - (lifetime - lifeActual) / lifeActual;
				if(quip == string.Empty) {
					scale = scale.LerpTo(0, Time.Delta * 3f);
				}
			} else {
				Style.Opacity = 1;
				deathTime = 0;
				scale = scale.LerpTo(1, Time.Delta * 10f);
			}
		}

		private float EaseOutCubic(float x) {
			return 1 - MathF.Pow(1 - x, 3);
		}
	}
}
