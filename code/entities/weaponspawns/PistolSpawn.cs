using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.WeaponSpawns {
	[Library("sd_weaponspawn_pistol")]
	public class PistolWeaponSpawn : BaseWeaponSpawn {
		public override string WorldModel => "models/playermodels/weapons/prop_pistol.vmdl";
		public override string WeaponClass => "sd_pistol";
	}
}
