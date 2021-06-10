using Sandbox;
using SpeedDial.Player;

namespace SpeedDial.Weapons {
	[Library("sd_pipe", Title = "Pipe")]
	partial class Pipe : BaseSpeedDialWeapon {
		/*
		*
		* YES! THIS SHOULD BE ANOTHER BASE CLASS, LIKE BASE MELEE WEAPON
		* BUT BECAUSE I WAS DUMB AND LAZY, THIS IS HOW IT IS NOW
		*
		*/
		public override float PrimaryRate => 2.0f;
		public override int HoldType => 6; // need melee holdtype
		public override int ClipSize => -1; // no ammo hud
		public override string WorldModel => "models/weapons/pipe/pipe.vmdl";
		public override string AttachementName => "melee_pipe_attach";

		[Net, Predicted, Local]
		public bool Hitting { get; set; } = false;

		[Net, Predicted, Local]
		public TimeSince TimeSinceSwing { get; set; }

		public override void AttackPrimary(bool _ = false, bool __ = false) {
			if(TimeSinceSwing > 1) {
				TimeSinceSwing = 0;
				Hitting = true;
				(Owner as AnimEntity).SetAnimBool("b_attack", true);
				using(Prediction.Off()) {
					if(IsServer) {
						PlaySound("woosh");
					}
				}
			}
		}

		public override void Simulate(Client owner) {
			base.Simulate(owner);
			var start = EffectEntity.GetAttachment("melee_start").Position;
			var end = EffectEntity.GetAttachment("melee_end").Position;

			// hardcoded values cause I suck
			if(TimeSinceSwing <= 1.2f && Hitting) {
				if(TimeSinceSwing >= 0.8f) {
					foreach(var tr in TraceBullet(start, end, 4)) {
						if(tr.Entity is SpeedDialPlayer) {
							var ps = Particles.Create("particles/blood/blood_plip.vpcf", tr.EndPos);
							ps?.SetForward(0, tr.Normal);
						}
						if(!IsServer) continue;
						if(!tr.Entity.IsValid()) continue;

						using(Prediction.Off()) {
							var damageInfo = DamageInfo.FromBullet(tr.EndPos, 1000, 100)
								.UsingTraceResult(tr)
								.WithAttacker(Owner)
								.WithWeapon(this);

							tr.Entity.TakeDamage(damageInfo);
						}
					}
				}
			} else {
				Hitting = false;
			}
		}
	}
}
