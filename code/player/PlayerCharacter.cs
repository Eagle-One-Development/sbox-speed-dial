using Sandbox;
using SpeedDial.Weapons;
using Sandbox.UI;

namespace SpeedDial.Player {
    public partial class SpeedDialPlayerCharacter
    {
		/// <summary>
		/// Name of the library for the weapon this character spawns with
		/// </summary>
        public string weapon;

		/// <summary>
		/// The name of the character that shows up on the character select
		/// </summary>
		public string name;

		/// <summary>
		/// The description that shows up on the character select
		/// </summary>
		public string description;

		/// <summary>
		/// The path to the portrait that appears on the character select
		/// </summary>
		public string portrait;

		/// <summary>
		/// The path to the image of the head that appears on the UI
		/// </summary>
		public string headImage;

		public SpeedDialPlayerCharacter(string w, string n, string d, string p, string h){
			weapon = w;
			name = n;
			description = d;
			portrait = p;
			headImage = h;
		}

    }
}
