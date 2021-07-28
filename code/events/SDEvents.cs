using System;
using Sandbox;

namespace SpeedDial {
	public static class SDEvent {

		public static class Voting {
			public class StartAttribute : EventAttribute {
				public StartAttribute()
					: base("SDEvent.Voting.Start") {
				}
			}
			public class EndAttribute : EventAttribute {
				public EndAttribute()
					: base("SDEvent.Voting.End") {
				}
			}
		}

	}
}
