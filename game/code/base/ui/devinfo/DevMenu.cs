namespace SpeedDial;

public partial class DevMenu
{
	public override void Tick()
	{
		SetClass( "open", Game.LocalClient.IsListenServerHost && Input.Down( "View" ) );
	}
}
