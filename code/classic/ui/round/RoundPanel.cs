using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class RoundPanel<T> : Panel where T : PlayerEntry, new() {
		private readonly Panel Timer;
		private readonly Label TimeLabel;
		private readonly Panel TopPlayers;
		Dictionary<Client, T> Players = new();

		public RoundPanel() {
			StyleSheet.Load("/classic/ui/round/RoundPanel.scss");
			AddClass("roundpanel");
			Timer = Add.Panel("timer");
			TimeLabel = Timer.Add.Label("69:69", "timelabel");
			TopPlayers = Add.Panel("players");
		}

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

		protected virtual T AddClient(Client entry) {
			var p = TopPlayers.AddChild<T>();
			p.Client = entry;
			return p;
		}
	}
}
