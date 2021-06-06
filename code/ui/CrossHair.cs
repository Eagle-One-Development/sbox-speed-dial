using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;
using SpeedDial.Player;
using System.Collections.Generic;
using SpeedDial.Player;
using SpeedDial.Weapons;



namespace SpeedDial.UI
{
	public partial class CrossHair : Panel
	{

		public Panel cross;

		public Panel[] hairs;

		public static CrossHair Current;

		public float bumpScale;

		public CrossHair()
		{
			hairs = new Panel[4];
			StyleSheet.Load( "/ui/CrossHair.scss" );
			cross = Add.Panel( "cross" );
			for (int i =0; i < 4; i++ )
			{
				hairs[i] = cross.Add.Panel( "hair" ); 
			}

			Current = this;
			
		}

		public void Bump()
		{
			bumpScale = 0f;
		}

		public override void Tick()
		{
			base.Tick();

			//SetClass( "inactive", false );
			SetClass( "active", Global.InGame);
			



			//Log.Info( $"X: {Mouse.Position.x} Y: {Mouse.Position.y} \b Screen Width: {Screen.Width}" );

			cross.Style.Left = Length.Fraction(Mouse.Position.x/Screen.Width);
			cross.Style.Top = Length.Fraction( Mouse.Position.y/Screen.Height);
			cross.Style.Dirty();

			float f = bumpScale;
			bumpScale = bumpScale.LerpTo( 1f, Time.Delta * 6f );

			for(int i = 0; i < 4; i++ )
			{
				PanelTransform pt = new PanelTransform();

				pt.AddRotation( 0, 0, i * 90f );

				float pixel = 18f + 20f * (1-f);
				if ( i == 0 )
				{
					pt.AddTranslateY( Length.Pixels( pixel ) );
				}

				if ( i == 1 )
				{
					pt.AddTranslateX( Length.Pixels( pixel ) );
				}

				if ( i == 2 )
				{
					pt.AddTranslateY( Length.Pixels( -pixel ) );
				}

				if ( i == 3 )
				{
					pt.AddTranslateX( Length.Pixels( -pixel ) );
				}

				if(Local.Pawn.ActiveChild == null )
				{
					hairs[i].SetClass( "inactive", true );
				}
				else
				{
					hairs[i].SetClass( "inactive", false );
				}


				hairs[i].Style.Transform = pt;
				hairs[i].Style.Dirty();
			}
		}
	}
}
