using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Classic.Weapons;
using SpeedDial.Classic.Player;
using System.Collections.Generic;
using SpeedDial.Classic.Meds;


namespace SpeedDial.Classic.UI {
	public partial class GamePanel : Panel {
		public Panel ammoCounter;
		public Label clipLabel;

		public Panel pickUpPanel;
		public Label pickUpLabel;

		public Panel drugPanel;
		public Image drugImage;

		public Label screenEventLabel;
		public Label screenEventSubLabel;
		private TimeSince aTime = 0;
		public static GamePanel Current;

		private float outscale;
		public float pickedup;
		private float pickeduptar;

		private float scale;

		private float wideScale;
		private float screenEventScale;
		private float old;
		private float oldScale;
		public Label preRoundCountDownLabel;
		public Label preRoundMenuLabel;

		public List<Skull> dominators;

		public GamePanel() {
			StyleSheet.Load("/classic/ui/gamescreen/GamePanel.scss");
			ammoCounter = Add.Panel("counter");
			clipLabel = ammoCounter.Add.Label("0", "ammoLabel");

			pickUpPanel = Add.Panel("pickuppanel");
			pickUpLabel = pickUpPanel.Add.Label("Right Click To Pick Up", "pickuplabel");

			screenEventLabel = pickUpPanel.Add.Label("DRUG TAKEN", "screenevent");
			screenEventSubLabel = screenEventLabel.Add.Label("you feel better", "screeneventsub");

			Current = this;
			scale = 0;

			dominators = new();

			var panel = Add.Panel("countdown");
			preRoundCountDownLabel = panel.Add.Label("10", "timer");
			preRoundMenuLabel = panel.Add.Label("PRESS " + Input.GetKeyWithBinding("+iv_duck").ToUpper() + " TO OPEN CHARACTER SELECT", "char");

			drugPanel = Add.Panel("drug");
			drugImage = drugPanel.Add.Image("materials/ui/smile.png", "drugImage");

		}

		public void Bump() {
			scale = 0.7f;
		}

		[ClientRpc]
		public static void AddDominator(Entity entity) {
			Skull s = new(entity);
			Current.AddChild(s);
			Current.dominators.Add(s);
		}

		[ClientRpc]
		public static void RemoveDominator(Entity entity) {
			for(int i = 0; i < Current.dominators.Count; i++) {
				Skull s = Current.dominators[i];
				if(s.target == entity) {
					Current.dominators[i].DeleteChildren(true);
					Current.dominators[i].Delete(true);
					Current.dominators.RemoveAt(i);
					continue;
				}
			}
		}

		// used for drug pickups and "killed by" popup
		[ClientRpc]
		public static void ScreenEvent(string text, string flavorText, bool boolTaken) {
			Current.wideScale = 0.5f;
			Current.screenEventScale = 1.0f;
			Current.screenEventLabel.Text = text;
			if(boolTaken) {
				Current.screenEventLabel.Text += " TAKEN";
			}
			Current.screenEventSubLabel.Text = flavorText;
		}

		public override void Tick() {

			Shadow shadow_cyan = new();
			shadow_cyan.OffsetX = 2f + MathF.Sin(aTime * 2f) * 2f;
			shadow_cyan.OffsetY = 0f;
			shadow_cyan.Color = SpeedDialHud.VHS_CYAN;
			shadow_cyan.Blur = 1f;
			shadow_cyan.Spread = 20f;

			Shadow shadow_magenta = new();
			shadow_magenta.OffsetX = -2f + MathF.Sin(aTime * 2f) * 2f;
			shadow_magenta.OffsetY = 0;
			shadow_magenta.Color = SpeedDialHud.VHS_MAGENTA;
			shadow_magenta.Blur = 1f;
			shadow_magenta.Spread = 20f;

			ShadowList shadows = new();
			shadows.Add(shadow_cyan);
			shadows.Add(shadow_magenta);

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

			pickeduptar = pickeduptar.LerpTo(pickedup, Time.Delta * 8f);

			pickUpLabel.Style.Transform = transform2;
			pickUpLabel.Style.TextShadow = shadows;

			PanelTransform transform3 = new();
			transform3.AddScale(new Vector3(screenEventScale + wideScale, screenEventScale, screenEventScale));

			wideScale = wideScale.LerpTo(0, Time.Delta * 4f);
			if(wideScale <= 0.01f) {
				screenEventScale = screenEventScale.LerpTo(0, Time.Delta * 8f);
			}

			screenEventLabel.Style.Transform = transform3;
			screenEventLabel.Style.TextShadow = shadows;

			if(ClassicGamemode.Instance.Round is PreRound gr) {
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

			if(ClassicGamemode.Instance.Round is GameRound || ClassicGamemode.Instance.Round is PreRound) {
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

			for(int i = 0; i < dominators.Count; i++) {
				if(dominators[i] != null) {

					if(!dominators[i].target.IsValid()) {
						dominators[i].DeleteChildren(true);
						dominators[i].Delete(true);
						dominators.RemoveAt(i);
						continue;
					}
					var pos = dominators[i].target.EyePos.ToScreen();
					dominators[i].Style.Left = Length.Fraction(pos.x);
					dominators[i].Style.Top = Length.Fraction(pos.y);
				}
			}

			drugPanel.Style.Left = Length.Fraction(screenPos.x);
			drugPanel.Style.Top = Length.Fraction(screenPos.y);

			float f = Math.Clamp(MathF.Round((player as SpeedDialPlayer).TimeSinceMedTaken) / (player as SpeedDialPlayer).MedDuration, 0f, 1f);
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


			PanelTransform pt = new();
			if(f != old) {
				oldScale = 0.5f;
			}

			oldScale = oldScale.LerpTo(0, Time.Delta * 4f);

			pt.AddScale(1 - f + oldScale);

			old = f;

			drugImage.Style.TransformOriginX = Length.Fraction(-0.0f);
			drugImage.Style.TransformOriginY = Length.Fraction(-1.0f);

			drugImage.Style.Transform = pt;
		}
	}

	public class Skull : Panel {
		public Entity target;
		public Skull(Entity t) {
			Add.Image("materials/ui/skull.png", "icon");
			target = t;
		}
	}
}
