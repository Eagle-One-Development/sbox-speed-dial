using Sandbox;
using SpeedDial.UI;
namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {

		[Net, Local]
		public int KillCombo { get; set; }

		[Net, Local]
		public int KillScore { get; set; }

		[Net, Local]
		public TimeSince TimeSinceMurdered { get; set; }

		[ClientRpc]
		public static void ComboEvents() {
			ComboPanel.Current.Bump();
		}

		[Event("tick")]
		public void OnTick() {
			if(IsServer) {
				if(TimeSinceMurdered >= SpeedDialGame.ComboTime)
					KillCombo = 0;
			}
		}
	}
}
