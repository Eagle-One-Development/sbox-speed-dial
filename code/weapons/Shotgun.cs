using Sandbox;
using System;

namespace SpeedDial.Weapons {
	[Library("sd_shotgun", Title = "Spatz-12")]
	partial class Shotgun : BaseSpeedDialWeapon {
		public override float PrimaryRate => 1;
		public override int ClipSize => 8;
		public override int HoldType => 4;
		public override string WorldModel => "models/playermodels/weapons/prop_shotgun.vmdl";
		public override string ShootSound => "rust_pumpshotgun.shoot";
		public override float BulletSpread => 0.6f;
		public override float BulletForce => 0.3f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override int BulletCount => 6;
		public override float Range => 1024;
		public override Vector4 ScreenShakeParameters => new(1, 1.5f, 2, 1);
		public override string AttachementName => "shotgun_attach";
	}
}
