using Sandbox;
using Sandbox.UI;

using SpeedDial.Classic.Player;
using SpeedDial.Base.UI;

namespace SpeedDial.Classic.UI {
	[Library]
	public class SpeedDialHud : BaseHud {

		public static Color VHS_MAGENTA = new Color(255f / 255f, 89 / 255f, 255f / 255f, 1.0f);
		public static Color VHS_CYAN = new Color(28f / 255f, 255f / 255f, 176f / 255f, 1.0f);

		[Net]
		public static SpeedDialScoreboard<SpeedDialScoreboardEntry> Scoreboard { get; private set; }
		public SpeedDialHud() : base() {
			if(!IsClient)
				return;
			RootPanel.AddChild<GamePanel>();
			RootPanel.AddChild<DrugEffects>();
			RootPanel.AddChild<GameRoundPanel>();
			RootPanel.AddChild<ComboPanel>();
			RootPanel.AddChild<CharacterSelect>();
			RootPanel.AddChild<VotingScreen>();
			RootPanel.AddChild<EndRound>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<CrossHair>();
			Scoreboard = RootPanel.AddChild<SpeedDialScoreboard<SpeedDialScoreboardEntry>>();
		}
	}
}
