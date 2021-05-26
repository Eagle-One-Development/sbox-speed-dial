using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {

		[Net, Local]
		public int KillCombo { get; set; }

		[Net, Local]
		public int KillScore { get; set; }

		[Net, Local]
		public TimeSince TimeSinceMurdered { get; set; }

		[Net, Local] //this should probably be a static of the game itself, oh well
		public int ScoreBase { get; set; } = 500;

		[Event("tick")]
		public void OnTick() {
			if(IsServer) {
				if(TimeSinceMurdered >= 3)
					KillCombo = 0;
			}
		}
	}
}
