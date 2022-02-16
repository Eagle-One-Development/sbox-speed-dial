using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial;

[UseTemplate]
public partial class DevMenu : Panel {
	public override void Tick() {
		base.Tick();

		SetClass("open", Local.Client.IsListenServerHost && Input.Down(InputButton.View));
	}
}
