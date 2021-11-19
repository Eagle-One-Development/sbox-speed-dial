using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;
using SpeedDial.Player;
using System.Collections.Generic;


namespace SpeedDial.UI {
	public partial class ComboPanel : Panel {

		public Label scoreLabel;
		public Label comboLabel;
		public Panel scoreContainer;
		public Panel comboContainer;
		private float scoreTar;
		private float scalemod;
		private float comboTar;
		private float endScaleTar;
		public static ComboPanel Current;
		public Panel worldScorePanel;
		private List<WorldScore> scores = new();

		public ComboPanel() {
			StyleSheet.Load("/ui/ComboPanel.scss");

			scoreContainer = Add.Panel("container");
			comboContainer = Add.Panel("comboContainer");

			comboLabel = comboContainer.Add.Label("x999", "combo");
			scoreLabel = scoreContainer.Add.Label("0000000", "score");
			Current = this;
			comboTar = 0f;

			worldScorePanel = Add.Panel("worldscoreparent");

			for(int i = 0; i < 5; i++) {
				WorldScore ws = AddChild<WorldScore>();
				scores.Add(ws);
			}
		}

		public void OnKill(Vector3 pos, int amt, COD death, int combo) {
			WorldScore ws = null;
			for(int i = 0; i < scores.Count; i++) {
				WorldScore temp = scores[i];
				if(temp.lifetime > temp.life) {
					ws = temp;
					continue;
				}
			}

			if(ws != null) {
				ws.position = pos;
				ws.amount = amt;
				ws.lifetime = 0;
				ws.ang = 0;
				ws.quip = String.Empty;
				ws.tarAng = Rand.Float(-15f, 15f);
			}

			float chance = Rand.Float(1);
			float chanceMod = 0.35f;

			if(chance > chanceMod || combo < 2) {
				return;
			}

			string[] baseQuips = { "RADICAL", "DISGUSTING", "HARD RESET", "OUCH", "SICK", "EXCELLENT", "POPPIN' OFF", "CAPPED", "DECEASED", "AN EARLY GRAVE" };

			string quip = baseQuips[Rand.Int(baseQuips.Length - 1)];

			string[] combos = { "DOUBLE KILL", "TRIPLE KILL", "QUAD KILL", "KILLIONAIRE", "KILL BILL" };

			if(combo >= 2 && combo < combos.Length) {
				quip = combos[combo - 2];
			}

			switch(death) {
				case COD.Melee:
					quip = "BRUTAL";
					break;
				case COD.Thrown:
					quip = "BAD CATCH";
					break;
				case COD.Explosive:
					quip = "BOMB";
					break;
			}

			//This is for the qup
			ws = null;
			for(int i = 0; i < scores.Count; i++) {
				WorldScore temp = scores[i];
				if(temp.lifetime > temp.life) {
					ws = temp;
					continue;
				}
			}

			if(ws != null) {
				ws.position = pos;
				ws.amount = amt;
				ws.lifetime = 0;
				ws.ang = 0;
				ws.quip = quip;
			}
		}

		public void Bump() {
			scalemod = 2.0f;
			comboTar++;
		}

		public override void Tick() {

			foreach(WorldScore w in scores) {
				w.Tick();
			}

			Shadow shadow_cyan = new();
			shadow_cyan.OffsetX = 2f + MathF.Sin(Time.Now * 2f) * 2f;
			shadow_cyan.OffsetY = 0f;
			shadow_cyan.Color = SpeedDialHud.VHS_CYAN;
			shadow_cyan.Blur = 4f;
			shadow_cyan.Blur = 1f;
			shadow_cyan.Spread = 20f;

			Shadow shadow_magenta = new();
			shadow_magenta.OffsetX = -2f + MathF.Sin(Time.Now * 2f) * 2f;
			shadow_magenta.OffsetY = 0;
			shadow_magenta.Color = SpeedDialHud.VHS_MAGENTA;
			shadow_magenta.Blur = 4f;
			shadow_magenta.Blur = 1f;
			shadow_magenta.Spread = 20f;

			ShadowList shadows = new();
			shadows.Add(shadow_cyan);
			shadows.Add(shadow_magenta);

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

				comboTar = comboTar.LerpTo(p.KillCombo, Time.Delta * 10f);

				c = (int)MathF.Round(comboTar);

				comboLabel.Text = $"x{c}";

				if(c <= 0) {
					endScaleTar = endScaleTar.LerpTo(0, Time.Delta * 8f);
				} else {
					endScaleTar = endScaleTar.LerpTo(1, Time.Delta * 8f);
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
		}
	}
}
