using Sandbox;
using SpeedDial.Weapons;
using Sandbox.UI;

namespace SpeedDial.Player {
	public partial class BaseSpeedDialCharacter {

		/// <summary>
		/// Name of the library for the weapon this character spawns with
		/// </summary>
		public virtual string Weapon => "sd_pistol";

		/// <summary>
		/// The name of the character that shows up on the character select
		/// </summary>
		public virtual string Name => "N/A";

		/// <summary>
		/// The description that shows up on the character select
		/// </summary>
		public virtual string Description => "NONE";

		/// <summary>
		/// The path to the portrait that appears on the character select
		/// </summary>
		public virtual string Portrait => "ui/portraits/default.png";

		/// <summary>
		/// The path to the image of the head that appears on the UI
		/// </summary>
		public virtual string HeadImage => "ui/head/default.png";

		public BaseSpeedDialCharacter() {
		}

	}
}
