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
			AddClass("scorepanel");
			Score = Add.Panel("score");
			ScoreLabel = Score.Add.Label("0 PTS", "scorelabel");

			Combo = Add.Panel("combo");
			ComboLabel = Combo.Add.Label("x0", "combolabel");
		}

		public override void Tick() {
			var score = Local.Client.GetValue("score", 0);
			_score = _score.LerpTo(score, Time.Delta * 7f);
			//ScoreLabel.Text = $"{(int)(_score + 0.5f)} PTS";
			ScoreLabel.Text = $"{score} PTS";

			var combo = Local.Client.GetValue("combo", 0);
			_combo = _combo.LerpTo(combo, Time.Delta * 10f);
			ComboLabel.Text = $"x{(int)(_combo + 0.5f)}";

			// combo text scaling + rotating for intensity that scales with combo amount
			PanelTransform transform = new();
			// clamp it so it doesn't go ape mode on higher combos
			var _effectMultiplier = combo.Clamp(0, 25);
			transform.AddScale(ComboScale + 0.05f * _combo + ((MathF.Sin(Time.Now * _effectMultiplier) + 1) / 2 * combo * 0.05f));
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
