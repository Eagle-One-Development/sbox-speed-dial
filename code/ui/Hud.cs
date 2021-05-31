using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

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
			RootPanel.AddChild<Sandbox.UI.Scoreboard<Sandbox.UI.ScoreboardEntry>>();
		}
	}
}
