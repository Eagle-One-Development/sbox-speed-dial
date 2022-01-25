using Sandbox;

using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Entities {
	[Library("sd_weaponspawn_random", Title = "Random Weapon Spawn")]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Random Weapon", "Speed-Dial Weaponspawns", "Spawns random weapons.")]
	public partial class ClassicRandomWeaponSpawn : ClassicWeaponSpawn {
		public override void SpawnWeapon() {
			var ent = Library.Create<ClassicBaseWeapon>(ClassicBaseWeapon.GetRandomSpawnableType());
			ent.Transform = Transform;
			ent.WeaponSpawn = this;
			ent.ResetInterpolation();
		}
	}

	[Library("sd_weaponspawn_pistol")]
	[Hammer.EditorModel("models/weapons/pistol/prop_pistol.vmdl")]
	[Hammer.EntityTool("Pistol", "Speed-Dial Weaponspawns", "Spawns Pistols.")]
	public class PistolWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sd_pistol";
	}

	[Library("sd_weaponspawn_rifle")]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Rifle", "Speed-Dial Weaponspawns", "Spawns Rifles.")]
	public class RifleWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sd_rifle";
	}

	[Library("sd_weaponspawn_smg")]
	[Hammer.EditorModel("models/weapons/smg/prop_smg.vmdl")]
	[Hammer.EntityTool("SMG", "Speed-Dial Weaponspawns", "Spawns SMGs.")]
	public class SmgWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sd_smg";
	}

	[Library("sd_weaponspawn_shotgun")]
	[Hammer.EditorModel("models/weapons/shotgun/prop_shotgun.vmdl")]
	[Hammer.EntityTool("Shotgun", "Speed-Dial Weaponspawns", "Spawns Shotguns.")]
	public class ShotgunWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sd_shotgun";
	}

	[Library("sd_weaponspawn_sniper")]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Sniper", "Speed-Dial Weaponspawns", "Spawns Snipers.")]
	public class SniperWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sd_sniper";
	}

	[Library("sd_weaponspawn_roomclearer")]
	[Hammer.EditorModel("models/weapons/shotgun/prop_roomclearer.vmdl")]
	[Hammer.EntityTool("Room-Clearer", "Speed-Dial Weaponspawns", "Spawns Room-Clearer Shotguns.")]
	public class RoomClearerWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sd_roomclearer";
	}

	[Library("sd_weaponspawn_bat")]
	[Hammer.EditorModel("models/weapons/melee/melee.vmdl")]
	[Hammer.EntityTool("Baseball Bat", "Speed-Dial Weaponspawns", "Spawns Baseball Bats.")]
	public class BaseballBatWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sd_bat";
	}
}
