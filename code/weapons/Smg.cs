using Sandbox;
using System;

namespace SpeedDial.Weapons {
	[Library("sd_mac", Title = "Mac-10")]
	partial class Mac10 : BaseSpeedDialWeapon {

		public override float PrimaryRate => 15.0f;
		public override float SecondaryRate => 1.0f;
		public override int ClipSize => 20;
		public override string WorldModel => "models/playermodels/weapons/prop_smg.vmdl";
		public override string ShootSound => "rust_smg.shoot";
		public override float BulletSpread => 0.65f;
		public override float BulletForce => 1.5f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override int HoldType => 1;
		public override bool Automatic => true;
		public override Vector4 ScreenShakeParameters => new(0.5f, 4, 1, 0.5f);
		public override string AttachementName => "smg_attach";
	}
}
