using System;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class KillFeedEntry : Panel {
		public Label Left { get; set; }
		public Label Right { get; set; }
		public Image Method { get; set; }

		public RealTimeSince TimeSinceCreated = 0;

		public override void Tick() {
			base.Tick();

			if(TimeSinceCreated > 6) {
				Delete();
			}
		}
	}
}
