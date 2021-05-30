using Sandbox;
using System;

namespace SpeedDial.Weapons {
	[Library("sd_mac", Title = "Mac-10")]
	partial class Mac10 : BaseSpeedDialWeapon {


		public override float PrimaryRate => 15.0f;
		public override float SecondaryRate => 1.0f;
		public override int ClipSize => 20;
		public override int Bucket => 2;

		public override void Spawn() {
			base.Spawn();

			SetModel("models/weapons/sk_prop_rifle_01.vmdl");
			AmmoClip = 20;
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
			PlaySound("rust_smg.shoot");
			ShootBullet(0.65f, 1.5f, 100, 3.0f);
		}

		public override void AttackSecondary() {
			// Grenade lob
		}

		[ClientRpc]
		protected override void ShootEffects() {
			Host.AssertClient();

			Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
			Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");

			if(Owner == Local.Pawn) {
				new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
			}

			CrosshairPanel?.OnEvent("fire");
		}

		public override void SimulateAnimator(PawnAnimator anim) {
			anim.SetParam("holdtype", 4); // TODO this is shit
			anim.SetParam("aimat_weight", 1.0f);
		}
	}
}
