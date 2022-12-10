namespace SpeedDial.Classic.UI;

[UseTemplate]
public partial class ClassicScoreboardEntry : Panel
{
	public IClient Client;

	public Label PlayerName { get; set; }
	public Label Score { get; set; }
	public Label MaxCombo { get; set; }
	public Image InputMethod { get; set; }
	public Label Ping { get; set; }

	private RealTimeSince TimeSinceUpdate = 0;

	public override void Tick()
	{
		base.Tick();

		if ( !IsVisible )
			return;

		if ( !Client.IsValid() )
			return;

		if ( TimeSinceUpdate < 0.1f )
			return;

		TimeSinceUpdate = 0;
		UpdateData();
	}

	public virtual void UpdateData()
	{
		PlayerName.Text = Client.Name;
		// globalizing like this so it's dots instead of commas cause it looks better with the font
		var scoreFormatted = string.Format( System.Globalization.CultureInfo.GetCultureInfo( "de-DE" ), "{0:#,##0}", Client.GetValue( "score", 0 ) );
		Score.Text = $"{scoreFormatted}";
		MaxCombo.Text = $"{Client.GetValue( "maxcombo", 0 )}";

		Ping.Text = $"{Client.Ping}ms";
		Ping.SetClass( "hidden", !Input.Down( InputButton.Walk ) || Client.IsBot );

		SetClass( "me", Client == Game.LocalClient && Game.Clients.Count > 1 );
		InputMethod.SetClass( "hidden", !Input.UsingController || Client.IsBot );
	}

	public virtual void UpdateFrom( IClient client )
	{
		Client = client;
		UpdateData();
	}
}
