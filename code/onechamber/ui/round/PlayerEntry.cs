using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class OneChamberPlayerEntry : Panel
{
	public Client Client;

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
