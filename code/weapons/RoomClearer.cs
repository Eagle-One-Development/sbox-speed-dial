using Sandbox;
using System;

namespace SpeedDial.Weapons {
	[Library("sd_roomclearer", Title = "The Room Clearer (TEST)")]
	partial class RoomClearer : BaseSpeedDialWeapon {
		public override float PrimaryRate => 0.5f;
		public override int ClipSize => 4;
		public override int HoldType => 4;
		public override string WorldModel => "models/playermodels/weapons/prop_shotgun.vmdl";
		public override string ShootSound => "rust_pumpshotgun.shoot";
		public override float BulletSpread => 0.8f;
		public override float BulletForce => 0.3f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override int BulletCount => 8;
		public override float Range => 1024;
		public override string AttachementName => "shotgun_attach";
		public override float VerticalBulletSpread => 0.2f;
	}
}