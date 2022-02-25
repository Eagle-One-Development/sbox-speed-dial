using SpeedDial.Classic.UI;
using SpeedDial.Classic.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public class OneChamberHud : ClassicHud {
	public override void Tick() {
		// cursor only shows when using mouse input and when not spectator
		SetClass("state-visible-cursor", !Input.UsingController || Local.Pawn is not ClassicSpectator);

		// game panels hide in post round
		SetClass("state-visible-game", !Gamemode.Instance.Ending && !Debug.UI);
	}
}
