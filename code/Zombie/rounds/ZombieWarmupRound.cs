using System;
using SpeedDial.Zombie.Player;

namespace SpeedDial.Zombie.Rounds;

public class ZombieWarmupRound : ZombieRound
{

	public override string RoundText => $"Waiting for players... [{Game.Clients.Count}/{SDGame.MinPlayers}]";

	protected override void OnStart()
	{
		base.OnStart();

		GameMode.SetState( GamemodeState.Waiting );

		foreach ( var client in Game.Clients.Where( x => x.Pawn is ZombiePlayer ) )
		{
			var pawn = client.Pawn as ZombiePlayer;

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
		SDGame.Current.ActiveGamemode.ChangeRound( new ZombiePreRound() );
	}
}
