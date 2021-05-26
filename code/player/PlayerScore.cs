using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {

		[Net, Local]
		public int KillCombo { get; set; }

		[Net, Local]
		public int KillScore { get; set; }

		[Net, Local]
		public TimeSince TimeSinceMurdered { get; set; }


		[Event("tick")]
		public void OnTick() {
			if(IsServer) {
				if(TimeSinceMurdered >= SpeedDialGame.ComboTime)
					KillCombo = 0;
			}
		}
	}
}
