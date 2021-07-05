using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.WeaponSpawns {
	[Library("sd_weaponspawn_pistol")]
	[Hammer.EditorModel("models/weapons/pistol/prop_pistol.vmdl")]
	public class PistolWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_pistol";
	}
}
