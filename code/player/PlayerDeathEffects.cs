using System.Numerics;
using System.Diagnostics;
using System;
using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.UI;
using System.Threading.Tasks;

namespace SpeedDial.Player {
	public enum COD {
		Gunshot,
		Melee,
		Thrown,
		Explosive,
		HeartAttack
	}

	public partial class SpeedDialPlayer {
		[Net]
		public COD CauseOfDeath { get; set; } = COD.HeartAttack;

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
				var tr = Sandbox.Trace.Ray(pos, trDir)
						.UseHitboxes()
						.Ignore(this)
						.Size(1)
						.Run();

				_ = CreateDecalAsync("decals/blood_splatter.decal", tr, i * 0.05f);
			}

			//For blood splatter on the ground, pool of blood essentially
			for(int i = 0; i < 5; i++) {
				var trDir = pos + (Vector3.Down + (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 3 * 0.25f) * 100;
				var tr = Sandbox.Trace.Ray(pos, trDir)
						.WorldOnly()
						.Ignore(this)
						.Size(1)
						.Run();

				_ = CreateDecalAsync("decals/blood_splatter_floor.decal", tr, i * 0.05f);
			}

			//For blood detail splatters on the ground
			for(int i = 0; i < 5; i++) {
				var trDir = pos + (Vector3.Down + (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 3 * 0.25f) * 100;
				var tr = Sandbox.Trace.Ray(pos, trDir)
						.WorldOnly()
						.Ignore(this)
						.Size(1)
						.Run();

				_ = CreateDecalAsync("decals/blood_splatter.decal", tr, i * 0.1f);
			}

			// three slightly different particle effects, splash will be the most noticeable 
			_ = CreateParticleAsync("particles/blood/blood_splash.vpcf", Corpse, dir.Normal, 0, "head");

			_ = CreateParticleAsync("particles/blood/blood_drops.vpcf", Corpse, Vector3.Down, 0.5f, "head", false, true, 3);

			_ = CreateParticleAsync("particles/blood/blood_plip.vpcf", Corpse, Vector3.Down, 0, "head", true);
		}

		async Task CreateDecalAsync(string decalname, TraceResult tr, float delay = 0) {
			await GameTask.DelaySeconds(delay);

			var decalPath = decalname;
			if(decalPath != null) {
				if(DecalDefinition.ByPath.TryGetValue(decalPath, out var decal)) {
					decal.PlaceUsingTrace(tr);
				}
			}
		}

		async Task CreateParticleAsync(string particle, Entity entity, Vector3 forward, float delay = 0, string bone = "root", bool attach = false, bool bloodpool = false, int pools = 1) {
			await GameTask.DelaySeconds(delay);
			if(entity is ModelEntity ent) {
				var boneBody = ent.GetBonePhysicsBody(ent.GetBoneIndex(bone));
				var ps = Particles.Create(particle, boneBody.Position);
				ps.SetForward(0, forward);
				if(attach)
					ps.SetEntityAttachment(0, entity, "head_blood", true);
				if(bloodpool) {
					for(int i = 0; i < pools; i++) {
						await GameTask.DelaySeconds(i * 0.1f);
						var trDir = boneBody.Position + Vector3.Down * 1000;
						var tr = Sandbox.Trace.Ray(boneBody.Position, trDir)
								.WorldAndEntities()
								.UseHitboxes()
								.Ignore(this)
								.Size(1)
								.Run();

						_ = CreateDecalAsync("decals/blood_splatter_floor.decal", tr, 0.5f);
					}
				}
			}
		}

		[ServerCmd]
		public void KillMyself(Entity attacker) {
			DamageInfo info = new();
			info.Damage = 200f;
			info.Attacker = attacker;
			info.Position = Position;
			TakeDamage(info);
			PlaySound("smack");
		}

		public override void OnKilled() {
			Game.Current?.OnKilled(this);
			TimeSinceDied = 0;
			LifeState = LifeState.Dead;
			StopUsing();

			Inventory.DeleteContents();

			// spawn ragdoll before effects to spawn particles on the body
			BecomeRagdollOnClient(new Vector3(Velocity.x / 2, Velocity.y / 2, 300), GetHitboxBone(0));

			// create blood effects
			if(LastDamage.Attacker is SpeedDialPlayer attacker && attacker != this) {
				// someone killed someone, base the effect direction on the attacker
				BloodSplatter(EyePos + Vector3.Down * 20 - (attacker.EyePos + Vector3.Down * 20));


			} else {
				// suicide, effects just go down
				BloodSplatter();
			}

			(Controller as SpeedDialController).Freeze = true;
			(Camera as SpeedDialCamera).Freeze = true;

			EnableAllCollisions = false;
			EnableDrawing = false;
		}
		DamageInfo LastDamage;

		public override void TakeDamage(DamageInfo info) {
			LastDamage = info;

			base.TakeDamage(info);

			if(info.Attacker is SpeedDialPlayer attacker && attacker != this) {
				if(Health <= 0) {
					attacker.KillCombo++;
					Log.Info($"TEST { attacker.KillCombo }");
				}
				// Note - sending this only to the attacker!
				attacker.DidDamage(To.Single(attacker), info.Position, info.Damage, Health, CauseOfDeath, attacker.KillCombo);


				TookDamage(To.Single(this), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position);
			}
		}

		[ClientRpc]
		public void DidDamage(Vector3 pos, float amount, float healthinv, COD death, int combo) {
			if(healthinv <= 0) {
				Log.Info(combo);
				int ScoreBase = SpeedDialGame.ScoreBase;
				ComboEvents(pos, ScoreBase * combo, combo, death);
				Sound.FromWorld("kill_confirm", pos); // should this sound play on every kill? maybe a bit too high pitch for that
			}
		}

		[ClientRpc]
		public void TookDamage(Vector3 pos) {
		}
	}
}
