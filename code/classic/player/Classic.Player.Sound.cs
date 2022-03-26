using SpeedDial.Classic.GameSound;

namespace SpeedDial.Classic.Player;

public partial class ClassicPlayer
{
	public static SoundTrack SoundTrack { get; set; }
	public static bool SoundtrackPlaying { get; set; }

	[ClientRpc]
	public static void PlayRoundendClimax()
	{
		if ( !Settings.MusicEnabled ) return;
		SoundTrack.FromScreen( "climax" );
		_ = StopSoundtrackAsync( 3 );
	}

	private static async Task StopSoundtrackAsync( int delay = 5 )
	{
		await GameTask.DelaySeconds( delay );
		_ = SoundTrack.Stop( 5, 500 );
		SoundtrackPlaying = false;
	}


	[ClientRpc]
	public static void PlaySoundtrack()
	{
		if ( !Settings.MusicEnabled ) return;
		_ = PlaySoundtrackAsync( ClassicGamemode.Current.CurrentSoundtrack, 2.5f );
	}

	private static async Task PlaySoundtrackAsync( string track, float delay )
	{
		if ( !Settings.MusicEnabled ) return;
		await GameTask.DelaySeconds( delay );
		if ( !SoundtrackPlaying )
		{
			SoundTrack?.Stop();
			SoundTrack = SoundTrack.FromScreen( track );
			SoundtrackPlaying = true;
		}
	}

	[ClientRpc]
	public static void StopSoundtrack( bool instant = false )
	{
		if ( instant )
		{
			SoundTrack?.Stop();
			SoundtrackPlaying = false;
		}
		else
		{
			SoundTrack?.Stop( 1 );
			SoundtrackPlaying = false;
		}
	}

	[ClientRpc]
	public static void FadeSoundtrack( float volumeTo )
	{
		SoundTrack?.FadeVolumeTo( volumeTo );
	}
}
