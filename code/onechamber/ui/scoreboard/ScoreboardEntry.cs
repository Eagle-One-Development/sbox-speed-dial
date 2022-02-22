using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class OneChamberScoreboardEntry : Panel {
	public Client Client;

	public Label PlayerName { get; set; }
	public Label Lives { get; set; }

	RealTimeSince TimeSinceUpdate = 0;

	public override void Tick() {
		base.Tick();

		if(!IsVisible)
			return;

		if(!Client.IsValid())
			return;

		if(TimeSinceUpdate < 0.1f)
			return;

		TimeSinceUpdate = 0;
		UpdateData();
	}

	public virtual void UpdateData() {
		PlayerName.Text = Client.Name;
		if(Client.Pawn is OneChamberPlayer player) {
			Lives.Text = $"{player.Lives}";
		} else {
			Lives.Text = "-";
			AddClass("dead");
		}
			
		SetClass("me", Client == Local.Client && Client.All.Count > 1);
	}

	public virtual void UpdateFrom(Client client) {
		Client = client;
		UpdateData();
	}
}
