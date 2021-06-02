using Sandbox;
using SpeedDial.UI;
namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {

		[Net, Local]
		public int KillCombo { get; set; }

		[Net]
		public int KillScore { get; set; }

		[Net, Local]
		public TimeSince TimeSinceMurdered { get; set; }

		[Net]
		public int maxCombo { get; set; }

		[ClientRpc]
		public void ComboEvents(Vector3 position, int amt) {
			ComboPanel.Current.Bump();
			ComboPanel.Current.OnKill(position, amt);
			AmmoPanel.Current?.Bump();
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
