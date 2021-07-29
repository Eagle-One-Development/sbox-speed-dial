using System;
using System.Collections.Generic;
using Sandbox;
using SpeedDial.UI;

namespace SpeedDial {

	public partial class VoteRound : BaseRound {
		public override string RoundName => "Voting";
		public override int RoundDuration => 15;
		public static bool mapsReceived = false;

		protected override void OnStart() {
			if(Host.IsServer)
				RunVotingEvent(To.Everyone, "Start");
			Log.Info("VoteRound Started");
		}
		public override void OnSecond() {
			if(Host.IsServer && mapsReceived) {
				if(RoundEndTime > 0 && Sandbox.Time.Now >= RoundEndTime) {
					RoundEndTime = 0f;
					OnTimeUp();
				} else {
					TimeLeftFormatted = TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
					//NetworkDirty("TimeLeftFormatted", NetVarGroup.Net);
				}
			}
		}
		[ServerCmd]
		public static void StartTimer() {
			var game = SpeedDialGame.Current as SpeedDialGame;
			if(Host.IsServer && game.Round.RoundDuration > 0) {
				game.Round.RoundEndTime = Sandbox.Time.Now + game.Round.RoundDuration;
				VoteRound.mapsReceived = true;

			}
		}

		[ClientRpc()]
		public static void RunVotingEvent(string VoteEvent) {
			Event.Run($"SDEvent.Voting.{VoteEvent}");
		}

		[ClientRpc]
		public static void RefreshMapSelection(string json) {
			VoteItemCollection.instance.SetDataAndRecreate(json);
			if(Global.IsListenServer)
				StartTimer();
		}

		protected override void OnTimeUp() {
			if(Host.IsServer)
				RunVotingEvent(To.Everyone, "End");

			mapsReceived = false;
			SpeedDialGame.Instance.ChangeRound(new WarmUpRound());
		}




	}



}
