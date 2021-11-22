using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Classic.WeaponSpawns {
	[Library("sd_weaponspawn_pistol")]
	[Hammer.EditorModel("models/weapons/pistol/prop_pistol.vmdl")]
	[Hammer.EntityTool("Pistol", "Speed Dial Weapons", "Pistol spawn point")]
	public class PistolWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_pistol";
	}
}
