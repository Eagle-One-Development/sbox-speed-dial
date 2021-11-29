using Sandbox;

namespace SpeedDial.Classic.Weapons {
	[Library("sdg_smg", Title = "SMG", Spawnable = true)]
	[Hammer.EditorModel("models/weapons/smg/prop_smg.vmdl")]
	[Hammer.EntityTool("SMG", "Speed-Dial Classic Weapons", "Spawns an SMG.")]
	partial class Smg : ClassicBaseWeapon, ISpawnable {
		public override float PrimaryRate => 15.0f;
		public override int ClipSize => 20;
		public override string WorldModel => "models/weapons/smg/prop_smg.vmdl";
		public override string ShootSound => "sd_smg.shoot";
		public override float BulletSpread => 0.70f;
		public override float VerticalBulletSpread => 0.35f;
		public override float BulletForce => 1.5f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override int HoldType => 3;
		public override bool Automatic => true;
		public override Vector4 ScreenShakeParameters => new(0.5f, 4, 1, 0.5f);
		public override string AttachementName => "smg_attach";

	}
}
