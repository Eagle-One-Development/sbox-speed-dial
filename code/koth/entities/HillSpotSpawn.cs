namespace SpeedDial.Koth.Entities;

[Library( "sd_koth_hill_spawn" ), Title( "Random Hill Site Spawn" ), Category( "Gameplay" ), HammerEntity]
[EditorModel( "models/koth/ring.vmdl" )]
public partial class HillSpotSpawn : GamemodeEntity
{
	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	[SpeedDialEvent.Gamemode.Reset]
	public void HandleGamemodeReset( GamemodeIdentity ident )
	{
		if ( ident == GamemodeIdentity.Koth )
		{
			Enable();
		}
		else
		{
			Disable();
		}
	}
}
