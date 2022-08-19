namespace SpeedDial.Classic.UI;

[UseTemplate]
public partial class ClassicScoreboard : Panel
{
	private readonly Dictionary<Client, ClassicScoreboardEntry> Rows = new();

	public Panel Header { get; set; }
	public Panel Canvas { get; set; }
	public Panel Footer { get; set; }
	public Label GamemodeInfo { get; set; }
	public Label MapInfo { get; set; }
	public Label LoopInfo { get; set; }

	public ClassicScoreboard()
	{
		BindClass( "open", () => Input.Down( InputButton.Score ) );
	}

	public override void Tick()
	{
		if ( !IsVisible )
			return;

		GamemodeInfo.Text = $"Gamemode: {Game.Current.ActiveGamemode.ClassName}";
		MapInfo.Text = $"Map: {Global.MapName}";
		LoopInfo.Text = $"Games until vote: {Game.Current.ActiveGamemode.GameloopsUntilVote - Game.Current.CompletedGameloops}";
		Footer.SetClass( "visible", true );

		// Clients that joined
		foreach ( var client in Client.All.Except( Rows.Keys ) )
		{
			var entry = AddClient( client );
			Rows[client] = entry;
		}

		// clients that left
		foreach ( var client in Rows.Keys.Except( Client.All ) )
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

	protected virtual ClassicScoreboardEntry AddClient( Client entry )
	{
		var p = Canvas.AddChild<ClassicScoreboardEntry>();
		p.Client = entry;
		return p;
	}
}
