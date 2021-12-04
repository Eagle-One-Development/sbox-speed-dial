using Sandbox;

using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Weapons {
	[Library("sdg_rifle", Title = "BURST-RIFLE", Spawnable = true)]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Rifle", "Speed-Dial Classic Weapons", "Spawns a Rifle.")]
	partial class Rifle : ClassicBaseWeapon, ISpawnable {
		public override float PrimaryRate => 2.0f;
		public override int ClipSize => 24;
		public virtual int BurstLength => 3;
		private int Burst = 0;
		private bool Firing;
		public TimeSince TimeSinceBurst;
		public override int HoldType => 4;
		public override Vector4 ScreenShakeParameters => new(0.5f, 4.0f, 1.0f, 0.5f);
		public override float BulletSpread => 0.05f;
		public override float VerticalBulletSpread => 0.25f;
		public override float BulletForce => 1.5f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override string ShootSound => "sd_rifle.shoot";
		public override string WorldModel => "models/weapons/rifle/prop_rifle.vmdl";
		public override bool Automatic => true;
		public override string AttachementName => "rifle_attach";

		// Override for burst fire
		public override void Simulate(Client owner) {

			base.Simulate(owner);

			if(Firing) {
				if(TimeSinceBurst > 0.07f && Burst < BurstLength) {
					Burst++;
					if(!TakeAmmo(1)) {
						TimeSinceBurst = 0;
						Burst = 0;
						Firing = false;
						PlaySound("sd_dryfrire");
						return;
					}

					ShootEffects();

					ShootBullet(BulletSpread * (float)((Burst * 1.5) + 1), BulletForce, BulletDamage, BulletSize, 0);
					TimeSinceBurst = 0;
				}

				if(Burst >= BurstLength) {
					TimeSinceBurst = 0;
					Burst = 0;
					Firing = false;
				}
			}
		}

		public override void AttackPrimary() {
			TimeSincePrimaryAttack = 0;

			if(!Firing) {
				Firing = true;
			}
		}
	}
}
