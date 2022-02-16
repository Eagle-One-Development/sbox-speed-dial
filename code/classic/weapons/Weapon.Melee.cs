using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.Weapons;

public partial class Weapon {
	private void MeleePrimary() {
		if(TimeSinceSpecial > Blueprint.FireDelay) {
			TimeSinceSpecial = 0;
			Firing = true;
			(Owner as AnimEntity).SetAnimBool("b_attack", true);
			PlaySound("woosh");
		}
	}

	private void MeleeSimulate() {
		if(EffectEntity.GetAttachment("melee_start") is Transform start && EffectEntity.GetAttachment("melee_end") is Transform end) {
			if(TimeSinceSpecial <= 0.20f && Firing) {
				foreach(var tr in TraceBullet(start.Position, end.Position, 4)) {
					if(tr.Entity is ClassicPlayer hitPlayer && hitPlayer.IsValid()) {
						var ps = Particles.Create("particles/blood/blood_plip.vpcf", tr.EndPos);
						ps?.SetForward(0, tr.Normal);

						PlaySound("sd_bat.hit");

						if(IsServer) {
							using(Prediction.Off()) {
								var damageInfo = DamageInfo.FromBullet(tr.EndPos, 1000, 100)
									.UsingTraceResult(tr)
									.WithAttacker(Owner)
									.WithWeapon(this);

								tr.Entity.TakeDamage(damageInfo);
							}
						}
					}
				}
			} else {
				Firing = false;
			}
		}
	}
}
