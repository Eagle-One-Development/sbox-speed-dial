using Sandbox;
using SpeedDial.Player;

namespace SpeedDial.Weapons {
	[Library("sd_pistol", Title = "Pistol")]
	partial class Pistol : BaseSpeedDialWeapon {
		public override float PrimaryRate => 12.0f;
		public override int ClipSize => 12;
		public override float BulletSpread => 0.05f;
		public override float BulletForce => 1.5f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override int HoldType => 1;
		public override string ShootSound => "rust_pistol.shoot";
		public override string WorldModel => "models/playermodels/weapons/prop_pistol.vmdl";
		public override string AttachementName => "pistol_attach";
	}
}
