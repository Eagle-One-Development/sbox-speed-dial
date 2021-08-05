using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.UI {
	public class VotingScreen : Panel {
		public VoteItemCollection VoteItemCollection;
		public SideInfoPanel SideInfoPanel;

		public Label timer;
		public Label VoteTimeLeft;

		public Label date;

		Sound tapeSound;

		public VotingScreen() {
			StyleSheet.Load("/ui/Voting/VotingScreen.scss");
			Init();
		}

		[Event.Hotload]
		private void Init() {
			Log.Info("initiating");

			if(VoteItemCollection is not null) VoteItemCollection.Delete(true);
			VoteItemCollection = AddChild<VoteItemCollection>();
			VoteItemCollection.Voted = false;
			if(SideInfoPanel is not null) SideInfoPanel.Delete(true);
			SideInfoPanel = AddChild<SideInfoPanel>();
			SideInfoPanel.Collection = VoteItemCollection;
			if(timer is not null) timer.Delete(true);
			timer = Add.Label("00:00", "timer Defaultlabel");
			if(date is not null) date.Delete(true);
			date = Add.Label("00:00:00", "date Defaultlabel");

			if(VoteTimeLeft is not null) VoteTimeLeft.Delete(true);
			VoteTimeLeft = Add.Label("Time left to vote", "VoteTimeLeft Defaultlabel");
		}
		public override void Tick() {
			base.Tick();
			if(SpeedDialGame.Instance.Round is VoteRound vr) {
				timer.Text = vr.TimeLeftFormatted;

				DateTime dt = DateTime.Now.AddYears(-28);

				string s = dt.ToString(@"tt hh:mm");

				s += "\n";

				s += dt.ToString(@"MMM. dd yyyy");
				date.Text = s;
			}

		}
		[SDEvent.Voting.Start]
		private void Open() {
			SetClass("Active", true);
			Log.Info("opened");
			CharacterSelect.Current.open = true;

			tapeSound = Sound.FromScreen("tape_noise");
		}
		[SDEvent.Voting.End]
		private void Close() {

			tapeSound.Stop();
			CharacterSelect.Current.open = false;

			VoteItem Winner = VoteItemCollection.items[0];

			foreach(var item in VoteItemCollection.items) {
				if(Winner.votes == item.votes && Rand.Int(0, 1) == 0) continue;
				else if(Winner.votes <= item.votes) Winner = item;
			}
			if(Winner.votes == 0) {
				PlayEnd(null);
				return;
			}

			if(Global.IsListenServer && Host.IsClient && !Winner.MapInfo.FullIdent.Equals(Global.MapName) && !Winner.HasClass("Back"))
				PlayEnd(Winner);
			else
				PlayEnd(null);

		}

		private async void PlayEnd(VoteItem winner = null) {
			if(winner is VoteItem vi) {
				var sound = Sound.FromScreen("fastforward_map_selection");
				await GameTask.DelayRealtimeSeconds(1.25f);
				ConsoleSystem.Run($"changelevel {vi.MapInfo.FullIdent}");
			} else {
				var sound = Sound.FromScreen("rewind_map_selection");
				await GameTask.DelayRealtimeSeconds(1.25f);
			}
			SetClass("Active", false);

		}
	}
}
