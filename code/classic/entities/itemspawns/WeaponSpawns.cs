using Sandbox;

using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Entities {
	[Library("sdg_weaponspawn_random", Title = "Random Weapon Spawn")]
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

	[Library("sdg_weaponspawn_pistol")]
	[Hammer.EditorModel("models/weapons/pistol/prop_pistol.vmdl")]
	[Hammer.EntityTool("Pistol", "Speed-Dial Weaponspawns", "Spawns Pistols.")]
	public class PistolWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sdg_pistol";
	}

	[Library("sdg_weaponspawn_rifle")]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Rifle", "Speed-Dial Weaponspawns", "Spawns Rifles.")]
	public class RifleWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sdg_rifle";
	}

	[Library("sdg_weaponspawn_smg")]
	[Hammer.EditorModel("models/weapons/smg/prop_smg.vmdl")]
	[Hammer.EntityTool("SMG", "Speed-Dial Weaponspawns", "Spawns SMGs.")]
	public class SmgWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sdg_smg";
	}

	[Library("sdg_weaponspawn_shotgun")]
	[Hammer.EditorModel("models/weapons/shotgun/prop_shotgun.vmdl")]
	[Hammer.EntityTool("Shotgun", "Speed-Dial Weaponspawns", "Spawns Shotguns.")]
	public class ShotgunWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sdg_shotgun";
	}

	[Library("sdg_weaponspawn_sniper")]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Sniper", "Speed-Dial Weaponspawns", "Spawns Snipers.")]
	public class SniperWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sdg_sniper";
	}

	[Library("sdg_weaponspawn_roomclearer")]
	[Hammer.EditorModel("models/weapons/shotgun/prop_roomclearer.vmdl")]
	[Hammer.EntityTool("Room-Clearer", "Speed-Dial Weaponspawns", "Spawns Room-Clearer Shotguns.")]
	public class RoomClearerWeaponSpawn : ClassicWeaponSpawn {
		public override string WeaponClass => "sdg_roomclearer";
	}
}
