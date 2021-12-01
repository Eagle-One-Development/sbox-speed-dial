using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class PlayerEntry : Panel {
		public Client Client;
		private readonly Image Avatar;
		private readonly Label Score;

		public PlayerEntry() {
			AddClass("player");

			Avatar = Add.Image($"", "avatar");
			Score = Add.Label("000", "score");
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
			// globalizing like this so it's dots instead of commas cause it looks better with the font
			var scoreFormatted = string.Format(System.Globalization.CultureInfo.GetCultureInfo("de-DE"), "{0:#,##0}", Client.GetValue("score", 0));
			Score.Text = $"{scoreFormatted}";
			if(Avatar.Texture is null) {
				Avatar.SetTexture($"avatar:{Client.PlayerId}");
			}

			Score.SetClass("me", Client == Local.Client);
		}

		public virtual void UpdateFrom(Client client) {
			Client = client;
			UpdateData();
		}
	}
}
