using Sandbox;

namespace SpeedDial.Classic.Weapons {
	[Library("sdg_shotgun", Title = "Spatz-12", Spawnable = true)]
	[Hammer.EntityTool("Shotgun", "Speed-Dial Classic Weapons", "Spawns a weapon")]
	partial class Shotgun : ClassicBaseWeapon, ISpawnable {
		public override float PrimaryRate => 1;
		public override int ClipSize => 6;
		public override int HoldType => 5;
		public override string WorldModel => "models/weapons/shotgun/prop_shotgun.vmdl";
		public override string ShootSound => "sd_shotgun.shoot";
		public override float BulletSpread => 0.4f;
		public override float BulletForce => 0.3f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override int BulletCount => 6;
		public override float Range => 1024;
		public override Vector4 ScreenShakeParameters => new(1, 1.5f, 2, 1);
		public override string AttachementName => "shotgun_attach";
		public override string EjectionParticle => "particles/weapon_fx/shotgun_ejectbrass.vpcf";
	}
}
