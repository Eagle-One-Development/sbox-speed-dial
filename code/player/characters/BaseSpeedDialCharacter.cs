using Sandbox;
using SpeedDial.Weapons;
using Sandbox.UI;

namespace SpeedDial.Player {
	public partial class BaseSpeedDialCharacter {

		/// <summary>
		/// Name of the library for the weapon this character spawns with
		/// </summary>
		public virtual string weapon => "sd_pistol";

		/// <summary>
		/// The name of the character that shows up on the character select
		/// </summary>
		public virtual string name => "N/A";

		/// <summary>
		/// The description that shows up on the character select
		/// </summary>
		public virtual string description => "NONE";

		/// <summary>
		/// The path to the portrait that appears on the character select
		/// </summary>
		public virtual string portrait => "ui/portraits/default.png";

		/// <summary>
		/// The path to the image of the head that appears on the UI
		/// </summary>
		public virtual string headImage => "ui/head/default.png";

		public BaseSpeedDialCharacter() {
		}

	}
}
