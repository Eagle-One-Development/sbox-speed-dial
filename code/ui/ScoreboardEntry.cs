
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

		public SpeedDialScoreboardEntry() {
			AddClass("entry");

			PlayerName = Add.Label("PlayerName", "name");
			Score = Add.Label("", "score");
			MaxCombo = Add.Label("", "maxcombo");
		}

		public virtual void UpdateFrom(PlayerScore.Entry entry) {
			Entry = entry;

			PlayerName.Text = entry.GetString("name");
			Score.Text = entry.Get<int>("score", 0).ToString();
			MaxCombo.Text = entry.Get<int>("maxcombo", 0).ToString();

			SetClass("me", Local.Client != null && entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
		}
	}
}
