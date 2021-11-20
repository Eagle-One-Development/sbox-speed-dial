using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.WeaponSpawns {
	[Library("sd_weaponspawn_rifle")]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Rifle", "Speed Dial Weapons", "Rifle spawn point")]
	public class RifleWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_rifle";
	}
}
