using SpeedDial.Classic.UI;

namespace SpeedDial.Koth.Entities;

public partial class HillSpot : ModelEntity
{

	public List<BasePlayer> TouchingPlayers = new();

	[Net, Predicted]
	public TimeSince TimeSinceAlive { get; set; }

	[SpeedDialEvent.Gamemode.Reset]
	public void HandleGamemodeReset( GamemodeIdentity ident )
	{
		if ( ident != GamemodeIdentity.Koth )
		{
			Delete();
		}
	}

	[Event.Tick]
	public void Tick()
	{
		if ( TimeSinceAlive > 10f )
		{
			foreach ( Client client in Client.All )
			{
				ScreenHints.FireEvent( To.Single( client ), "HILL MOVED", "Good luck!" );
			}

			if ( IsValid && !IsClient )
			{
				Delete();
			}
			return;
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/koth/ring.vmdl" );
		Transmit = TransmitType.Always;
		CollisionGroup = CollisionGroup.Trigger;
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		TimeSinceAlive = 0f;
	}

	public override void StartTouch( Entity other )
	{
		if ( other is BasePlayer player )
			TouchingPlayers.Add( player );
	}

	public override void EndTouch( Entity other )
	{
		if ( other is BasePlayer player )
			TouchingPlayers.Remove( player );
	}
}
