using Sandbox;

using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Weapons {
	[Library("sdg_rifle", Title = "BURST-RIFLE", Spawnable = true)]
	[Hammer.EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
	[Hammer.EntityTool("Rifle", "Speed-Dial Classic Weapons", "Spawns a Rifle.")]
	partial class Rifle : ClassicBaseWeapon, ISpawnable {
		public override float PrimaryRate => 2.0f;
		public override int ClipSize => 24;
		public int burst = 3;
		private int curBurst = 0;
		private bool isFiring;
		public TimeSince burstTimer;
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

			if(isFiring) {
				if(burstTimer > 0.07f && curBurst < burst) {
					curBurst++;
					if(!TakeAmmo(1)) {
						burstTimer = 0;
						curBurst = 0;
						isFiring = false;
						PlaySound("sd_dryfrire");
						return;
					}
					(Owner as AnimEntity).SetAnimBool("b_attack", true);

					ShootEffects();

					using(Prediction.Off()) {
						WeaponPanel.Fire(To.Single(Owner.Client), PanelBumpScale);
						Crosshair.Fire(To.Single(Owner.Client));
					}
					ShootBullet(BulletSpread * (float)((curBurst * 1.5) + 1), BulletForce, BulletDamage, BulletSize, 0);
					burstTimer = 0;
				}

				if(curBurst >= burst) {
					burstTimer = 0;
					curBurst = 0;
					isFiring = false;
				}
			}
		}

		public override void AttackPrimary() {
			TimeSincePrimaryAttack = 0;

			if(!isFiring) {
				isFiring = true;
			}
		}
	}
}
