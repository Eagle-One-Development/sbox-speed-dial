
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
		

		Color vhs_green;
		Color vhs_magenta;

		public WorldScore(int amt, Vector3 pos, Panel parent) {
			Parent = parent;
			position = pos;

			lb = Add.Label($"{amt}", "label");
			screenPosition = position.ToScreen();

			Style.Left = Length.Fraction(screenPosition.x);
			Style.Top = Length.Fraction(screenPosition.y);
			Style.Dirty();

			vhs_green = new Color( 28f / 255f, 255f / 255f, 176f / 255f, 1.0f );//new Color(173f/255f,255f/255f,226f/255f,1.0f);
			vhs_magenta = new Color( 255f / 255f, 89 / 255f, 255f / 255f, 1.0f );//new Color(255f / 255f, 163f / 255f, 255f / 255f, 1.0f);

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
			vhs_green = new Color( 28f / 255f, 255f / 255f, 176f / 255f, 1.0f );//new Color(173f/255f,255f/255f,226f/255f,1.0f);
			vhs_magenta = new Color( 255f / 255f, 89 / 255f, 255f / 255f, 1.0f );//new Color(255f / 255f, 163f / 255f, 255f / 255f, 1.0f);
		}

		public override void Tick() {
			base.Tick();

			Shadow s1 = new();
			s1.OffsetX = 2f + MathF.Sin( Time.Now * 2f ) * 2f;
			s1.OffsetY = 0f;
			s1.Color = vhs_green;
			s1.Blur = 1f;
			s1.Spread = 20f;

			Shadow s2 = new();
			s2.OffsetX = -2f + MathF.Sin( Time.Now * 2f ) * 2f;
			s2.OffsetY = 0;
			s2.Color = vhs_magenta;
			s2.Blur = 1f;
			s2.Spread = 20f;

			ShadowList shadows = new();
			shadows.Add( s1 );
			shadows.Add( s2 );

			

			UpdateNumberPosition();
			if ( quip == String.Empty )
			{
				lb.Text = $"{amount}pts";
			}
			else
			{
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

			float f = Math.Clamp(deathTime / 0.25f, 0, 1f);
			float f2 = Math.Clamp(lifetime / life, 0, 1f);
			float emptyMod = 1f;
			float quipMod = 0f;

			if(quip != String.Empty )
			{
				emptyMod = 0;
				quipMod = 1f;
			}


			var transform = new PanelTransform();
			transform.AddTranslateX(Length.Pixels(-200));
			if ( quip == String.Empty )
			{
				transform.AddTranslateY( Length.Pixels( -100 - 25 * EaseOutCubic( f2 ) ) );
			}
			else
			{
				transform.AddTranslateY( Length.Pixels( -100 - 100 * EaseOutCubic( f2 ) ) );
			}

			transform.AddScale(scale);
			transform.AddRotation(0, 0, ang * emptyMod);
			ang = ang.LerpTo(tarAng, Time.Delta * 2f);


			Style.Transform = transform;

			float lifeActual = life * (1 + 0.5f * quipMod);

			if(lifetime > lifeActual ) {
				Style.Opacity = 1 - (lifetime - lifeActual) / lifeActual;
				if ( quip == string.Empty )
				{
					scale = scale.LerpTo( 0, Time.Delta * 3f );
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
