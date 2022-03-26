namespace SpeedDial.Classic.UI;

[UseTemplate]
public partial class WinScreen : Panel
{
	protected WinPanel FirstPanel { get; set; }
	protected WinPanel SecondPanel { get; set; }
	protected WinPanel ThirdPanel { get; set; }
	public static WinScreen Current { get; private set; }
	private bool Open { get; set; }

	public WinScreen()
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
		var clients = Client.All.ToList();
		clients.Sort( ( x, y ) => x.GetValue( "score", 0 ) < y.GetValue( "score", 0 ) ? 1 : -1 );
		Log.Debug( "Win Panel Update clients" );

		var client1 = clients.ElementAtOrDefault( 0 );
		Log.Debug( $"1st {client1?.Name} - {client1?.GetValue( "score", 0 )}" );
		FirstPanel.UpdateFrom( client1, 1 );

		var client2 = clients.ElementAtOrDefault( 1 );
		Log.Debug( $"2nd {client2?.Name} - {client2?.GetValue( "score", 0 )}" );
		SecondPanel.UpdateFrom( client2, 2 );

		var client3 = clients.ElementAtOrDefault( 2 );
		Log.Debug( $"3rd {client3?.Name} - {client3?.GetValue( "score", 0 )}" );
		ThirdPanel.UpdateFrom( client3, 3 );
	}

	[ClientRpc]
	public static void SetState( bool state )
	{
		if ( Current is null ) return;
		Current.Open = state;
	}

	public override void Tick()
	{
		base.Tick();
		SetClass( "open", Open );
		FirstPanel?.SetClass( "open", Open && FirstPanel.Client.IsValid() );
		SecondPanel?.SetClass( "open", Open && SecondPanel.Client.IsValid() );
		ThirdPanel?.SetClass( "open", Open && ThirdPanel.Client.IsValid() );
	}
}
