using SpeedDial.OneChamber.Player;
using SpeedDial.Classic.Player;

namespace SpeedDial.OneChamber.Rounds;

public partial class OneChamberPreRound : TimedRound
{
	public override TimeSpan RoundDuration => TimeSpan.FromSeconds( 11 );
	private OneChamberGamemode onechamber => SDGame.Current.ActiveGamemode as OneChamberGamemode;
	public override string RoundText => "Round starting...";

	protected override void OnStart()
	{
		base.OnStart();

		onechamber.SetState( GamemodeState.Preparing );

		if ( Game.IsServer )
		{
			onechamber.PickNewSoundtrack();
			// clear kills list to clear domination info
			onechamber.Kills.Clear();
		}

		foreach ( var client in Game.Clients )
		{
			client.AssignPawn<OneChamberPlayer>();

			var pawn = client.Pawn as OneChamberPlayer;

			ClassicPlayer.StopSoundtrack( To.Single( client ), true );
			ClassicPlayer.PlaySoundtrack( To.Single( client ) );

			pawn.Frozen = true;

			Log.Debug( "pre round" );
		}

		// reset stuff from warmup etc
		SDGame.Current.ActiveGamemode?.CallResetEvent();
	}

	protected override void OnFinish()
	{
		base.OnFinish();
		SDGame.Current.ActiveGamemode?.ChangeRound( new OneChamberGameRound() );
	}

	public override void OnPawnJoined( BasePlayer pawn )
	{
		base.OnPawnJoined( pawn );
		if ( pawn is OneChamberPlayer player )
		{
			ClassicPlayer.PlaySoundtrack( To.Single( player.Client ) );
			player.Frozen = true;
		}
	}
}
