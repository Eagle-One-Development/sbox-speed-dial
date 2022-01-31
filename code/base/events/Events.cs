using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;

namespace SpeedDial {
	public static class SpeedDialEvent {
		public static class Gamemode {
			public class ResetAttribute : EventAttribute {
				public ResetAttribute() : base("sd.gamemode.reset") {}
			}

			public class StartAttribute : EventAttribute {
				public StartAttribute() : base("sd.gamemode.start") { }
			}

			public class EndAttribute : EventAttribute {
				public EndAttribute() : base("sd.gamemode.end") { }
			}
		}
	}
}
