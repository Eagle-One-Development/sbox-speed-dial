using System;
using Sandbox;

namespace SpeedDial {

	public partial class VoteRound : BaseRound {
		public override string RoundName => "Voting";
		public override int RoundDuration => 15;

		protected override void OnStart() {
			if(Host.IsServer)
				RunVotingEvent(To.Everyone, "Start");
			Log.Info("VoteRound Started");
		}
		[ClientRpc]
		public static void RunVotingEvent(string VoteEvent) {
			Event.Run($"SDEvent.Voting.{VoteEvent}");
		}

		protected override void OnTimeUp() {
			if(Host.IsServer)
				RunVotingEvent(To.Everyone, "End");




			SpeedDialGame.Instance.ChangeRound(new WarmUpRound());
		}
	}
}
