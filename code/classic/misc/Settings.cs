namespace SpeedDial.Classic;

using SpeedDial.Classic.Player;

public static partial class Settings {
	[ClientVar("sd_viewshift_toggle")]
	public static bool ViewshiftToggle { get; set; } = false;

	// Music enable/disable
	[ClientVar("sd_music")]
	public static bool MusicEnabled { get { return _musicEnabled; } set { OnMusicEnabledChanged(value); _musicEnabled = value; } }
	private static bool _musicEnabled = true;

	private static void OnMusicEnabledChanged(bool value) {
		if(!value) {
			ClassicPlayer.StopSoundtrack(To.Single(Local.Client), true);
		}
	}
}
