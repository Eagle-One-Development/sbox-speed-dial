using SpeedDial.OneChamber.Player;
using SpeedDial.Classic.UI;
using SpeedDial.OneChamber.UI;

namespace SpeedDial.OneChamber.Rounds;

public partial class OneChamberPostRound : TimedRound
{
	public override TimeSpan RoundDuration => TimeSpan.FromSeconds( 11 );
	private OneChamberGamemode onechamber => SDGame.Current.ActiveGamemode as OneChamberGamemode;
	public override string RoundText => "";

	protected override void OnStart()
	{
		base.OnStart();

		onechamber.SetState( GamemodeState.Ending );

		foreach ( var client in Game.Clients.Where( x => x.Pawn is OneChamberPlayer ) )
		{
			var pawn = client.Pawn as OneChamberPlayer;

			pawn.Frozen = true;
		}
		CharacterSelect.ForceState( To.Everyone, false );
		OneChamberWinScreen.SetState( To.Everyone, true );
	}

	protected override void OnFinish()
	{
		base.OnFinish();
		// tell the game that a gameloop has finished before we keep going
		SDGame.Current.GameloopCompleted();

		SDGame.Current.ActiveGamemode?.ChangeRound( new OneChamberPreRound() );

		OneChamberWinScreen.SetState( To.Everyone, false );
	}

	public override void OnPawnJoined( BasePlayer pawn )
	{
		base.OnPawnJoined( pawn );
		if ( pawn is OneChamberPlayer player )
		{
			player.Frozen = true;
		}
	}
}
