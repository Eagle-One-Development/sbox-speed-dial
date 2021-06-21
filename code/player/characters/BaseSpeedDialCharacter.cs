using Sandbox;
using SpeedDial.Weapons;
using Sandbox.UI;

namespace SpeedDial.Player {
	public partial class BaseSpeedDialCharacter : NetworkComponent {

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
		public virtual string Portrait => "materials/ui/portraits/default.png";


		/// <summary>
		/// The path for the model
		/// </summary>
		public virtual string Model => "models/playermodels/character_fallback.vmdl";

		private Texture cachedTexture;
		public virtual Texture PortraitTexture {
			get {
				if(cachedTexture == null) {
					cachedTexture = Texture.Load(Portrait);
				}
				return cachedTexture;
			}
		}

		/// <summary>
		/// The path to the image of the head that appears on the UI
		/// </summary>
		public virtual string HeadImage => "ui/head/default.png";

		public BaseSpeedDialCharacter() {
		}

	}
}
