using SpeedDial.Classic.Player;
using SpeedDial.Classic.UI;

namespace SpeedDial.Koth.Rounds;

public partial class KothPostRound : TimedRound
{
	public override TimeSpan RoundDuration => TimeSpan.FromSeconds( 11 );
	private KothGamemode classic => SDGame.Current.ActiveGamemode as KothGamemode;
	public override string RoundText => "";

	protected override void OnStart()
	{
		base.OnStart();

		classic.SetState( GamemodeState.Ending );

		foreach ( var client in Game.Clients.Where( x => x.Pawn is ClassicPlayer ) )
		{
			var pawn = client.Pawn as ClassicPlayer;

			pawn.Frozen = true;
		}
		CharacterSelect.ForceState( To.Everyone, false );
		WinScreen.SetState( To.Everyone, true );
	}

	protected override void OnFinish()
	{
		base.OnFinish();
		// tell the game that a gameloop has finished before we keep going
		SDGame.Current.GameloopCompleted();

		SDGame.Current.ActiveGamemode?.ChangeRound( new KothPreRound() );

		WinScreen.SetState( To.Everyone, false );
	}

	public override void OnPawnJoined( BasePlayer pawn )
	{
		base.OnPawnJoined( pawn );
		if ( pawn is ClassicPlayer player )
		{
			player.Frozen = true;
		}
	}
}
