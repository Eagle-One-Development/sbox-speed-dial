using System;
using System.Collections.Generic;
using Sandbox;
using SpeedDial.Player;
using SpeedDial.UI;

namespace SpeedDial {

	public partial class VoteRound : BaseRound {
		public override string RoundName => "Voting";
		public override int RoundDuration => 30;
		[Net] public static bool mapsReceived { get; set; } = false;

		protected override void OnStart() {
			if(Host.IsServer)
				RunVotingEvent(To.Everyone, "Start");
		}

		public override void OnSecond() {
			if(Host.IsServer && mapsReceived) {
				if(RoundEndTime > 0 && Time.Now >= RoundEndTime) {
					RoundEndTime = 0f;
					OnTimeUp();
				} else {
					TimeLeftFormatted = TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
				}
			}
		}

		[ServerCmd]
		public static void StartTimer() {
			var game = SpeedDialGame.Current as SpeedDialGame;
			if(Host.IsServer && game.Round.RoundDuration > 0) {
				game.Round.RoundEndTime = Time.Now + game.Round.RoundDuration;
				mapsReceived = true;
			}
		}

		[ClientRpc]
		public static void RunVotingEvent(string VoteEvent) {
			Event.Run($"SDEvent.Voting.{VoteEvent}");
		}

		static string currentJson;

		[ServerCmd]
		public static void RefreshMapSelection(string json) {
			RefreshMapSelectionClient(json);
			currentJson = json;
		}

		[ClientRpc]
		public static void RefreshMapSelectionClient(string json) {
			VoteItemCollection.SetDataAndRecreate(json);
			if(Global.IsListenServer)
				StartTimer();
		}

		protected override async void OnTimeUp() {
			if(Host.IsServer) {
				ResetVotingState(To.Everyone);
				RunVotingEvent(To.Everyone, "End");
			}

			mapsReceived = false;
			await GameTask.DelayRealtimeSeconds(1.25f);
			SpeedDialGame.Instance.ChangeRound(new WarmUpRound());
		}

		[ClientRpc]
		public static void ResetVotingState() {
			VoteItemCollection.Voted = false;
		}

		public override void OnPlayerSpawn(SpeedDialPlayer player) {
			base.OnPlayerSpawn(player);
			RefreshMapSelectionClient(To.Single(player), currentJson);
			PlayerJoinedDuringVote();
		}

		[ClientRpc]
		public static void PlayerJoinedDuringVote() {
			RunVotingEvent("Start");
		}
	}
}
