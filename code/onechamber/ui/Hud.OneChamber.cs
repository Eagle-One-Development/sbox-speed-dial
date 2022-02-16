using Sandbox;
using Sandbox.UI;

using SpeedDial.Classic.Rounds;

using SpeedDial.Classic;
using SpeedDial.Classic.UI;
using SpeedDial.Classic.Player;

using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI {
	[UseTemplate]
	public class OneChamberHud : ClassicHud {
		public override void Tick() {
			// cursor only shows when using mouse input and when not spectator
			SetClass("state-visible-cursor", !Input.UsingController || Local.Pawn is not ClassicSpectator);
		}
	}
}
