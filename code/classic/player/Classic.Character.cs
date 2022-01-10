using System.Collections.Generic;

using Sandbox;
using Hammer;

namespace SpeedDial.Classic.Player {
	[Library("sdgchar"), AutoGenerate]
	public partial class Character : Asset {
		public string CharacterName { get; set; } = "name";
		public string Description { get; set; } = "desc";
		public string WeaponClass { get; set; } = "sdg_pistol";
		[ResourceType("png")] public string Portrait { get; set; } = "materials/ui/portraits/jack_hd.jpg";
		[ResourceType("png")] public string Weapon { get; set; } = "materials/ui/weapons/pistol.jpg";
		[ResourceType("vmdl")] public string Model { get; set; } = "models/playermodels/character_jack.vmdl";

		[Skip]
		public static List<Character> All = new();

		[Skip]
		public Texture PortraitTexture { get; private set; }
		[Skip]
		public Texture WeaponTexture { get; private set; }
		[Skip]
		public Model CharacterModel { get; private set; }

		protected override void PostLoad() {
			Portrait = System.IO.Path.ChangeExtension(Portrait, "png");
			Weapon = System.IO.Path.ChangeExtension(Weapon, "png");
			if(Host.IsClient) {
				PortraitTexture = Texture.Load(FileSystem.Mounted, Portrait);
				WeaponTexture = Texture.Load(FileSystem.Mounted, Weapon);
			}

			CharacterModel = Sandbox.Model.Load(Model);

			All.Add(this);
			Debug.Log($"loaded character {CharacterName}");
		}
	}
}
