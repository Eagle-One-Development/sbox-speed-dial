using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class ClassicScoreboard<T> : Panel where T : ClassicScoreboardEntry, new() {
		private readonly Panel Canvas;
		Dictionary<Client, T> Rows = new();

		private readonly Panel Header;

		public ClassicScoreboard() {
			StyleSheet.Load("/classic/ui/scoreboard/Scoreboard.scss");
			AddClass("scoreboard");

			// Add header
			Header = Add.Panel("header");
			Header.Add.Label("Name", "name");
			Header.Add.Label("Score", "score");
			Header.Add.Label("Max Combo", "maxcombo");
			//

			Canvas = Add.Panel("canvas");

			BindClass("open", () => Input.Down(InputButton.Score));
		}

		public override void Tick() {
			base.Tick();

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

		protected virtual T AddClient(Client entry) {
			var p = Canvas.AddChild<T>();
			p.Client = entry;
			return p;
		}
	}
}
