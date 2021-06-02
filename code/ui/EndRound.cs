using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Player;
using System.Linq;
using System.Collections.Generic;
namespace SpeedDial.UI
{
	public class EndRound : Panel
	{
		public Image render;
		
		
		AnimSceneObject firstAnim;
		SceneCapture sceneCapture;
		Angles CamAngles;
		Transform firstPlaceTransform;

		public static EndRound Current;
		public PlayerPanel firstPlace;
		public PlayerPanel secondPlace;
		public PlayerPanel thirdPlace;

		private TimeSince anim;
		private float[] scale  = new float[3];

		
		

		public void SetPlayers()
		{
			var players = SpeedDialGame.Instance.SortedPlayerList();

			//Log.Info( "------------------" );
			//int i = 0;
			//foreach(SpeedDialPlayer p in players )
			//{
			//	Log.Info( $"{p.GetClientOwner().Name} {p.KillScore} {i}" );
			//	i++;
			//}
			//Log.Info( "------------------" );

			firstPlace.myLabel.Text = players[0].GetClientOwner().Name;
			firstPlace.subLabel.Text = "1st";

			if(players.Count > 1 )
			{
				secondPlace.myLabel.Text = players[1].GetClientOwner().Name;
				secondPlace.subLabel.Text = "2nd";
			}

			if ( players.Count > 2 )
			{
				thirdPlace.myLabel.Text = players[2].GetClientOwner().Name;
				thirdPlace.subLabel.Text = "3rd";
			}

			
		}

		public EndRound()
		{
			StyleSheet.Load( "/ui/EndRound.scss" );
			firstPlace = AddChild<PlayerPanel>();
			secondPlace =AddChild<PlayerPanel>();
			secondPlace.myImage.SetClass( "right",true);
			thirdPlace = AddChild<PlayerPanel>();
			thirdPlace.myImage.SetClass( "left", true);
			Current = this;
			scale[0] = 100f;
			scale[1] = 100f;
			scale[2] = 100f;
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

		}

		public override void Tick()
		{
			base.Tick();
			if ( SpeedDialGame.Instance.Round is PostRound gr )
			{
				SetClass( "active", true );
				SetPlayers();
				PanelTransform first = new PanelTransform();
				first.AddTranslateY( Length.Percent( scale[0] ) );

				PanelTransform second = new PanelTransform();
				second.AddTranslateY( Length.Percent( scale[1] ) );

				PanelTransform third = new PanelTransform();
				third.AddTranslateY( Length.Percent( scale[2] ) );

				float startTime = 2f;

				if ( anim > startTime )
				{
					scale[0] = scale[0].LerpTo( 0, Time.Delta * 4f );
				}

				startTime += 1f;

				if ( anim > startTime && secondPlace.myLabel.Text != "STEAM NAME")
				{
					scale[1] = scale[1].LerpTo( 0, Time.Delta * 4f );
				}

				startTime += 1f;

				if ( anim > startTime && thirdPlace.myLabel.Text != "STEAM NAME" )
				{
					scale[2] = scale[2].LerpTo( 0, Time.Delta * 4f );
				}

				firstPlace.Style.Transform = first;
				firstPlace.Style.Dirty();

				secondPlace.Style.Transform = second;
				secondPlace.Style.Dirty();

				thirdPlace.Style.Transform = third;
				thirdPlace.Style.Dirty();

			}
			else
			{
				SetClass( "active", false );
				anim = 0;
			}
		}


		//Depreceated
		public void CreateWorld()
		{
			CamAngles = new Angles( 0, 0.0f, 0 );

			using ( SceneWorld.SetCurrent( new SceneWorld() ) )
			{
				firstPlaceTransform = new Transform( Vector3.Zero, Rotation.FromAxis( Vector3.Up, 180f ), 1f );
				Model m = (Local.Pawn as SpeedDialPlayer).GetModel();
			
				
				firstAnim = new AnimSceneObject( m, firstPlaceTransform );
				firstAnim.SetAnimParam( "b_grounded", true );
				firstAnim.MeshGroupMask = 0;

				Light.Point( Vector3.Up * 150.0f, 200.0f, Color.Red * 5000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Forward * 100.0f, 200, Color.White * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Backward * 100.0f, 200, Color.Magenta * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Right * 100.0f, 200, Color.Blue * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Left * 100.0f, 200, Color.Green * 15000.0f );


				sceneCapture = SceneCapture.Create( "test2", 1280, 720 );

				sceneCapture.SetCamera( Vector3.Up * 10 + CamAngles.Direction * -50, CamAngles, 90 );
			}
			render = Add.Image( "scene:test2" );
		}
	}

	public class PlayerPanel : Panel
	{
		public Image myImage;
		public Label myLabel;
		public Label subLabel;
		public PlayerPanel()
		{
			myImage = Add.Image( "materials/ui/portraits/default.png", "playerimage" );
			myLabel = myImage.Add.Label( "STEAM NAME", "name" );
			subLabel = myLabel.Add.Label( "1st", "sublabel" );
		}
	}
}
