namespace SpeedDial.Classic.UI;

public partial class RoundPanel : Panel
{

	public Panel Timer { get; set; }
	public Label TimeLabel { get; set; }
	public Panel TopPlayers { get; set; }
	public Label RoundText { get; set; }

	private readonly Dictionary<IClient, PlayerEntry> Players = new();

	public override void Tick()
	{
		var gamemode = SDGame.Current.ActiveGamemode;
		if ( gamemode is not null && gamemode.ActiveRound is not null )
		{
			if ( gamemode.ActiveRound is TimedRound timedRound )
			{
				TimeLabel.Text = $"-{timedRound.TimeLeftFormatted}";
			}
			else if ( gamemode.ActiveRound is Round round )
			{
				TimeLabel.Text = $"+{round.TimeElapsedFormatted}";
			}

			RoundText.Text = $"{gamemode.ActiveRound.RoundText}";
		}

		// Clients that joined
		foreach ( var client in Game.Clients.Except( Players.Keys ) )
		{
			var entry = AddClient( client );
			Players[client] = entry;
		}

		// clients that left
		foreach ( var client in Players.Keys.Except( Game.Clients ) )
		{
			if ( Players.TryGetValue( client, out var row ) )
			{
				row?.Delete();
				Players.Remove( client );
			}
		}

		TopPlayers.SortChildren( ( x ) => -(x as PlayerEntry).Client.GetValue( "score", 0 ) );

		for ( int i = 0; i < TopPlayers.Children.Count(); i++ )
		{
			var child = TopPlayers.Children.ElementAt( i );
			// Only show top five and only show if there's more than one player connected
			child.SetClass( "hidden", i > 4 || Game.Clients.Count <= 1 );
		}
	}

	protected virtual PlayerEntry AddClient( IClient entry )
	{
		var p = TopPlayers.AddChild<PlayerEntry>();
		p.Client = entry;
		return p;
	}
}
