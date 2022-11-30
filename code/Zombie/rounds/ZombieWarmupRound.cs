using System;
using SpeedDial.Zombie.Player;

namespace SpeedDial.Zombie.Rounds;

public class ZombieWarmupRound : ZombieRound
{

	public override string RoundText => $"Waiting for players... [{Client.All.Count}/{Game.MinPlayers}]";

	protected override void OnStart()
	{
		base.OnStart();

		GameMode.SetState( GamemodeState.Waiting );

		foreach ( var client in Client.All.Where( x => x.Pawn is ZombiePlayer ) )
		{
			var pawn = client.Pawn as ZombiePlayer;

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
		Game.Current.ActiveGamemode.ChangeRound( new ZombiePreRound() );
	}
}
