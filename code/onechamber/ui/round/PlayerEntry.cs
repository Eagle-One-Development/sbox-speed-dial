using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class OneChamberPlayerEntry : Panel
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
		if ( Client.Pawn is OneChamberPlayer player )
		{
			Score.Text = $"{player.Lives}";
		}
		else
		{
			Score.Text = $"-";
		}

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
