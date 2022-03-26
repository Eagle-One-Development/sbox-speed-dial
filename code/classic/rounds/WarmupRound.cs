using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.Rounds;

public partial class WarmupRound : Round
{
	private ClassicGamemode classic => Game.Current.ActiveGamemode as ClassicGamemode;
	public override string RoundText => $"Waiting for players... [{Client.All.Count}/{Game.MinPlayers}]";

	protected override void OnStart()
	{
		base.OnStart();

		classic.SetState( GamemodeState.Waiting );

		foreach ( var client in Client.All.Where( x => x.Pawn is ClassicPlayer ) )
		{
			var pawn = client.Pawn as ClassicPlayer;

			pawn.Frozen = false;
		}
	}

	protected override void OnThink()
	{
		base.OnThink();
		if ( Client.All.Count >= Game.MinPlayers )
		{
			Finish();
		}
	}

	protected override void OnFinish()
	{
		base.OnFinish();
		Game.Current.ActiveGamemode.ChangeRound( new PreRound() );
	}
}
