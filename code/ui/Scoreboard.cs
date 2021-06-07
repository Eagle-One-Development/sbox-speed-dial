
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace SpeedDial.UI {
	public partial class SpeedDialScoreboard<T> : Panel where T : SpeedDialScoreboardEntry, new() {
		public Panel Canvas { get; protected set; }
		public TextEntry Input { get; protected set; }

		Dictionary<int, T> Entries = new();

		public Panel Header { get; protected set; }

		public SpeedDialScoreboard() {
			StyleSheet.Load("/ui/Scoreboard.scss");
			AddClass("scoreboard");

			AddHeader();

			Canvas = Add.Panel("canvas");

			PlayerScore.OnPlayerAdded += AddPlayer;
			PlayerScore.OnPlayerUpdated += UpdatePlayer;
			PlayerScore.OnPlayerRemoved += RemovePlayer;

			foreach(var player in PlayerScore.All) {
				AddPlayer(player);
			}
		}

		public override void Tick() {
			base.Tick();

			foreach(var (_, value) in Entries) {
				SpeedDialScoreboardEntry entry = value;
				entry.FauxTick();
			}

			SetClass("open", Local.Client?.Input.Down(InputButton.Score) ?? false);
		}

		protected virtual void AddHeader() {
			Header = Add.Panel("header");
			Header.Add.Label("Name", "name");
			Header.Add.Label("Score", "score");
			Header.Add.Label("Max Combo", "maxcombo");
		}

		protected virtual void AddPlayer(PlayerScore.Entry entry) {
			var p = Canvas.AddChild<T>();
			p.UpdateFrom(entry);

			Entries[entry.Id] = p;
		}

		protected virtual void UpdatePlayer(PlayerScore.Entry entry) {
			if(Entries.TryGetValue(entry.Id, out var panel)) {
				panel.UpdateFrom(entry);
				//sort scoreboard entries by score
				Canvas.SortChildren((x) => -(x as SpeedDialScoreboardEntry).Entry.Get<int>("score"));
			}
		}

		protected virtual void RemovePlayer(PlayerScore.Entry entry) {
			if(Entries.TryGetValue(entry.Id, out var panel)) {
				panel.Delete();
				Entries.Remove(entry.Id);
			}
		}

		[ServerCmd]
		public void UpdateScoreboard() {
			foreach(var player in PlayerScore.All) {
				UpdatePlayer(player);
			}
		}
	}
}
