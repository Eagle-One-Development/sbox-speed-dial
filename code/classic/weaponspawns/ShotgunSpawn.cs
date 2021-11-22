using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Classic.WeaponSpawns {
	[Library("sd_weaponspawn_shotgun")]
	[Hammer.EditorModel("models/weapons/shotgun/prop_shotgun.vmdl")]
	[Hammer.EntityTool("Shotgun", "Speed Dial Weapons", "Shotgun spawn point")]
	public class ShotgunWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_shotgun";
	}
}
