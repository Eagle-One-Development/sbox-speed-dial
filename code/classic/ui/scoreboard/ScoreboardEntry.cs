using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class ClassicScoreboardEntry : Panel {
		public Client Client;
		public Label PlayerName;
		public Label Score;
		public Label MaxCombo;

		public ClassicScoreboardEntry() {
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
			// globalizing like this so it's dots instead of commas cause it looks better with the font
			var scoreFormatted = string.Format(System.Globalization.CultureInfo.GetCultureInfo("de-DE"), "{0:#,##0}", Client.GetValue("score", 0));
			Score.Text = $"{scoreFormatted}";
			MaxCombo.Text = $"{Client.GetValue("maxcombo", 0)}";

			SetClass("me", Client == Local.Client && Client.All.Count > 1);
		}

		public virtual void UpdateFrom(Client client) {
			Client = client;
			UpdateData();
		}
	}
}
