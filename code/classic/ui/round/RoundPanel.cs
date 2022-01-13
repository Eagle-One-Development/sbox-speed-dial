using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class RoundPanel : Panel {

		public Panel Timer { get; set; }
		public Label TimeLabel { get; set; }
		public Panel TopPlayers { get; set; }

		Dictionary<Client, PlayerEntry> Players = new();

		public override void Tick() {
			if(Game.Current.GetGamemode() is Gamemode gamemode) {
				if(gamemode.GetRound() is TimedRound timedRound) {
					TimeLabel.Text = $"-{timedRound.TimeLeftFormatted}";
				} else if(gamemode.GetRound() is Round round) {
					TimeLabel.Text = $"+{round.TimeElapsedFormatted}";
				}
			}

			// Clients that joined
			foreach(var client in Client.All.Except(Players.Keys)) {
				var entry = AddClient(client);
				Players[client] = entry;
			}

			// clients that left
			foreach(var client in Players.Keys.Except(Client.All)) {
				if(Players.TryGetValue(client, out var row)) {
					row?.Delete();
					Players.Remove(client);
				}
			}

			TopPlayers.SortChildren((x) => -(x as PlayerEntry).Client.GetValue("score", 0));

			for(int i = 0; i < TopPlayers.Children.Count(); i++) {
				var child = TopPlayers.Children.ElementAt(i);
				// Only show top five and only show if there's more than one player connected
				child.SetClass("hidden", i > 4 || Client.All.Count <= 1);
			}
		}

		protected virtual PlayerEntry AddClient(Client entry) {
			var p = TopPlayers.AddChild<PlayerEntry>();
			p.Client = entry;
			return p;
		}
	}
}
