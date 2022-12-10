using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class OneChamberPlayerEntry : Panel
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
		Score.Text = Client.Pawn is OneChamberPlayer player ? $"{player.Lives}" : $"-";

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
