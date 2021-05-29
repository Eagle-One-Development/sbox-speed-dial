using Sandbox;
using SpeedDial.Player;

namespace SpeedDial.Weapons {
	[Library("sd_pistol", Title = "Pistol")]
	partial class Pistol : BaseSpeedDialWeapon {

		public override float PrimaryRate => 12.0f;
		public override float SecondaryRate => 1.0f;

		public override int Bucket => 1;

		public override int ClipSize => 12;

		public override void Spawn() {
			base.Spawn();

			SetModel("models/weapons/sk_prop_pistol_01.vmdl");
			AmmoClip = 12;
		}

		public override bool CanPrimaryAttack() {
			return base.CanPrimaryAttack() && Input.Pressed(InputButton.Attack1);
		}

		public override void AttackPrimary() {
			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = 0;

			if(!TakeAmmo(1)) {
				DryFire();
				return;
			}

			ShootEffects();
			PlaySound("rust_pistol.shoot");
			ShootBullet(0.05f, 1.5f, 100.0f, 3.0f);
		}
	}
}
