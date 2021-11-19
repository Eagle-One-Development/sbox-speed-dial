
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeedDial.UI {
	public partial class SpeedDialScoreboard<T> : Panel where T : SpeedDialScoreboardEntry, new() {
		public Panel Canvas { get; protected set; }
		Dictionary<Client, T> Rows = new();

		public Panel Header { get; protected set; }

		public SpeedDialScoreboard() {
			StyleSheet.Load("/ui/scoreboard/SD_Scoreboard.scss");
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

			Canvas.SortChildren((x) => -(x as SpeedDialScoreboardEntry).Client.GetValue("score", 0));
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
