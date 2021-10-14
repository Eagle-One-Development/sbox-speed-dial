
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace SpeedDial.UI {
	public partial class SpeedDialScoreboardEntry : Panel {
		public PlayerScore.Entry Entry;

		public Label PlayerName;
		public Label Score;
		public Label MaxCombo;

		Color vhs_green;
		Color vhs_magenta;
		int maxCombo = 0;
		float offset;

		public SpeedDialScoreboardEntry() {
			AddClass("entry");

			PlayerName = Add.Label("PlayerName", "name");
			Score = Add.Label("score", "score");
			MaxCombo = Add.Label("combo", "maxcombo");

			vhs_green = new Color(28f / 255f, 255f / 255f, 176f / 255f, 1.0f);//new Color(173f/255f,255f/255f,226f/255f,1.0f);
			vhs_magenta = new Color(255f / 255f, 89 / 255f, 255f / 255f, 1.0f);//new Color(255f / 255f, 163f / 255f, 255f / 255f, 1.0f);
			offset = Rand.Float(0, 100f);
		}

		public void FauxTick() {
			Shadow s1 = new();
			s1.OffsetX = 2f + MathF.Sin(Time.Now * 1f) * 1f;
			s1.OffsetY = 0f;
			s1.Color = vhs_green;
			s1.Blur = 1f;
			s1.Spread = 20f;

			Shadow s2 = new();
			s2.OffsetX = -1f + MathF.Sin(Time.Now * 2f) * 1f;
			s2.OffsetY = 0;
			s2.Color = vhs_magenta;
			s2.Blur = 2f;
			s2.Blur = 1f;
			s2.Spread = 20f;

			ShadowList shadows = new();
			shadows.Add(s1);
			shadows.Add(s2);


			float f = maxCombo / 7f;
			f = Math.Clamp(f, 0f, 1f);


			float anim = (MathF.Sin((Time.Now + offset) * 2f * (1 + f * 2f)) + 1) / 2;
			float anim2 = MathF.Sin((Time.Now + offset) * 1f * (1 + f * 2f));
			PanelTransform transform = new();


			transform.AddScale(((1.0f + anim * f * 0.4f) + (0.65f * f)));
			transform.AddRotation(0f, 0f, anim2 * 5f * f);

			MaxCombo.Style.Transform = transform;
			MaxCombo.Style.TextShadow = shadows;
			MaxCombo.Style.Dirty();



			MaxCombo.Style.TextShadow = shadows;
			MaxCombo.Style.Dirty();
		}

		public virtual void UpdateFrom(PlayerScore.Entry entry) {
			Entry = entry;

			PlayerName.Text = $"{entry.GetString("name")}";
			Score.Text = $"{entry.Get<int>("score", 0)} pts";
			MaxCombo.Text = $"x{entry.Get<int>("maxcombo", 0)}";
			maxCombo = entry.Get<int>("maxcombo", 0);

			SetClass("me", Local.Client != null && entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
		}
	}
}
