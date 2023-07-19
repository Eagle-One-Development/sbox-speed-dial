using SpeedDial.Classic.Player;

namespace SpeedDial.OneChamber.UI;

public partial class OneChamberHud
{
	public override void Tick()
	{
		// cursor only shows when using mouse input and when not spectator
		SetClass( "state-visible-cursor", !Input.UsingController || Game.LocalPawn is not ClassicSpectator );

		// game panels hide in post round
		SetClass( "state-visible-game", !Gamemode.Instance.Ending && !Debug.UI );
	}
}