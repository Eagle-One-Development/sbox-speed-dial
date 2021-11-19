
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace Sandbox.UI {
	public partial class SpeedDialScoreboardEntry : Panel {
		public Client Client;

		public Label PlayerName;
		public Label Score;
		public Label MaxCombo;

		public SpeedDialScoreboardEntry() {
			AddClass("entry");

			PlayerName = Add.Label("PlayerName", "name");
			Score = Add.Label("", "score");
			MaxCombo = Add.Label("", "maxcombo");
		}

		RealTimeSince TimeSinceUpdate = 0;

		public override void Tick() {
			base.Tick();

			if(!IsVisible)
				return;

			if(!Client.IsValid())
				return;

			if(TimeSinceUpdate < 0.1f)
				return;

			TimeSinceUpdate = 0;
			UpdateData();
		}

		public virtual void UpdateData() {
			PlayerName.Text = Client.Name;
			Score.Text = $"{Client.GetValue("score", 0)}";
			MaxCombo.Text = $"{Client.GetValue("maxcombo", 0)}";

			SetClass("me", Client == Local.Client);
		}

		public virtual void UpdateFrom(Client client) {
			Client = client;
			UpdateData();
		}
	}
}
