using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {

		[Net, Local]
		public int KillCombo { get; set; }

		[Net]
		public int Score { get; set; }

		[Net, Local]
		public TimeSince TimeSinceKilled { get; set; }

		public void ProcessKill() {
			if(Host.IsServer) {
				if(KillCombo >= 1) {
					if(TimeSinceKilled <= 3) {
						KillCombo++;
					} else {
						KillCombo = 0;
					}
				}
				KillCombo = 1;
				TimeSinceKilled = 0;
			}
		}

		[Event("tick")]
		public void OnTick() {
			if(Host.IsClient)
				DebugOverlay.ScreenText(new Vector2(300, 300), 1, Color.Green, $"{KillCombo} {TimeSinceKilled}");
		}
	}
}
