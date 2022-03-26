using SpeedDial.Koth.Player;
using SpeedDial.Classic.Player;

namespace SpeedDial.Koth.Rounds;

public partial class KothPreRound : TimedRound
{
	public override TimeSpan RoundDuration => TimeSpan.FromSeconds( 11 );
	private KothGamemode classic => Game.Current.ActiveGamemode as KothGamemode;
	public override string RoundText => "Round starting...";

	protected override void OnStart()
	{
		base.OnStart();

		classic.SetState( GamemodeState.Preparing );

		if ( Host.IsServer )
		{
			classic.PickNewSoundtrack();
			// clear kills list to clear domination info
			classic.Kills.Clear();
		}

		foreach ( var client in Client.All.Where( x => x.Pawn is KothPlayer ) )
		{
			var pawn = client.Pawn as KothPlayer;
			pawn.Respawn();

			ClassicPlayer.StopSoundtrack( To.Single( client ), true );
			ClassicPlayer.PlaySoundtrack( To.Single( client ) );

			// reset scores etc from potential last round
			client.SetValue( "score", 0 );
			client.SetValue( "maxcombo", 0 );
			client.SetValue( "combo", 0 );

			pawn.Frozen = true;

			Log.Debug( "pre round" );
		}

		// reset stuff from warmup etc
		Game.Current.ActiveGamemode?.CallResetEvent();
	}

	protected override void OnFinish()
	{
		base.OnFinish();
		Game.Current.ActiveGamemode.ChangeRound( new KothGameRound() );
	}

	public override void OnPawnJoined( BasePlayer pawn )
	{
		base.OnPawnJoined( pawn );
		if ( pawn is KothPlayer player )
		{
			ClassicPlayer.PlaySoundtrack( To.Single( player.Client ) );
			player.Frozen = true;
		}
	}
}
