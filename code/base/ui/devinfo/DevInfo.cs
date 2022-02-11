using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;

namespace SpeedDial {
	[UseTemplate]
	public partial class DevInfo : Panel {
		private string Info { get; set; }
		public override void Tick() {
			SetClass("visible", Debug.Playtest || Debug.Enabled);
			Info = $"IN-DEV {DateTime.Now.ToString("dd.MM.yyyy")}";
		}
	}
}
