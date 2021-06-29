using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Player;
using System.Linq;
using System.Collections.Generic;
namespace SpeedDial.UI {
	public class EndRound : Panel {
		public Image render;


		AnimSceneObject firstAnim;
		//SceneCapture sceneCapture;
		Angles CamAngles;
		Transform firstPlaceTransform;

		public static EndRound Current;
		public PlayerPanel firstPlace;
		public PlayerPanel secondPlace;
		public PlayerPanel thirdPlace;

		private TimeSince animTime;
		private float[] scale = new float[3];

		Color vhs_green;
		Color vhs_magenta;

		public TimeSince time;

		public bool characterset;

		public void SetPlayers() {
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


			firstPlace.myImage.Texture = Texture.Load(players[0].character.Portrait);



			if(players.Count > 1) {
				secondPlace.myLabel.Text = players[1].GetClientOwner().Name;
				secondPlace.myImage.Texture = Texture.Load(players[1].character.Portrait);
				secondPlace.subLabel.Text = "2nd";
			}

			if(players.Count > 2) {
				thirdPlace.myLabel.Text = players[2].GetClientOwner().Name;
				thirdPlace.myImage.Texture = Texture.Load(players[2].character.Portrait);
				thirdPlace.subLabel.Text = "3rd";
			}

			vhs_green = new Color(28f / 255f, 255f / 255f, 176f / 255f, 1.0f);//new Color(173f/255f,255f/255f,226f/255f,1.0f);
			vhs_magenta = new Color(255f / 255f, 89 / 255f, 255f / 255f, 1.0f);//new Color(255f / 255f, 163f / 255f, 255f / 255f, 1.0f);

		}

		public EndRound() {
			StyleSheet.Load("/ui/EndRound.scss");
			firstPlace = AddChild<PlayerPanel>();
			secondPlace = AddChild<PlayerPanel>();
			secondPlace.myImage.SetClass("right", true);
			secondPlace.subLabel.SetClass("right", true);
			thirdPlace = AddChild<PlayerPanel>();
			thirdPlace.myImage.SetClass("left", true);
			thirdPlace.subLabel.SetClass("left", true);
			Current = this;
			scale[0] = 100f;
			scale[1] = 100f;
			scale[2] = 100f;
		}

		public override void OnDeleted() {
			base.OnDeleted();

		}

		public override void Tick() {
			base.Tick();
			if(SpeedDialGame.Instance.Round is PostRound gr) {
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

				SetClass("active", true);

				if(!characterset) {
					SetPlayers();
					characterset = true;
				}
				PanelTransform first = new PanelTransform();
				first.AddTranslateY(Length.Percent(scale[0]));

				PanelTransform firstLabel = new PanelTransform();
				float anim = MathF.Sin(Time.Now * 4f);
				float anim2 = MathF.Sin(Time.Now * 2f);
				firstLabel.AddScale(1f + 0.1f * anim);
				firstLabel.AddRotation(0, 0, 15 * anim2);

				PanelTransform secondLabel = new PanelTransform();
				anim = MathF.Sin(Time.Now * 4f);
				anim2 = MathF.Sin(Time.Now * 2f);
				secondLabel.AddScale(1f + 0.05f * anim);
				secondLabel.AddRotation(0, 0, 2 * anim2);

				PanelTransform second = new PanelTransform();
				second.AddTranslateY(Length.Percent(scale[1]));


				PanelTransform third = new PanelTransform();
				third.AddTranslateY(Length.Percent(scale[2]));

				float startTime = 2f;

				if(animTime > startTime) {
					scale[0] = scale[0].LerpTo(0, Time.Delta * 4f);
				}



				startTime += 1f;

				if(animTime > startTime && secondPlace.myLabel.Text != "STEAM NAME") {
					scale[1] = scale[1].LerpTo(0, Time.Delta * 4f);
				}

				startTime += 1f;

				if(animTime > startTime && thirdPlace.myLabel.Text != "STEAM NAME") {
					scale[2] = scale[2].LerpTo(0, Time.Delta * 4f);
				}

				firstPlace.Style.Transform = first;
				firstPlace.Style.TextShadow = shadows;
				firstPlace.Style.Dirty();


				time = 0f;

				firstPlace.subLabel.Style.Transform = firstLabel;
				firstPlace.subLabel.Style.Dirty();

				secondPlace.Style.Transform = second;
				secondPlace.Style.TextShadow = shadows;
				secondPlace.Style.Dirty();

				secondPlace.subLabel.Style.Transform = secondLabel;
				secondPlace.subLabel.Style.Dirty();

				thirdPlace.Style.Transform = third;
				thirdPlace.Style.TextShadow = shadows;
				thirdPlace.Style.Dirty();

			} else {
				SetClass("active", false);
				animTime = 0;
				scale[0] = 100f;
				scale[1] = 100f;
				scale[2] = 100f;
				characterset = false;
				if(time > 2f) {
					secondPlace.myLabel.Text = "STEAM NAME";
					thirdPlace.myLabel.Text = "STEAM NAME";
				}
			}
		}


		//Depreceated
		public void CreateWorld() {
			CamAngles = new Angles(0, 0.0f, 0);

			using(SceneWorld.SetCurrent(new SceneWorld())) {
				firstPlaceTransform = new Transform(Vector3.Zero, Rotation.FromAxis(Vector3.Up, 180f), 1f);
				Model m = (Local.Pawn as SpeedDialPlayer).GetModel();


				firstAnim = new AnimSceneObject(m, firstPlaceTransform);
				firstAnim.SetAnimParam("b_grounded", true);
				firstAnim.MeshGroupMask = 0;

				Light.Point(Vector3.Up * 150.0f, 200.0f, Color.Red * 5000.0f);
				Light.Point(Vector3.Up * 10.0f + Vector3.Forward * 100.0f, 200, Color.White * 15000.0f);
				Light.Point(Vector3.Up * 10.0f + Vector3.Backward * 100.0f, 200, Color.Magenta * 15000.0f);
				Light.Point(Vector3.Up * 10.0f + Vector3.Right * 100.0f, 200, Color.Blue * 15000.0f);
				Light.Point(Vector3.Up * 10.0f + Vector3.Left * 100.0f, 200, Color.Green * 15000.0f);


				//sceneCapture = SceneCapture.Create( "test2", 1280, 720 );

				//sceneCapture.SetCamera( Vector3.Up * 10 + CamAngles.Direction * -50, CamAngles, 90 );
			}
			render = Add.Image("scene:test2");
		}
	}

	public class PlayerPanel : Panel {
		public Image myImage;
		public Label myLabel;
		public Label subLabel;
		public PlayerPanel() {
			myImage = Add.Image("materials/ui/portraits/default.png", "playerimage");
			myLabel = myImage.Add.Label("STEAM NAME", "name");
			subLabel = myLabel.Add.Label("1st", "sublabel");
		}
	}
}
