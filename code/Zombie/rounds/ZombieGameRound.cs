using SpeedDial.Zombie.Player;

namespace SpeedDial.Zombie.Rounds;

public class ZombieGameRound : ZombieRound
{
	public TimeSince RoundTime { get; set; }

	protected override void OnStart()
	{
		base.OnStart();
		GameMode.SetState( GamemodeState.Running );
		RoundTime = 0;


		foreach ( var client in Client.All.Where( x => x.Pawn is ZombiePlayer ) )
		{
			var pawn = client.Pawn as ZombiePlayer;

			pawn.Frozen = false;
		}
	}

	protected override void OnThink()
	{
		base.OnThink();
		//TODO: add round logic here
		/* if ( RoundTime > 300 )
		{

			Finish();
		} */
	}
	protected override void OnFinish()
	{
		base.OnFinish();
		Game.Current.ActiveGamemode?.ChangeRound( new ZombiePreRound() );
	}
}
