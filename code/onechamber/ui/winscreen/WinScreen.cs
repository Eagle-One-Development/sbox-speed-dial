using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class OneChamberWinScreen : Panel
{
	protected OneChamberWinPanel FirstPanel { get; set; }
	public static OneChamberWinScreen Current { get; private set; }
	private bool Open { get; set; }

	public OneChamberWinScreen()
	{
		Current = this;
	}

	[ClientRpc]
	public static void UpdatePanels()
	{
		Current?.OnUpdate();
	}

	protected virtual void OnUpdate()
	{
		Log.Debug( "One Chamber Win Panel Update clients" );

		var pawn = Entity.All.OfType<OneChamberPlayer>().First( x => x.CanRespawn() );
		Log.Debug( $"winner {pawn.Client?.Name}" );
		FirstPanel.UpdateFrom( pawn.Client, 1 );
	}

	[ClientRpc]
	public static void SetState( bool state )
	{
		if ( Current is null ) return;
		Current.Open = state;
		Log.Debug( $"winscreen set state {state}" );
	}

	public override void Tick()
	{
		base.Tick();
		SetClass( "open", Open );
		FirstPanel?.SetClass( "open", Open && FirstPanel.Client.IsValid() );
	}
}
