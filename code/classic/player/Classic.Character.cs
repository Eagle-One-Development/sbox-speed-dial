using System.Collections.Generic;

using Sandbox;
using Hammer;

namespace SpeedDial.Classic.Player {
	[Library("sdchar"), AutoGenerate]
	public partial class Character : Asset {
		public string CharacterName { get; set; } = "name";
		public string Description { get; set; } = "desc";
		public string WeaponClass { get; set; } = "sd_pistol";
		[ResourceType("png")] public string Portrait { get; set; } = "materials/ui/portraits/jack_hd.png";
		[ResourceType("vmdl")] public string Model { get; set; } = "models/playermodels/character_jack.vmdl";

		[Skip]
		public static List<Character> All = new();

		[Skip]
		public Texture PortraitTexture { get; private set; }
		[Skip]
		public Model CharacterModel { get; private set; }

		protected override void PostLoad() {
			if(Host.IsClient) {
				PortraitTexture = Texture.Load(FileSystem.Mounted, Portrait);
			}

			CharacterModel = Sandbox.Model.Load(Model);

			All.Add(this);
			Log.Debug($"loaded character {CharacterName}");
		}
	}
}
