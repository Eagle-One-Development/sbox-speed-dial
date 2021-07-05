using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.WeaponSpawns {
	[Library("sd_weaponspawn_roomclearer")]
	[Hammer.EditorModel("models/weapons/shotgun/prop_shotgun.vmdl")]
	public class RoomClearerWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_roomclearer";
	}
}
