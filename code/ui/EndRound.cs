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
		public static EndRound Current;
		public PlayerPanel firstPlace;
		public PlayerPanel secondPlace;
		public PlayerPanel thirdPlace;
		private TimeSince animTime;
		private float[] scale = new float[3];
		public TimeSince time;
		public bool characterset;

		public void SetPlayers() {
			var players = SpeedDialGame.Instance.SortedPlayerList();

			firstPlace.myLabel.Text = players[0].Client.Name;
			firstPlace.subLabel.Text = "1st";
			firstPlace.scoreLabel.Text = players[0].KillScore.ToString() + " pts";

			firstPlace.myImage.Texture = Texture.Load(players[0].character.Portrait);

			if(players.Count > 1) {
				secondPlace.myLabel.Text = players[1].Client.Name;
				secondPlace.scoreLabel.Text = players[1].KillScore.ToString() + " pts";
				secondPlace.myImage.Texture = Texture.Load(players[1].character.Portrait);
				secondPlace.subLabel.Text = "2nd";
			}

			if(players.Count > 2) {
				thirdPlace.myLabel.Text = players[2].Client.Name;
				thirdPlace.scoreLabel.Text = players[2].KillScore.ToString() + " pts";
				thirdPlace.myImage.Texture = Texture.Load(players[2].character.Portrait);
				thirdPlace.subLabel.Text = "3rd";
			}
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

		public override void Tick() {
			base.Tick();

			// TODO: rpc?
			if(SpeedDialGame.Instance.Round is PostRound) {
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

				SetClass("active", true);

				if(!characterset) {
					SetPlayers();
					characterset = true;
				}
				PanelTransform first = new();
				first.AddTranslateY(Length.Percent(scale[0]));

				PanelTransform firstLabel = new();
				float anim = MathF.Sin(Time.Now * 4f);
				float anim2 = MathF.Sin(Time.Now * 2f);
				firstLabel.AddScale(1f + 0.1f * anim);
				firstLabel.AddRotation(0, 0, 15 * anim2);

				PanelTransform secondLabel = new();
				anim = MathF.Sin(Time.Now * 4f);
				anim2 = MathF.Sin(Time.Now * 2f);
				secondLabel.AddScale(1f + 0.05f * anim);
				secondLabel.AddRotation(0, 0, 2 * anim2);

				PanelTransform second = new();
				second.AddTranslateY(Length.Percent(scale[1]));

				PanelTransform third = new();
				third.AddTranslateY(Length.Percent(scale[2]));

				float startTime = 0.5f;

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
	}

	public class PlayerPanel : Panel {
		public Image myImage;
		public Label myLabel;
		public Label subLabel;
		public Label scoreLabel;
		public PlayerPanel() {
			myImage = Add.Image("materials/ui/portraits/default.png", "playerimage");
			myLabel = myImage.Add.Label("STEAM NAME", "name");
			subLabel = myLabel.Add.Label("1st", "sublabel");
			scoreLabel = myLabel.Add.Label("0000000", "scoreLabel");
		}
	}
}
