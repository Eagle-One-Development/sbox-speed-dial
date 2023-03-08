namespace SpeedDial.Classic.UI;

public partial class PlayerEntry
{
	public IClient Client;

	public Image Avatar { get; set; }
	public Label Score { get; set; }

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
		// globalizing like this so it's dots instead of commas cause it looks better with the font
		var scoreFormatted = string.Format( System.Globalization.CultureInfo.GetCultureInfo( "de-DE" ), "{0:#,##0}", Client.GetValue( "score", 0 ) );
		Score.Text = $"{scoreFormatted}";
		if ( Avatar.Texture is null )
		{
			Avatar.SetTexture( $"avatar:{Client.SteamId}" );
		}

		Score.SetClass( "me", Client == Game.LocalClient );
	}

	public virtual void UpdateFrom( IClient client )
	{
		Client = client;
		UpdateData();
	}
}
