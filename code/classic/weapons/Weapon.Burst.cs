namespace SpeedDial.Classic.Weapons;

public partial class Weapon {
	private void BurstPrimary() {
		TimeSincePrimaryAttack = 0;

		if(!Firing) {
			Firing = true;
		}
	}

	private void BurstSimulate() {
		if(Firing) {
			if(TimeSinceSpecial > Blueprint.FireDelay && Burst < Blueprint.BurstLength) {
				Burst++;
				if(!TakeAmmo(1)) {
					TimeSinceSpecial = 0;
					Burst = 0;
					Firing = false;
					PlaySound("sd_dryfrire");
					return;
				}

				ShootEffects();

				ShootBullet(Blueprint.BulletSpread * (float)((Burst * 1.5) + 1), Blueprint.BulletForce, Blueprint.BulletDamage, Blueprint.BulletSize, 0);
				TimeSinceSpecial = 0;
			}

			if(Burst >= Blueprint.BurstLength) {
				TimeSinceSpecial = 0;
				Burst = 0;
				Firing = false;
			}
		}
	}
}
