using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SpeedDial.Player;
using SpeedDial.UI;
using SpeedDial.WeaponSpawns;
using SpeedDial.Settings;

namespace SpeedDial.Weapons {
	[Hammer.Skip]
	public partial class BaseSpeedDialWeapon : BaseCarriable {
		public virtual AmmoType AmmoType => AmmoType.Pistol;
		public virtual int ClipSize => 16;
		public virtual float ReloadTime => 0.17f;

		[Net]
		public Entity PreviousOwner { get; set; }
		[Net]
		public BaseWeaponSpawn WeaponSpawn { get; set; }

		[Net, Predicted]
		public int AmmoClip { get; set; }

		[Net, Local]
		public TimeSince TimeSinceDeployed { get; set; }

		public PickupTrigger PickupTrigger { get; protected set; }
		TimeSince lifetime;
		public bool DespawnAfterTime = true;

		[Net, Predicted]
		public TimeSince TimeSincePrimaryAttack { get; set; }

		public virtual float PrimaryRate => 5.0f;
		public virtual float SecondaryRate => 15.0f;
		public virtual int BulletCount => 1;
		public virtual float BulletSpread => 0.1f;
		public virtual float VerticalBulletSpread => 1f;
		public virtual float BulletForce => 1;
		public virtual float BulletDamage => 100;
		public virtual float BulletSize => 1;
		public virtual bool Automatic => false;
		public virtual string ShootSound => "sd_pistol_shoot";
		public virtual string WorldModel => "models/playermodels/weapons/prop_pistol.vmdl";
		public virtual Vector4 ScreenShakeParameters => new(1, 1, 1, 1);
		public virtual float Range => 4096;
		public virtual int AmmoPerShot => 1;
		public virtual float DeployTime => 0.6f;
		public virtual int HoldType => 2;
		public virtual string AttachementName => "pistol_attach";
		public virtual string EjectionParticle => "particles/pistol_ejectbrass.vpcf";
		public virtual bool Penetrate => false;
		[Net]
		public bool CanKill { get; set; } = true;

		public int AvailableAmmo() {
			if(Owner is SpeedDialPlayer owner) {
				if(owner == null) return 0;
				return owner.AmmoCount(AmmoType);
			}
			return -1;
		}

		public override void ActiveStart(Entity ent) {
			base.ActiveStart(ent);
			TimeSinceDeployed = 0;
		}

		public override void Spawn() {
			base.Spawn();

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs(CollisionLayer.Debris); // so player movement doesn't walk into it

			SetModel(WorldModel);

			AmmoClip = ClipSize;

			PickupTrigger = new();
			PickupTrigger.SetTriggerSize(32f);
			PickupTrigger.Parent = this;
			PickupTrigger.Position = Position;
			PickupTrigger.EnableTouchPersists = true;
		}

		public void ApplyThrowVelocity(Vector3 rot) {
			PhysicsBody.Velocity = Velocity + rot * 500;
			PhysicsBody.AngularVelocity = new Vector3(0, 0, 100f);
			PhysicsBody.GravityScale = 1.0f;
			_ = SetGravity();
		}

		[Event("server.tick")]
		public void CheckLifeTime() {
			if(lifetime > 10f && Owner == null && DespawnAfterTime) {
				Delete();
			}
		}

		async Task SetGravity() {
			await GameTask.DelaySeconds(0.2f);
			//if(PhysicsBody?.IsValid() ?? false)
			//PhysicsBody.GravityScale = 1.0f;
		}

		public override void SimulateAnimator(PawnAnimator anim) {
			anim.SetParam("holdtype", HoldType);
			anim.SetParam("aimat_weight", 1.0f);
		}

		public override void Simulate(Client owner) {

			lifetime = 0;

			if(!this.IsValid())
				return;

			if(Owner != null) {
				PreviousOwner = Owner;
			}

			if(TimeSinceDeployed < DeployTime)
				return;

			if(CanPrimaryAttack()) {
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}
		}

		public virtual bool CanPrimaryAttack() {
			if(Owner is not SpeedDialBotPlayer) {
				if(!Owner.IsValid() || (Automatic && !Input.Down(InputButton.Attack1)) || (!Automatic && !Input.Pressed(InputButton.Attack1))) return false;
			} else {
				if(!Owner.IsValid() || (Automatic && !(Owner as SpeedDialBotPlayer).ShootAtPlayer) || (!Automatic && !(Owner as SpeedDialBotPlayer).ShootAtPlayer) || (Owner as SpeedDialBotPlayer).TimeSinceShoot < (Owner as SpeedDialBotPlayer).ShootDelay) return false;
			}

			var rate = PrimaryRate;
			if(rate <= 0) return true;

			return TimeSincePrimaryAttack > (1 / rate);
		}

		public virtual void AttackPrimary(bool overrideBullet = false, bool overrideShootEffects = false) {
			TimeSincePrimaryAttack = 0;
			if(Owner is SpeedDialBotPlayer bot) {
				bot.TimeSinceShoot = 0f;
			}

			if(!overrideBullet) {
				if(!TakeAmmo(AmmoPerShot)) {

					PlaySound("sd_dryfrire");
					return;
				}// no ammo, no shooty shoot

				// shoot the bullets, bulletcount for something like a shotgun with multiple bullets
				for(int i = 0; i < BulletCount; i++) {

					this.ShootBullet(BulletSpread, BulletForce, BulletDamage, BulletSize, i);
				}
			}

			if(!overrideShootEffects) {

				// clientside shoot effects
				if(IsServer) {
					//Log.Info("TWICE?!?!");
					ShootEffects(); // muzzle and brass eject
				}
				PlaySound(ShootSound); // shoot sound

				(Owner as AnimEntity).SetAnimBool("b_attack", true); // shoot anim

			}
		}

		public virtual void ShootBullet(float spread, float force, float damage, float bulletSize, int seed) {
			float f = 1f;
			var player = Owner as SpeedDialPlayer;
			if(player.MedTaken && player.CurrentDrug == Meds.DrugType.Ritindi) {

				f = 0.25f;
			}
			Rand.SetSeed(Time.Tick + seed);


			var forward = Owner.EyeRot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f * f;
			forward = forward.Normal;
			//forward = new Vector3( forward.x, forward.y, forward.z * VerticalBulletSpread );
			forward.z *= VerticalBulletSpread;

			GamePanel.Current?.Bump();
			CrossHair.Current?.Bump();
			int index = 0;
			foreach(var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * Range, bulletSize)) {
				tr.Surface.DoBulletImpact(tr);

				// blood plip where player was hit
				if(tr.Entity is SpeedDialPlayer hitply) {
					var ps = Particles.Create("particles/blood/blood_plip.vpcf", tr.EndPos);
					ps?.SetForward(0, tr.Normal);
				}

				if(IsServer) {
					if(index == 0) {
						BulletTracer(EffectEntity.GetAttachment("muzzle", true).Value.Position, tr.EndPos);
					} else {
						BulletTracer(tr.StartPos, tr.EndPos);
					}
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

		public virtual float MaxWallbangDistance => 20;

		public virtual IEnumerable<TraceResult> TraceBullet(Vector3 start, Vector3 end, float size = 2.0f, float wallBangedDistance = 0) {

			var bullet = Trace.Ray(start, end)
					.UseHitboxes()
					.Ignore(Owner)
					.Ignore(this)
					.Size(size)
					.Run();

			yield return bullet;

			var player = Owner as SpeedDialPlayer;

			//Log.Info(SpeedDialGame.Instance.SniperCanPenetrate);

			if(this is Sniper && SpeedDialGame.Instance.SniperCanPenetrate) {
				var dir = (bullet.EndPos - bullet.StartPos).Normal;
				if(bullet.Hit && wallBangedDistance < MaxWallbangDistance) {
					var inNormal = bullet.Normal;
					var inPoint = bullet.EndPos - inNormal * (size / 2);
					//bullet
					//DebugOverlay.Line(start, inPoint, Color.Green, 10, false);
					//inpoint
					//DebugOverlay.Sphere(inPoint, 0.5f, Color.Green, false, 10);
					// normal
					//DebugOverlay.Line(inPoint, inPoint + inNormal * 3, Color.Magenta, 10, false);

					// adding dir to not be inside the inPoint
					var wallbangTest = Trace.Ray(inPoint + dir, inPoint + dir * (MaxWallbangDistance - 1))
									.HitLayer(CollisionLayer.WORLD_GEOMETRY)
									.Ignore(Owner)
									.Ignore(this)
									.Size(1)
									.Run();

					if(wallbangTest.Hit) {
						var outNormal = wallbangTest.Normal;
						var outPoint = wallbangTest.EndPos - outNormal * 0.5f;

						//outpoint
						//DebugOverlay.Sphere(outPoint, 0.1f, Color.Red, false, 10);

						if(outNormal != Vector3.Zero && inNormal.Dot(outNormal) >= 0) {
							//normal
							//DebugOverlay.Line(outPoint, outPoint + outNormal * 3, Color.Magenta, 10, false);

							//wallbang
							//DebugOverlay.Line(inPoint, outPoint, Color.Cyan, 10, false);

							var distance = (inPoint - outPoint).Length;
							var totalDistance = wallBangedDistance + distance;

							//Log.Info($"{distance} {totalDistance}");

							if(totalDistance < MaxWallbangDistance) {
								foreach(var bullet2 in TraceBullet(outPoint + dir * 2, outPoint + dir * 1000, 1, totalDistance)) {
									yield return bullet2;
								}
							}
						}
					}
				}
			}

			if(player.MedTaken && player.CurrentDrug == Meds.DrugType.Ollie || Penetrate) {
				// pierce through the first player hit
				if(bullet.Entity is SpeedDialPlayer) {
					var dir = bullet.EndPos - bullet.StartPos;
					var penetrate = Trace.Ray(bullet.EndPos, bullet.EndPos + dir.Normal * 100f)
							.UseHitboxes()
							.Ignore(this)
							.Ignore(bullet.Entity)
							.Size(size)
							.Run();

					yield return penetrate;
				} else {
					// ricochet off the wall
					var inDir = bullet.EndPos - bullet.StartPos;
					float dot = Vector3.Dot(inDir.Normal, bullet.Normal);

					if(dot < 0) {
						var dir = Vector3.Reflect(inDir, bullet.Normal).WithZ(0);
						var ricochet = Trace.Ray(bullet.EndPos, end + dir * Range)
								.UseHitboxes()
								.Ignore(Owner)
								.Ignore(this)
								.Size(size)
								.Run();

						yield return ricochet;
					}
				}
			}
		}

		[ClientRpc]
		protected virtual void ShootEffects() {
			Host.AssertClient();
			Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
			Particles.Create(EjectionParticle, EffectEntity, "ejection_point");
			//Log.Info("TEST PLEASE");
			if(IsLocalPawn) {
				_ = new Sandbox.ScreenShake.Perlin(ScreenShakeParameters.x, ScreenShakeParameters.y, ScreenShakeParameters.z, ScreenShakeParameters.w);
			}
		}

		[ClientRpc]
		protected virtual void BulletTracer(Vector3 from, Vector3 to) {
			var ps = Particles.Create("particles/weapon_fx/sd_bullet_trail/sd_bullet_trail.vpcf", to);
			ps.SetPosition(0, from);
			ps.SetPosition(1, to);
		}

		public bool TakeAmmo(int amount) {
			if(Debug.InfiniteAmmo) return true;
			if(AmmoClip < amount)
				return false;

			AmmoClip -= amount;
			return true;
		}

		public override void OnCarryStart(Entity carrier) {
			if(IsClient) return;

			CanKill = true;

			//spawned via a weaponspawn. Tell the spawn that it's cleared up and can start respawning the weapon
			if(WeaponSpawn != null) {
				WeaponSpawn.ItemTaken = true;
				WeaponSpawn.TimeSinceTaken = 0;
				WeaponSpawn = null;
			}

			SetParent(carrier, AttachementName, Transform.Zero);

			Owner = carrier;
			MoveType = MoveType.None;
			EnableAllCollisions = false;
			EnableDrawing = false;

			if(PickupTrigger.IsValid()) {
				PickupTrigger.EnableTouch = false;
			}
		}

		public override void OnCarryDrop(Entity dropper) {
			base.OnCarryDrop(dropper);

			if(PickupTrigger.IsValid()) {
				PickupTrigger.EnableTouch = true;
			}
		}
	}
}
