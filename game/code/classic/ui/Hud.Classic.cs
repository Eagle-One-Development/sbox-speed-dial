using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI;

public partial class ClassicHud
{
	public static Color VHS_MAGENTA = new( 255f / 255f, 43f / 255f, 112f / 255f, 1.0f );
	public static Color VHS_CYAN = new( 0f / 255f, 210f / 255f, 255f / 255f, 1.0f );

	public ClassicHud()
	{
		if ( !Game.IsClient ) return;
		AddClass( "root" );
	}

	public override void Tick()
	{
		base.Tick();
		// game panels hide in post round
		SetClass( "state-visible-game", !Gamemode.Instance.Ending && !Debug.UI );
		// cursor only shows when using mouse input
		SetClass( "state-visible-cursor", !Input.UsingController && !Debug.UI && Game.LocalPawn is not ClassicSpectator );
	}
}
