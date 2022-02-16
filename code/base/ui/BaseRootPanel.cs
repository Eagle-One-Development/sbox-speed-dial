using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial {
	public partial class BaseRootPanel : RootPanel {

		public Panel GamemodePanel { get; set; }

		public BaseRootPanel() {
			if(!Host.IsClient) return;

			StyleSheet.Load("/base/ui/root.scss");
			AddChild<DevInfo>();
			AddChild<DevMenu>();
		}
	}
}
