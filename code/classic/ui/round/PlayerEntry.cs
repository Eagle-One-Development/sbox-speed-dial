namespace SpeedDial.Classic.UI;

[UseTemplate]
public partial class PlayerEntry : Panel
{
	public Client Client;

	public Image Avatar { get; set; }
	public Label Score { get; set; }

	RealTimeSince TimeSinceUpdate = 0;

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
			Avatar.SetTexture( $"avatar:{Client.PlayerId}" );
		}

		Score.SetClass( "me", Client == Local.Client );
	}

	public virtual void UpdateFrom( Client client )
	{
		Client = client;
		UpdateData();
	}
}
