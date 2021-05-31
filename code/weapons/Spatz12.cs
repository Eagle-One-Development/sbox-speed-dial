using Sandbox;
using System;

namespace SpeedDial.Weapons {
	[Library("sd_spatz", Title = "Spatz-12")]
	partial class Shotgun : BaseSpeedDialWeapon {
		public override float PrimaryRate => 1;
		public override int ClipSize => 8;
		public override int HoldType => 2;
		public override string WorldModel => "models/weapons/sk_shotgun.vmdl";
		public override string ShootSound => "rust_pumpshotgun.shoot";
		public override float BulletSpread => 0.6f;
		public override float BulletForce => 0.3f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override Vector4 ScreenShakeParameters => new(1, 1.5f, 2, 1);
	}
}
