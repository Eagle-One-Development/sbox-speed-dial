using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public class VoteItem : Panel {
		protected override void OnMouseDown(MousePanelEvent e) {
			base.OnMouseDown(e);
			Log.Info("click");
		}
	}
}
