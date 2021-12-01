using System;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class KillFeedEntry : Panel {
		public Label Left { get; internal set; }
		public Label Right { get; internal set; }
		//public Label Method { get; internal set; }
		public Image Method { get; internal set; }

		public RealTimeSince TimeSinceBorn = 0;

		public KillFeedEntry() {
			Left = Add.Label("", "left");
			Method = Add.Image("materials/ui/killicons/generic.png", "method");
			Right = Add.Label("", "right");
		}

		public override void Tick() {
			base.Tick();

			if(TimeSinceBorn > 6) {
				Delete();
			}
		}
	}
}
