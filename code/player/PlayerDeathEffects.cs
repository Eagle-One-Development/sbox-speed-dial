using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {

		[ClientRpc]
		public void BloodSplatter() {
			Host.AssertClient();
			BloodSplatter(Vector3.Down);
		}

		[ClientRpc]
		public void BloodSplatter(Vector3 dir) {
			Host.AssertClient();
			Vector3 pos = EyePos + Vector3.Down * 20;

			// splatters around and behind the target, mostly from impact
			for(int i = 0; i < 10; i++) {
				var trDir = pos + (dir.Normal + (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 0.85f * 0.25f) * 100 + Vector3.Down * i;
				var trSplatter = Trace.Ray(pos, trDir)
						.UseHitboxes()
						.Ignore(this)
						.Size(1)
						.Run();

				var decalPathSplatter = "decals/blood_splatter.decal";
				if(decalPathSplatter != null) {
					if(DecalDefinition.ByPath.TryGetValue(decalPathSplatter, out var decal)) {
						decal.PlaceUsingTrace(trSplatter);
					}
				}
			}

			//For blood splatter on the ground, pool of blood essentially
			for(int i = 0; i < 5; i++) {
				var trDir = pos + (Vector3.Down + (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 3 * 0.25f) * 100;
				var tr = Trace.Ray(pos, trDir)
						.UseHitboxes()
						.Ignore(this)
						.Size(1)
						.Run();

				var decalPath = "decals/blood_splatter_floor.decal";
				if(decalPath != null) {
					if(DecalDefinition.ByPath.TryGetValue(decalPath, out var decal)) {
						decal.PlaceUsingTrace(tr);
					}
				}
			}

			//For blood detail splatters on the ground
			for(int i = 0; i < 5; i++) {
				var trDir = pos + (Vector3.Down + (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 3 * 0.25f) * 100;
				var tr = Trace.Ray(pos, trDir)
						.UseHitboxes()
						.Ignore(this)
						.Size(1)
						.Run();

				var decalPath = "decals/blood_splatter.decal";
				if(decalPath != null) {
					if(DecalDefinition.ByPath.TryGetValue(decalPath, out var decal)) {
						decal.PlaceUsingTrace(tr);
					}
				}
			}

			// three slightly different particle effects, splash will be the most noticeable 
			var ps = Particles.Create("particles/blood_splash.vpcf", EyePos + Vector3.Down * 20);
			ps.SetForward(0, dir.Normal);

			ps = Particles.Create("particles/blood_drops.vpcf", EyePos + Vector3.Down * 20);
			ps.SetForward(0, dir.Normal);

			ps = Particles.Create("particles/blood_plip.vpcf", EyePos + Vector3.Down * 20);
			ps.SetForward(0, dir.Normal);
		}

		[ServerCmd]
		public void KillMyself(Entity attacker) {
			DamageInfo info = new();
			info.Damage = 200f;
			info.Attacker = attacker;
			info.Position = Position;
			TakeDamage(info);
			PlaySound("weaponhit");
		}

		public override void OnKilled() {
			Game.Current?.OnKilled(this);
			TimeSinceDied = 0;
			LifeState = LifeState.Dead;
			StopUsing();

			Inventory.DeleteContents();

			// create blood effects
			if(LastDamage.Attacker is SpeedDialPlayer attacker && attacker != this) {
				// someone killed someone, base the effect direction on the attacker
				BloodSplatter(EyePos + Vector3.Down * 20 - (attacker.EyePos + Vector3.Down * 20));
			} else {
				// suicide, effects just go down
				BloodSplatter();
			}

			// funny ragdoll moment
			BecomeRagdollOnClient(new Vector3(Velocity.x / 2, Velocity.y / 2, 300), GetHitboxBone(0));

			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;
		}
		DamageInfo LastDamage;

		public override void TakeDamage(DamageInfo info) {
			LastDamage = info;

			base.TakeDamage(info);

			if(info.Attacker is SpeedDialPlayer attacker && attacker != this) {
				// Note - sending this only to the attacker!
				attacker.DidDamage(To.Single(attacker), info.Position, info.Damage, Health);

				TookDamage(To.Single(this), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position);
			}
		}

		[ClientRpc]
		public void DidDamage(Vector3 pos, float amount, float healthinv) {
			if(healthinv <= 0) {
				int ScoreBase = SpeedDialGame.ScoreBase;
				ComboEvents(pos, ScoreBase * KillCombo);
			}
		}

		[ClientRpc]
		public void TookDamage(Vector3 pos) {
		}
	}
}
