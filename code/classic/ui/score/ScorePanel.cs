using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class ScorePanel : Panel {
		public static ScorePanel Current;
		private readonly Panel Score;
		private readonly Panel Combo;
		private readonly Label ScoreLabel;
		private readonly Label ComboLabel;
		private readonly Panel IntensityPanel;
		private float ComboScale = 1;
		private float _combo = 0;
		private float _score = 0;

		[ClientRpc]
		public static void AwardKill() {
			Current.ComboScale += 0.2f;
		}

		public ScorePanel() {
			Current = this;

			StyleSheet.Load("/classic/ui/score/ScorePanel.scss");

			// TODO: put this in it's own thing to make sure it's behind stuff?
			IntensityPanel = Add.Panel("intensity");
			IntensityPanel.Add.Panel("right");
			IntensityPanel.Add.Panel("left");

			var pan = Add.Panel("scoreCombo");
			Score = pan.Add.Panel("score");
			ScoreLabel = Score.Add.Label("0 PTS", "scorelabel");

			Combo = pan.Add.Panel("combo");
			ComboLabel = Combo.Add.Label("x0", "combolabel");
		}

		public override void Tick() {
			// score
			{
				var score = Local.Client.GetValue("score", 0);
				//_score = _score.LerpTo(score, Time.Delta * 7f);
				//ScoreLabel.Text = $"{(int)(_score + 0.5f)} PTS";

				// globalizing like this so it's dots instead of commas cause it looks better with the font
				var scoreFormatted = string.Format(System.Globalization.CultureInfo.GetCultureInfo("de-DE"), "{0:#,##0}", score);
				ScoreLabel.Text = $"{scoreFormatted} PTS";
			}

			// combo
			{
				var combo = Local.Client.GetValue("combo", 0);
				_combo = _combo.LerpTo(combo, Time.Delta * 10f);
				ComboLabel.Text = $"x{(int)(_combo + 0.5f)}";

				// intensity gradients at the left and right side
				IntensityPanel.Style.Opacity = _combo.Clamp(0, 25) * 0.02f; // 25 * 0.04 = 0.5

				// combo text scaling + rotating for intensity that scales with combo amount
				PanelTransform transform = new();
				// clamp it so it doesn't go ape mode on higher combos
				var _effectMultiplier = combo.Clamp(0, 25);
				transform.AddScale(ComboScale + 0.05f * _combo + ((MathF.Sin(Time.Now * _effectMultiplier) + 1) / 2 * _effectMultiplier * 0.05f));
				transform.AddRotation(0, 0, MathF.Sin(Time.Now * _effectMultiplier * 1.234f) * _effectMultiplier * 0.5f);
				ComboLabel.Style.Transform = transform;

				if(combo <= 0) {
					ComboScale = ComboScale.LerpTo(0, Time.Delta * 3f);
					return;
				}

				ComboScale = ComboScale.LerpTo(1, Time.Delta * 7f);
			}
		}
	}
}
