using System;

using Sandbox;

namespace SpeedDial.Classic {
	public static partial class Settings {
		[ClientVar("sd_viewshift_toggle")]
		public static bool ViewshiftToggle { get; set; } = false;
		[ClientVar("sd_music")]
		public static bool MusicEnabled { get;set; } = true;
	}
}
