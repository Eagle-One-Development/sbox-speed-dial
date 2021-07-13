using Sandbox;
using Sandbox.UI;

namespace SpeedDial.UI {
	[Library]
	public class SpeedDialHud : HudEntity<RootPanel> {

		[Net]
		public static SpeedDialScoreboard<SpeedDialScoreboardEntry> Scoreboard { get; private set; }
		public SpeedDialHud() {
			if(!IsClient)
				return;

			RootPanel.AddChild<GamePanel>();
			RootPanel.AddChild<DrugEffects>();
			RootPanel.AddChild<AmmoPanel>();
			RootPanel.AddChild<GameRoundPanel>();
			RootPanel.AddChild<ComboPanel>();
			RootPanel.AddChild<CharacterSelect>();
			RootPanel.AddChild<EndRound>();
			Scoreboard = RootPanel.AddChild<SpeedDialScoreboard<SpeedDialScoreboardEntry>>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<CrossHair>();
			RootPanel.AddChild<KillFeed>();
		}

		
	}
}
