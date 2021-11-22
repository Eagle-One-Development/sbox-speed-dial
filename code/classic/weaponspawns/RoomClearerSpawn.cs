using Sandbox;

namespace SpeedDial.Classic.WeaponSpawns {
	[Library("sd_weaponspawn_roomclearer")]
	[Hammer.EditorModel("models/weapons/shotgun/prop_shotgun.vmdl")]
	[Hammer.EntityTool("Room Clearer", "Speed Dial Weapons", "Room Clearer spawn point")]
	public class RoomClearerWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_roomclearer";
	}
}
