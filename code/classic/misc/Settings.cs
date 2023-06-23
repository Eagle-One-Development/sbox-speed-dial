namespace SpeedDial.Classic;

using SpeedDial.Classic.Player;

public static partial class Settings
{
	[ConVar.Client( "sd_viewshift_toggle" )]
	public static bool ViewshiftToggle { get; set; } = false;

	// Music enable/disable
	[ConVar.Client( "sd_music" ), Change(nameof( OnMusicEnabledChanged ) )]
	public static bool MusicToggled { get; set; } = false;

	//public static bool MusicEnabled { get => _musicEnabled; set { OnMusicEnabledChanged( value ); _musicEnabled = value; } }
	//private static bool _musicEnabled = true;

	private static void OnMusicEnabledChanged( bool oldValue, bool newValue )
	{
		ClassicPlayer.StopSoundtrack( To.Single( Game.LocalClient ), newValue );
	}
}
