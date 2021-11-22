using Sandbox;

namespace SpeedDial.Classic.WeaponSpawns {
	[Library("sd_weaponspawn_sniper")]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Sniper", "Speed Dial Weapons", "Sniper spawn point")]
	public class SniperWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_sniper";
	}
}
