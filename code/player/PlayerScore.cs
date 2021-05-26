using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {

		[Net, Local]
		public int KillCombo { get; set; }

		[Net]
		public int Score { get; set; }

		[Net, Local]
		public TimeSince TimeSinceMurdered { get; set; }

		[Event("tick")]
		public void OnTick() {
			if(IsServer) {
				if(TimeSinceMurdered >= 3)
					KillCombo = 0;
			}
		}
	}
}
