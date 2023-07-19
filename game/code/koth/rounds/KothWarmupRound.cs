using SpeedDial.Koth.Player;

namespace SpeedDial.Koth.Rounds;

public partial class KothWarmupRound : Round
{
	private KothGamemode koth => SDGame.Current.ActiveGamemode as KothGamemode;
	public override string RoundText => $"Waiting for players... [{Game.Clients.Count}/{SDGame.MinPlayers}]";

	protected override void OnStart()
	{
		base.OnStart();

		koth.SetState( GamemodeState.Waiting );

		foreach ( var client in Game.Clients.Where( x => x.Pawn is KothPlayer ) )
		{
			var pawn = client.Pawn as KothPlayer;

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
		SDGame.Current.ActiveGamemode.ChangeRound( new KothPreRound() );
	}
}
