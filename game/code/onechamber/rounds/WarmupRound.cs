using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.Rounds;
public partial class OneChamberWarmupRound : Round
{
	private OneChamberGamemode onechamber => SDGame.Current.ActiveGamemode as OneChamberGamemode;
	public override string RoundText => $"Waiting for players... [{Game.Clients.Count}/{SDGame.MinPlayers}]";

	protected override void OnStart()
	{
		base.OnStart();

		onechamber.SetState( GamemodeState.Waiting );

		foreach ( var client in Game.Clients.Where( x => x.Pawn is OneChamberPlayer ) )
		{
			var pawn = client.Pawn as OneChamberPlayer;

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
		SDGame.Current.ActiveGamemode.ChangeRound( new OneChamberPreRound() );
	}
}
