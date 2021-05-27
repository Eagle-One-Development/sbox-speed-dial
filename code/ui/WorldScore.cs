
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;
using SpeedDial.Player;
using System.Collections.Generic;


namespace SpeedDial.UI {
	public partial class WorldScore : Panel
		{
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

			public WorldScore(int amt, Vector3 pos, Panel parent){
				Parent = parent;
				position = pos;
				
				lb = Add.Label( amt.ToString() , "label");
				screenPosition = position.ToScreen();

				Style.Left = Length.Fraction( screenPosition.x );
				Style.Top = Length.Fraction( screenPosition.y );
				Style.Dirty();

				life = 1f;
				lifetime = 0;
				UpdateNumberPosition();

			}

			public WorldScore(){
				lb = Add.Label( "0" , "label");
				amount = 0;
				position = Vector3.Zero;
				screenPosition = Vector2.Zero;
				life = 1f;
			}

			public override void Tick(){
				base.Tick();
				
				UpdateNumberPosition();
				lb.Text = amount.ToString() + "pts";


				screenPosition = position.ToScreen();
				Style.Left = Length.Fraction( screenPosition.x );
				Style.Top = Length.Fraction( screenPosition.y );
				Style.Dirty();
			}

			public void UpdateNumberPosition(){

				float f = Math.Clamp(deathTime/0.25f,0,1f);
				float f2 = Math.Clamp(lifetime/life,0,1f);

				var transform = new PanelTransform();
				transform.AddTranslateX(Length.Pixels(-200));
				transform.AddTranslateY(Length.Pixels(-100 - 25 * EaseOutCubic(f2)));
				
				transform.AddScale(scale);
				transform.AddRotation(0,0,ang);
				ang = ang.LerpTo(tarAng, Time.Delta * 2f);
				

				Style.Transform = transform;

				if(lifetime > life){
					Style.Opacity = (1-(lifetime-life)/life);
					scale = scale.LerpTo(0, Time.Delta * 3f);
				}else{
					Style.Opacity = 1;
					deathTime = 0;
					scale = scale.LerpTo(1, Time.Delta * 10f);
				}	


			}

			private float EaseOutCubic(float x){
				return 1 - MathF.Pow(1 - x, 3);
			}
		}
}
