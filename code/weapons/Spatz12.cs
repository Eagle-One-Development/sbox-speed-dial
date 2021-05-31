using Sandbox;
using System;

namespace SpeedDial.Weapons {
	[Library("sd_spatz", Title = "Spatz-12")]
	partial class Shotgun : BaseSpeedDialWeapon {
		public override float PrimaryRate => 1;
		public override float SecondaryRate => 1;
		public override int ClipSize => 8;
		public override int Bucket => 2;

		public override void Spawn() {
			base.Spawn();

			SetModel("models/weapons/sk_shotgun.vmdl");

			AmmoClip = 8;
		}

		public override void AttackPrimary() {
			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = 0;

			if(!TakeAmmo(1)) {
				DryFire();
				return;
			}

			(Owner as AnimEntity).SetAnimBool("b_attack", true);

			ShootEffects();
			PlaySound("rust_pumpshotgun.shoot");
			for(int i = 0; i < 6; i++) {
				ShootBullet(0.6f, 0.3f, 100, 3.0f);
			}
		}


		[ClientRpc]
		protected override void ShootEffects() {
			Host.AssertClient();

			Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
			Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");

			ViewModelEntity?.SetAnimBool("fire", true);

			if(IsLocalPawn) {
				new Sandbox.ScreenShake.Perlin(1.0f, 1.5f, 2.0f);
			}

			CrosshairPanel?.OnEvent("fire");
		}

		public override void SimulateAnimator(PawnAnimator anim) {
			anim.SetParam("holdtype", 2); // TODO this is shit
			anim.SetParam("aimat_weight", 1.0f);
		}
	}
}
