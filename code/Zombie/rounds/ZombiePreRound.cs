using System;
using SpeedDial.Zombie.Player;

namespace SpeedDial.Zombie.Rounds;

public partial class ZombiePreRound : TimedRound
{
	public override TimeSpan RoundDuration => TimeSpan.FromSeconds( 11 );
	private ZombieGamemode classic => Game.Current.ActiveGamemode as ZombieGamemode;
	public override string RoundText => "Round starting...";

	protected override void OnStart()
	{
		base.OnStart();

		classic.SetState( GamemodeState.Preparing );

		if ( Host.IsServer )
		{
			//classic.PickNewSoundtrack();
			// clear kills list to clear domination info
			//classic.Kills.Clear();
		}

		foreach ( var client in Client.All.Where( x => x.Pawn is ZombiePlayer ) )
		{
			var pawn = client.Pawn as ZombiePlayer;
			pawn.Respawn();

			//Classic.Player.ClassicPlayer.StopSoundtrack( To.Single( client ), true );
			//Classic.Player.ClassicPlayer.PlaySoundtrack( To.Single( client ) );


			pawn.Frozen = true;

			Log.Debug( "pre round" );
		}

		// reset stuff from warmup etc
		Game.Current.ActiveGamemode?.CallResetEvent();
	}

	protected override void OnFinish()
	{
		base.OnFinish();
		Game.Current.ActiveGamemode?.ChangeRound( new ZombieGameRound() );
	}

	public override void OnPawnJoined( BasePlayer pawn )
	{
		base.OnPawnJoined( pawn );
		if ( pawn is ZombiePlayer player )
		{
			ZombiePlayer.PlaySoundtrack( To.Single( player.Client ) );
			player.Frozen = true;
		}
	}
}
