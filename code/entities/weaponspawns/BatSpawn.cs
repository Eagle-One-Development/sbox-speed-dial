using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.WeaponSpawns {
	[Library("sd_weaponspawn_bat")]
	[Hammer.EditorModel("models/weapons/melee/melee.vmdl")]
	[Hammer.EntityTool("Bat", "Speed Dial Weapons", "Bat spawn point")]
	public class BatWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_bat";
	}
}
