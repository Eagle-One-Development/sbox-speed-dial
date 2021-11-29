using Sandbox;

namespace SpeedDial.Classic.Weapons {
	[Library("sdg_roomclearer", Title = "ROOM-CLEARER", Spawnable = true)]
	[Hammer.EditorModel("models/weapons/shotgun/prop_roomclearer.vmdl")]
	[Hammer.EntityTool("Room-Clearer", "Speed-Dial Classic Weapons", "Spawns a Room-Clearer Shotgun")]
	partial class RoomClearer : ClassicBaseWeapon, ISpawnable {
		public override float PrimaryRate => 1;
		public override int ClipSize => 2;
		public override int HoldType => 5;
		public override string WorldModel => "models/weapons/shotgun/prop_roomclearer.vmdl";
		public override string ShootSound => "sd_roomclearer.shoot";
		public override float BulletSpread => 0.8f;
		public override float BulletForce => 0.3f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override int BulletCount => 16;
		public override float Range => 1024;
		public override string AttachementName => "roomclearer_attachment";
		public override float VerticalBulletSpread => 0.2f;
		public override string EjectionParticle => "particles/weapon_fx/shotgun_ejectbrass.vpcf";
	}
}
