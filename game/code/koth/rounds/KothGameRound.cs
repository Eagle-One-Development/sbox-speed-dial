using SpeedDial.Koth.Player;
using SpeedDial.Classic.UI;
using SpeedDial.Koth.Entities;
using SpeedDial.Classic.Player;

namespace SpeedDial.Koth.Rounds;

public partial class KothGameRound : TimedRound
{
	public override TimeSpan RoundDuration => TimeSpan.FromMinutes( 5 );
	private KothGamemode koth => SDGame.Current.ActiveGamemode as KothGamemode;
	public override string RoundText => "";

	[Net]
	public int LastHillSpawnIdent { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		koth.SetState( GamemodeState.Running );

		foreach ( var client in Game.Clients.Where( x => x.Pawn is KothPlayer ) )
		{
			var pawn = client.Pawn as KothPlayer;

			pawn.Frozen = false;
		}



		// start climax track 10 seconds before round ends
		_ = PlayClimaxMusic( (int)RoundDuration.TotalSeconds - 10 );
	}

	public void CreateHill()
	{
		if ( Entity.All.OfType<HillSpotSpawn>().Any() )
		{
			var targetHill = Entity.All.OfType<HillSpotSpawn>().Where( x => x.NetworkIdent != LastHillSpawnIdent ).Random();

			var hill = new HillSpot();
			hill.Position = targetHill.Position;
			hill.Rotation = targetHill.Rotation;
			hill.Scale = targetHill.Scale;
			LastHillSpawnIdent = targetHill.NetworkIdent;

		}
	}

	protected override void OnFinish()
	{
		base.OnFinish();

		WinScreen.UpdatePanels( To.Everyone );
		SDGame.Current.ActiveGamemode.ChangeRound( new KothPostRound() );
	}


	protected override void OnThink()
	{
		base.OnThink();

		if ( !Entity.All.OfType<HillSpot>().Any() )
		{

			CreateHill();

		}
	}

	protected void ServerTick()
	{



	}

	private async Task PlayClimaxMusic( int delay )
	{
		await GameTask.DelaySeconds( delay );
		foreach ( var client in Game.Clients.Where( x => x.Pawn is KothPlayer ) )
		{
			var pawn = client.Pawn as KothPlayer;
			ClassicPlayer.PlayRoundendClimax( To.Single( client ) );
		}
	}

	public override void OnPawnJoined( BasePlayer pawn )
	{
		base.OnPawnJoined( pawn );
		if ( pawn is KothPlayer player )
		{
			ClassicPlayer.PlaySoundtrack( To.Single( player.Client ) );
		}
	}
}
