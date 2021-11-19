using Sandbox;
using SpeedDial.UI;
namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {

		[Net, Local]
		public TimeSince TimeSinceMurdered { get; set; }

		[ClientRpc]
		public void ComboEvents(Vector3 position, int amt, int combo, COD death) {
			ComboPanel.Current.Bump();
			ComboPanel.Current.OnKill(position, amt, death, combo);
			GamePanel.Current?.Bump();
		}

		[Event.Tick]
		public void OnTick() {
			if(IsServer) {
				if(TimeSinceMurdered >= SpeedDialGame.ComboTime) {
					Client.SetValue("killcombo", 0);
				}
			}
		}
	}
}
