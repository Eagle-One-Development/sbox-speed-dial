using Sandbox;
using Hammer;

using System.Collections.Generic;
using System.Linq;

namespace SpeedDial.Classic.Player {
	[Library("sdchar"), AutoGenerate]
	public partial class Character : Asset {
		public string CharacterName { get; set; }
		public string Description { get; set; }
		public string WeaponClass { get; set; }
		[ResourceType("png")] public string Portrait { get; set; }
		[ResourceType("vmdl")] public string Model { get; set; }

		[Skip]
		public static List<Character> All = new();

		[Skip]
		public Texture PortraitTexture { get; private set; }
		protected override void PostLoad() {
			Portrait = System.IO.Path.ChangeExtension(Portrait, "png");
			PortraitTexture = Texture.Load(Portrait);
			All.Add(this);
			Debug.Log($"loaded character {CharacterName}");
		}
	}
}
