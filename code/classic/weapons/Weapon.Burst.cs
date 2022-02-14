using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			if(TimeSinceSpecial > Template.FireDelay && Burst < Template.BurstLength) {
				Burst++;
				if(!TakeAmmo(1)) {
					TimeSinceSpecial = 0;
					Burst = 0;
					Firing = false;
					PlaySound("sd_dryfrire");
					return;
				}

				ShootEffects();

				ShootBullet(Template.BulletSpread * (float)((Burst * 1.5) + 1), Template.BulletForce, Template.BulletDamage, Template.BulletSize, 0);
				TimeSinceSpecial = 0;
			}

			if(Burst >= Template.BurstLength) {
				TimeSinceSpecial = 0;
				Burst = 0;
				Firing = false;
			}
		}
	}
}
