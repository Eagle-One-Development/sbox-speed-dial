using Sandbox;
using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.Weapons {
	[Library("sd_bat", Title = "BASEBALL BAT", Spawnable = true)]
	[Hammer.EditorModel("models/weapons/melee/melee.vmdl")]
	[Hammer.EntityTool("Baseball Bat", "Speed-Dial Classic Weapons", "Spawns a Baseball Bat.")]
	partial class BaseballBat : ClassicBaseWeapon, ISpawnable, IMelee {
		public override float PrimaryRate => 100;
		public virtual float HitDelay => 0.8f;
		public override bool Automatic => true;
		public override int HoldType => 1;
		public override int ClipSize => -1; // no ammo hud
		public override string WorldModel => "models/weapons/melee/melee.vmdl";
		public override string AttachementName => "melee_bat_attach";

		[Net, Predicted]
		public bool Hitting { get; set; } = false;

		[Net, Predicted]
		public TimeSince TimeSinceSwing { get; set; }

		public override void AttackPrimary() {
			if(TimeSinceSwing > HitDelay) {
				TimeSinceSwing = 0;
				Hitting = true;
				(Owner as AnimEntity).SetAnimBool("b_attack", true);
				PlaySound("woosh");
			}
		}

		public override void Simulate(Client owner) {
			base.Simulate(owner);

			if(EffectEntity.GetAttachment("melee_start") is Transform start && EffectEntity.GetAttachment("melee_end") is Transform end) {
				if(TimeSinceSwing <= 0.20f && Hitting) {
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
					Hitting = false;
				}
			}
		}
	}
}
