using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.Rounds;
public partial class OneChamberWarmupRound : Round
{
	private OneChamberGamemode onechamber => Game.Current.ActiveGamemode as OneChamberGamemode;
	public override string RoundText => $"Waiting for players... [{Client.All.Count}/{Game.MinPlayers}]";

	protected override void OnStart()
	{
		base.OnStart();

		onechamber.SetState( GamemodeState.Waiting );

		foreach ( var client in Client.All.Where( x => x.Pawn is OneChamberPlayer ) )
		{
			var pawn = client.Pawn as OneChamberPlayer;

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
		Game.Current.ActiveGamemode.ChangeRound( new OneChamberPreRound() );
	}
}
