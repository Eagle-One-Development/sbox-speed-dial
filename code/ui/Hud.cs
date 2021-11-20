using Sandbox;
using Sandbox.UI;
using SpeedDial.Player;

namespace SpeedDial.UI {
	[Library]
	public class SpeedDialHud : HudEntity<RootPanel> {

		public static Color VHS_MAGENTA = new Color(255f / 255f, 89 / 255f, 255f / 255f, 1.0f);
		public static Color VHS_CYAN = new Color(28f / 255f, 255f / 255f, 176f / 255f, 1.0f);

		[Net]
		public static SpeedDialScoreboard<SpeedDialScoreboardEntry> Scoreboard { get; private set; }
		public SpeedDialHud() {
			if(!IsClient)
				return;
			RootPanel.AddChild<GamePanel>();
			RootPanel.AddChild<DrugEffects>();
			RootPanel.AddChild<GameRoundPanel>();
			RootPanel.AddChild<ComboPanel>();
			RootPanel.AddChild<CharacterSelect>();
			RootPanel.AddChild<VotingScreen>();
			RootPanel.AddChild<EndRound>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<CrossHair>();
			Scoreboard = RootPanel.AddChild<SpeedDialScoreboard<SpeedDialScoreboardEntry>>();
		}
	}
}
