using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Classic.WeaponSpawns {
	[Library("sd_weaponspawn_smg")]
	[Hammer.EditorModel("models/weapons/smg/prop_smg.vmdl")]
	[Hammer.EntityTool("Smg", "Speed Dial Weapons", "Smg spawn point")]
	public class SmgWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_smg";
	}
}
