using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class ClassicScoreboard<T> : Panel where T : ClassicScoreboardEntry, new() {
		public Panel Canvas { get; protected set; }
		Dictionary<Client, T> Rows = new();

		public Panel Header { get; protected set; }

		public ClassicScoreboard() {
			StyleSheet.Load("/classic/ui/scoreboard/Scoreboard.scss");
			AddClass("scoreboard");

			AddHeader();

			Canvas = Add.Panel("canvas");
		}

		public override void Tick() {
			base.Tick();

			SetClass("open", Input.Down(InputButton.Score));

			if(!IsVisible)
				return;

			//
			// Clients that were added
			//
			foreach(var client in Client.All.Except(Rows.Keys)) {
				var entry = AddClient(client);
				Rows[client] = entry;
			}

			foreach(var client in Rows.Keys.Except(Client.All)) {
				if(Rows.TryGetValue(client, out var row)) {
					row?.Delete();
					Rows.Remove(client);
				}
			}

			Canvas.SortChildren((x) => -(x as ClassicScoreboardEntry).Client.GetValue("score", 0));

			for(int i = 0; i < Canvas.Children.Count(); i++) {
				var child = Canvas.Children.ElementAt(i);
				child.SetClass("first", i == 0 && (child as ClassicScoreboardEntry).Client.GetValue("score", 0) > 0);
			}

		}

		protected virtual void AddHeader() {
			Header = Add.Panel("header");
			Header.Add.Label("Name", "name");
			Header.Add.Label("Score", "score");
			Header.Add.Label("Max Combo", "maxcombo");
		}

		protected virtual T AddClient(Client entry) {
			var p = Canvas.AddChild<T>();
			p.Client = entry;
			return p;
		}
	}
}
