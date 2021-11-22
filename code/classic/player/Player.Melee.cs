using Sandbox;

namespace SpeedDial.Classic.Player {
	public partial class SpeedDialPlayer {
		[Net, Predicted] public TimeSince TimeSinceMeleeStarted { get; set; } = 0;
		[Net, Predicted] public bool ActiveMelee { get; set; } = false;

		public void StartMelee() {
			ActiveMelee = true;
			TimeSinceMeleeStarted = 0;
		}

		public void SimulateMelee() {
			if(TimeSinceMeleeStarted > 0.2f) {
				ActiveMelee = false;
				var forward = EyeRot.Forward;
				Vector3 pos = EyePos + Vector3.Down * 20f;
				var tr = Trace.Ray(pos, pos + forward * 40f)
				.UseHitboxes()
				.Ignore(this)
				.Size(20f)
				.Run();
				using(Prediction.Off()) {
					if(IsServer) {
						PlaySound("woosh");
					}
				}
				SetAnimBool("b_attack", true);
				if(!IsServer) return;
				if(!tr.Entity.IsValid()) return;
				if(!(LifeState == LifeState.Alive)) return;

				using(Prediction.Off()) {
					var damage = DamageInfo.FromBullet(tr.EndPos, Owner.EyeRot.Forward * 100, 200)
						.UsingTraceResult(tr)
						.WithAttacker(Owner)
						.WithWeapon(this);
					damage.Attacker = this;
					damage.Position = Position;
					if(IsServer) {
						PlaySound("smack");
					}
					if(tr.Entity is SpeedDialPlayer player) {
						player.CauseOfDeath = COD.Melee;
					}
					tr.Entity.TakeDamage(damage);
				}
			}
		}
	}
}
