using Sandbox;

namespace SpeedDial.Classic.WeaponSpawns {
	[Library("sd_weaponspawn_rifle")]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Rifle", "Speed Dial Weapons", "Rifle spawn point")]
	public class RifleWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_rifle";
	}
}
