using Sandbox;
using Sandbox.UI;

namespace SpeedDial.UI {
	[Library]
	public class SpeedDialHud : HudEntity<RootPanel> {
		public SpeedDialHud() {
			if(!IsClient)
				return;

			RootPanel.AddChild<GamePanel>();
			RootPanel.AddChild<AmmoPanel>();
			RootPanel.AddChild<GameRoundPanel>();
			RootPanel.AddChild<CharacterSelect>();
			RootPanel.AddChild<ComboPanel>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<SpeedDialScoreboard<SpeedDialScoreboardEntry>>();
		}
	}
}
