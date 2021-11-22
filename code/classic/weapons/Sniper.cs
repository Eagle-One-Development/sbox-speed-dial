using Sandbox;
using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.Weapons {
	[Library("sd_sniper", Title = "Sniper")]
	partial class Sniper : BaseSpeedDialWeapon {
		public override float PrimaryRate => 1.0f;
		public override int ClipSize => 5;
		public override float BulletSpread => 0.01f;
		public override float BulletForce => 3.5f;
		public override float BulletDamage => 100;
		public override float BulletSize => 5;
		public override int HoldType => 4;
		public override string ShootSound => "sd_sniper.shoot";
		public override string WorldModel => "models/weapons/rifle/prop_rifle.vmdl";
		public override string AttachementName => "rifle_attach";
	}
}
