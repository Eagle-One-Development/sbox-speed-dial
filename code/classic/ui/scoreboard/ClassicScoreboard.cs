namespace SpeedDial.Classic.UI;

public partial class ClassicScoreboard
{
	[SkipHotload]
	private readonly Dictionary<IClient, ClassicScoreboardEntry> Rows = new();

	public Panel Header { get; set; }
	public Panel Canvas { get; set; }
	public Panel Footer { get; set; }
	public Label GamemodeInfo { get; set; }
	public Label MapInfo { get; set; }
	public Label LoopInfo { get; set; }

	public override void Tick()
	{
		SetClass( "open", Input.Down( "Score" ) );

		if ( !IsVisible )
			return;

		GamemodeInfo.Text = $"Gamemode: {SDGame.Current.ActiveGamemode.ClassName}";
		MapInfo.Text = $"Map: {Game.Server.MapIdent}";
		LoopInfo.Text = $"Games until vote: {SDGame.Current.ActiveGamemode.GameloopsUntilVote - SDGame.Current.CompletedGameloops}";
		Footer.SetClass( "visible", true );

		// Clients that joined
		foreach ( var client in Game.Clients.Except( Rows.Keys ) )
		{
			var entry = AddClient( client );
			Rows[client] = entry;
		}

		// clients that left
		foreach ( var client in Rows.Keys.Except( Game.Clients ) )
		{
			if ( Rows.TryGetValue( client, out var row ) )
			{
				row?.Delete();
				Rows.Remove( client );
			}
		}

		Canvas.SortChildren( ( x ) => -(x as ClassicScoreboardEntry).Client.GetValue( "score", 0 ) );

		for ( int i = 0; i < Canvas.Children.Count(); i++ )
		{
			var child = Canvas.Children.ElementAt( i );
			child.SetClass( "first", i == 0 && (child as ClassicScoreboardEntry).Client.GetValue( "score", 0 ) > 0 );
		}
	}

	protected virtual ClassicScoreboardEntry AddClient( IClient entry )
	{
		var p = Canvas.AddChild<ClassicScoreboardEntry>();
		p.Client = entry;
		return p;
	}
}
