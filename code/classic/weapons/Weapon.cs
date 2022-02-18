using System.Collections.Generic;
using System.Linq;

using SpeedDial.Classic.Entities;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Drugs;
using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Weapons;

public partial class Weapon : BaseCarriable {
	[Net] public WeaponBlueprint Blueprint { get; private set; }
	public string WeaponClass => Blueprint.WeaponClass;

	public void ApplyBlueprint(WeaponBlueprint blueprint) {
		Blueprint = blueprint;
		Model = blueprint.WorldModel;
		AmmoClip = Blueprint.ClipSize;
	}

	[Net, Predicted] public int AmmoClip { get; set; }
	[Net, Predicted] public TimeSince TimeSincePrimaryAttack { get; set; }

	// burst
	[Net, Predicted] public int Burst { get; set; }
	[Net, Predicted] public bool Firing { get; set; }
	[Net, Predicted] public TimeSince TimeSinceSpecial { get; set; }

	[Net] public TimeSince TimeSinceAlive { get; set; }
	[Net] public bool DespawnAfterTime { get; set; }
	[Net] public Entity PreviousOwner { get; set; }
	[Net] public bool CanImpactKill { get; set; } = true;

	public BasePickupTrigger PickupTrigger { get; protected set; }
	public ClassicWeaponSpawn WeaponSpawn { get; set; }

	public override void Spawn() {
		base.Spawn();

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs(CollisionLayer.Debris);

		PickupTrigger = new();
		PickupTrigger.Parent = this;
		PickupTrigger.Position = Position;
		PickupTrigger.EnableTouchPersists = true;

		GlowDistanceStart = 0;
		GlowDistanceEnd = 1000;

		GlowColor = Color.White;

		GlowState = GlowStates.On;
		GlowActive = true;
	}

	[SpeedDialEvent.Gamemode.Reset]
	public void GamemodeReset() {
		// despawn any guns laying around
		if(WeaponSpawn is null && Owner is null) {
			Delete();
		}
	}

	private void SetGlow(bool state) {
		if(state) {
			GlowState = GlowStates.On;
			GlowActive = true;

			if(AmmoClip > 0)
				GlowColor = new Color(0.2f, 1, 0.2f, 1);
			else {
				if(AmmoClip == -1)
					GlowColor = new Color(1, 1, 1, 1);
				else
					GlowColor = new Color(1, 0.2f, 0.2f, 1);
			}
		} else {
			GlowState = GlowStates.Off;
			GlowActive = false;
		}
	}

	[Event.Tick]
	public void Tick() {
		if(TimeSinceAlive > 10 && Owner == null && DespawnAfterTime && PickupTrigger.IsValid() && !PickupTrigger.TouchingPlayers.Any()) {
			if(IsAuthority)
				Delete();
		}
		var attach = GetAttachment("throw_pivot");
		if(attach is not null) {
			DebugOverlay.Line(attach.Value.Position, attach.Value.Position + attach.Value.Rotation.Forward * 10, Color.Red, Time.Delta, false);
			DebugOverlay.Line(Position, Position + Rotation.Forward * 10, Color.Green, Time.Delta, false);
			DebugOverlay.Line(Position, Position + Vector3.Up * 10, Color.Blue, Time.Delta, false);
		}
		if(Debug.Weapons) {
			if(IsServer)
				DebugOverlay.Text(Position, $"{GetType().Name}\nalive since: {TimeSinceAlive}\ndespawn: {DespawnAfterTime}", Owner is null ? Color.White : Color.Green, Time.Delta, 1000);
		}
	}

	public override void SimulateAnimator(PawnAnimator anim) {
		anim.SetParam("holdtype", (int)Blueprint.HoldType);
		anim.SetParam("aimat_weight", 1.0f);
	}

	public override void Simulate(Client owner) {
		TimeSinceAlive = 0;

		if(!this.IsValid())
			return;

		if(Owner != null) {
			PreviousOwner = Owner;
		}

		if(CanPrimaryAttack()) {
			using(LagCompensation()) {
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}
		}

		if(Blueprint.Special == WeaponSpecial.Burst) {
			BurstSimulate();
		} else if(Blueprint.Special == WeaponSpecial.Melee) {
			MeleeSimulate();
		}
	}

	public virtual bool CanPrimaryAttack() {
		if((Owner as ClassicPlayer).Frozen) return false;
		if(Owner is ClassicPlayer) {
			if(!Owner.IsValid() || (Blueprint.FireMode == WeaponFireMode.Automatic && !Input.Down(InputButton.Attack1)) || (!(Blueprint.FireMode == WeaponFireMode.Automatic) && !Input.Pressed(InputButton.Attack1))) return false;
		}

		var rate = Blueprint.FireRate;
		if(rate <= 0) return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	public virtual void AttackPrimary() {
		if(Blueprint.Special == WeaponSpecial.Burst) {
			BurstPrimary();
		} else if(Blueprint.Special == WeaponSpecial.Melee) {
			MeleePrimary();
		} else {
			TimeSincePrimaryAttack = 0;

			if(!TakeAmmo(Blueprint.AmmoPerShot)) {
				PlaySound("sd_dryfrire");
				return;
			}// no ammo, no shooty shoot

			// shoot the bullets, bulletcount for something like a shotgun with multiple bullets
			for(int i = 0; i < Blueprint.BulletCount; i++) {
				ShootBullet(Blueprint.BulletSpread, Blueprint.BulletForce, Blueprint.BulletDamage, Blueprint.BulletSize, i);
			}

			ShootEffects();
		}
	}

	public virtual void ShootEffects() {
		if(IsLocalPawn) {
			WeaponPanel.Fire(Blueprint.UIEffectsScalar);
			Crosshair.Fire();
		}
		Particles.Create(Blueprint.MuzzleParticle, EffectEntity, Blueprint.MuzzleAttach);
		Particles.Create(Blueprint.EjectParticle, EffectEntity, Blueprint.EjectAttach);
		PlaySound(Blueprint.ShootSound);
		(Owner as AnimEntity).SetAnimBool("b_attack", true); // shoot anim
	}

	public virtual void ShootBullet(float spread, float force, float damage, float bulletSize, int seed) {
		Rand.SetSeed(Time.Tick + seed);

		var player = Owner as ClassicPlayer;

		var forward = Owner.EyeRotation.Forward;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f * ((player.ActiveDrug && player.DrugType is DrugType.Ritindi) ? 0.25f : 1f);
		forward = forward.Normal;
		forward.z *= Blueprint.VerticalSpreadMultiplier;

		int index = 0;
		foreach(var tr in TraceBullet(Owner.EyePosition, Owner.EyePosition + forward * Blueprint.BulletRange, bulletSize)) {
			tr.Surface.DoBulletImpact(tr);

			// blood plip where player was hit
			if(tr.Entity is ClassicPlayer hitply) {
				var ps = Particles.Create("particles/blood/blood_plip.vpcf", tr.EndPos);
				ps?.SetForward(0, tr.Normal);
			}

			if(index == 0) {
				BulletTracer(EffectEntity.GetAttachment("muzzle", true).Value.Position, tr.EndPos);
			} else {
				BulletTracer(tr.StartPos, tr.EndPos);
			}

			index++;

			if(!IsServer) continue;
			if(!tr.Entity.IsValid()) continue;

			using(Prediction.Off()) {
				var damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
					.UsingTraceResult(tr)
					.WithAttacker(Owner)
					.WithWeapon(this);

				tr.Entity.TakeDamage(damageInfo);
			}
		}
	}

	public virtual void BulletTracer(Vector3 from, Vector3 to) {
		var ps = Particles.Create("particles/weapon_fx/sd_bullet_trail/sd_bullet_trail.vpcf", to);
		if(ps is not null) {
			ps.SetPosition(0, from);
			ps.SetPosition(1, to);
		}
	}

	public virtual float MaxWallbangDistance => 32;

	public virtual IEnumerable<TraceResult> TraceBullet(Vector3 start, Vector3 end, float size = 2.0f, float wallBangedDistance = 0) {

		var bullet = Trace.Ray(start, end)
				.UseHitboxes()
				.Ignore(Owner)
				.Ignore(this)
				.Size(size)
				.Run();

		if(Debug.Weapons) {
			DebugOverlay.TraceResult(bullet, 0.5f);
		}

		yield return bullet;

		var player = Owner as ClassicPlayer;

		if(Blueprint.Penetrate) {
			var dir = (bullet.EndPos - bullet.StartPos).Normal;
			if(bullet.Hit && wallBangedDistance < MaxWallbangDistance) {
				var inNormal = bullet.Normal;
				var inPoint = bullet.EndPos - inNormal * (size / 2);

				// adding dir to not be inside the inPoint
				var wallbangTest = Trace.Ray(inPoint + dir, inPoint + dir * (MaxWallbangDistance - 1))
								.HitLayer(CollisionLayer.WORLD_GEOMETRY)
								.Ignore(Owner)
								.Ignore(this)
								.Size(1)
								.Run();

				if(Debug.Weapons) {
					DebugOverlay.TraceResult(wallbangTest, 0.5f);
				}

				if(wallbangTest.Hit) {
					var outNormal = wallbangTest.Normal;
					var outPoint = wallbangTest.EndPos - outNormal * 0.5f;

					if(outNormal != Vector3.Zero && inNormal.Dot(outNormal) >= 0) {

						var distance = (inPoint - outPoint).Length;
						var totalDistance = wallBangedDistance + distance;

						if(totalDistance < MaxWallbangDistance) {
							foreach(var bullet2 in TraceBullet(outPoint + dir * 2, outPoint + dir * 1000, 1, totalDistance)) {
								yield return bullet2;
							}
						}
					}
				}
			}
		}

		if(player.ActiveDrug && player.DrugType == DrugType.Ollie) {
			// pierce through the first player hit
			if(bullet.Entity is ClassicPlayer) {
				var dir = bullet.EndPos - bullet.StartPos;
				var penetrate = Trace.Ray(bullet.EndPos, bullet.EndPos + dir.Normal * 100f)
						.UseHitboxes()
						.Ignore(this)
						.Ignore(bullet.Entity)
						.Size(size)
						.Run();

				if(Debug.Weapons) {
					DebugOverlay.TraceResult(penetrate, 0.5f);
				}

				yield return penetrate;
			} else {
				// ricochet off the wall
				var inDir = bullet.EndPos - bullet.StartPos;
				float dot = Vector3.Dot(inDir.Normal, bullet.Normal);

				if(dot < 0) {
					var dir = Vector3.Reflect(inDir, bullet.Normal).WithZ(0);
					var ricochet = Trace.Ray(bullet.EndPos, end + dir * Blueprint.BulletRange)
							.UseHitboxes()
							.Ignore(Owner)
							.Ignore(this)
							.Size(size)
							.Run();

					if(Debug.Weapons) {
						DebugOverlay.TraceResult(ricochet, 0.5f);
					}

					yield return ricochet;
				}
			}
		}
	}

	public bool TakeAmmo(int amount) {
		if(Debug.InfiniteAmmo) return true;
		if(AmmoClip < amount)
			return false;

		AmmoClip -= amount;
		return true;
	}

	public override void OnCarryStart(Entity carrier) {
		if(IsClient || !carrier.IsValid() || carrier is not BasePlayer player) return;

		CanImpactKill = true;

		//spawned via a weaponspawn. Tell the spawn that it's cleared up and can start respawning the weapon
		if(WeaponSpawn is not null) {
			WeaponSpawn.WeaponTaken();
			WeaponSpawn = null;
		}

		Parent = carrier;
		SetParent(player, Blueprint.HoldAttach, Transform.Zero);

		Owner = player;
		MoveType = MoveType.None;
		EnableAllCollisions = false;

		SetGlow(false);

		if(PickupTrigger.IsValid()) {
			PickupTrigger.EnableTouch = false;
		}

		// TODO: get pickup sound for weapons without ammo (bat)
		if(AmmoClip > 0) {
			BasePlayer.SoundFromWorld(To.Single(player.Client), "sd_pickup.loaded", Position);
		} else {
			BasePlayer.SoundFromWorld(To.Single(player.Client), "sd_pickup.empty", Position);
		}
	}

	public override void OnCarryDrop(Entity dropper) {
		base.OnCarryDrop(dropper);

		if(PickupTrigger.IsValid()) {
			PickupTrigger.EnableTouch = true;
		}

		DespawnAfterTime = true;
		SetGlow(true);
	}
}
