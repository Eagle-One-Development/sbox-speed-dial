using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class OneChamberScoreboardEntry : Panel
{
	public Client Client;

	public Label PlayerName { get; set; }
	public Label Lives { get; set; }
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
		if ( Client.Pawn is OneChamberPlayer player )
		{
			Lives.Text = $"{player.Lives}";
			SetClass( "dead", false );
		}
		else
		{
			Lives.Text = "-";
			SetClass( "dead", true );
		}

		Ping.Text = $"{Client.Ping}ms";
		Ping.SetClass( "hidden", !Input.Down( InputButton.Walk ) || Client.IsBot );

		SetClass( "me", Client == Local.Client && Client.All.Count > 1 );
		InputMethod.SetClass( "hidden", !Input.UsingController || Client.IsBot );
	}

	public virtual void UpdateFrom( Client client )
	{
		Client = client;
		UpdateData();
	}
}
