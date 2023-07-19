using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.Rounds;

public partial class WarmupRound : Round
{
	private ClassicGamemode classic => SDGame.Current.ActiveGamemode as ClassicGamemode;
	public override string RoundText => $"Waiting for players... [{Game.Clients.Count}/{SDGame.MinPlayers}]";

	protected override void OnStart()
	{
		base.OnStart();

		classic.SetState( GamemodeState.Waiting );

		foreach ( var client in Game.Clients.Where( x => x.Pawn is ClassicPlayer ) )
		{
			var pawn = client.Pawn as ClassicPlayer;

			pawn.Frozen = false;
		}
	}

	protected override void OnThink()
	{
		base.OnThink();
		if ( Game.Clients.Count >= SDGame.MinPlayers )
		{
			Finish();
		}
	}

	protected override void OnFinish()
	{
		base.OnFinish();
		SDGame.Current.ActiveGamemode.ChangeRound( new PreRound() );
	}
}
