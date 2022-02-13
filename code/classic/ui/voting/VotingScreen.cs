using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class VotingScreen : Panel {
		private string Title => "Gamemode Voting";
		private Panel Items { get; set; }
		private Panel Players { get; set; }

		public override void Tick() {
			base.Tick();
			SetClass("open", Debug.UI);
		}
	}
}
