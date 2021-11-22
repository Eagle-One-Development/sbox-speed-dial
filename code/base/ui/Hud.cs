using Sandbox;
using Sandbox.UI;

namespace SpeedDial.Base.UI {
	public class BaseHud : HudEntity<RootPanel> {
		public BaseHud() {
			if(!IsClient) return;
			RootPanel.AddChild<ChatBox>();
		}
	}
}
