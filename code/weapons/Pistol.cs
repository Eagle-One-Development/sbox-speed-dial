using Sandbox;
using SpeedDial.Player;

namespace SpeedDial.Weapons {
	[Library("sd_pistol", Title = "Glock-7")]
	partial class Pistol : BaseSpeedDialWeapon {
		public override float PrimaryRate => 12.0f;
		public override int ClipSize => 16;
		public override float BulletSpread => 0.24f;
		public override float BulletForce => 1.5f;
		public override float BulletDamage => 105;
		public override float BulletSize => 4;
		public override float VerticalBulletSpread => 0.25f;
		public override int HoldType => 2;
		public override string ShootSound => "sd_pistol.shoot";
		public override string WorldModel => "models/weapons/pistol/prop_pistol.vmdl";
		public override string AttachementName => "pistol_attach";
	}
}
