using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class OneChamberScoreboard : Panel {

	Dictionary<Client, OneChamberScoreboardEntry> Rows = new();

	public Panel Header { get; set; }
	public Panel Canvas { get; set; }
	public Panel Footer { get; set; }
	public Label GamemodeInfo { get; set; }
	public Label MapInfo { get; set; }

	public OneChamberScoreboard() {
		BindClass("open", () => Input.Down(InputButton.Score));
	}

	public override void Tick() {
		if(!IsVisible)
			return;

		GamemodeInfo.Text = $"Gamemode: {Game.Current.ActiveGamemode?.ClassInfo.Name}";
		MapInfo.Text = $"Map: {Global.MapName}";
		Footer.SetClass("visible", true);

		// Clients that joined
		foreach(var client in Client.All.Except(Rows.Keys)) {
			var entry = AddClient(client);
			Rows[client] = entry;
		}

		// clients that left
		foreach(var client in Rows.Keys.Except(Client.All)) {
			if(Rows.TryGetValue(client, out var row)) {
				row?.Delete();
				Rows.Remove(client);
			}
		}

		// sort by lives if pawn is still a player, otherwise push to the bottom (spectator)
		Canvas.SortChildren((x) => ((x as OneChamberScoreboardEntry).Client.Pawn is OneChamberPlayer player) ? -player.Lives : 1);

		for(int i = 0; i < Canvas.Children.Count(); i++) {
			var child = Canvas.Children.ElementAt(i);
			child.SetClass("first", i == 0 && (child as OneChamberScoreboardEntry).Client.GetValue("score", 0) > 0);
		}
	}

	protected virtual OneChamberScoreboardEntry AddClient(Client entry) {
		var p = Canvas.AddChild<OneChamberScoreboardEntry>();
		p.Client = entry;
		return p;
	}
}
