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
namespace SpeedDial.Weapons {
	public partial class BaseSpeedDialWeapon : BaseCarriable {
		public virtual AmmoType AmmoType => AmmoType.Pistol;
		public virtual int ClipSize => 16;
		public virtual float ReloadTime => 0.17f;

		[Net]
		public Entity previousOwner { get; set; }

		[Net, Predicted]
		public int AmmoClip { get; set; }

		[Net, Local]
		public TimeSince TimeSinceDeployed { get; set; }

		public PickupTrigger PickupTrigger { get; protected set; }
		TimeSince lifetime;

		[Net, Predicted]
		public TimeSince TimeSincePrimaryAttack { get; set; }

		public virtual float PrimaryRate => 5.0f;
		public virtual float SecondaryRate => 15.0f;
		public virtual int BulletCount => 1;
		public virtual float BulletSpread => 0.1f;
		public virtual float BulletForce => 1;
		public virtual float BulletDamage => 100;
		public virtual float BulletSize => 1;
		public virtual bool Automatic => false;
		public virtual string ShootSound => "rust_pistol.shoot";
		public virtual string WorldModel => "models/playermodels/weapons/prop_pistol.vmdl";
		public virtual Vector4 ScreenShakeParameters => new(1, 1, 1, 1);
		public virtual float Range => 4096;
		public virtual int AmmoPerShot => 1;
		public virtual float DeployTime => 0.6f;

		public virtual int HoldType => 1;

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
			PickupTrigger.Parent = this;
			PickupTrigger.Position = Position;
			PickupTrigger.EnableTouchPersists = true;
		}

		public void ApplyThrowVelocity(Vector3 rot) {
			PhysicsBody.Velocity = Velocity + rot * 500;
			PhysicsBody.AngularVelocity = new Vector3(0, 0, 100f);
			PhysicsBody.GravityScale = 0.0f;
			_ = SetGravity();
		}

		[Event("server.tick")]
		public void CheckLifeTime() {
			if(lifetime > 5f && Owner == null) {
				Delete();
			}
		}

		async Task SetGravity() {
			await GameTask.DelaySeconds(0.2f);
			if(PhysicsBody.IsValid())
				PhysicsBody.GravityScale = 1.0f;
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
				previousOwner = Owner;
			}

			if(TimeSinceDeployed < DeployTime)
				return;

			if(CanPrimaryAttack()) {
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}
		}

		public virtual bool CanPrimaryAttack() {
			if(!Owner.IsValid() || (Automatic && !Input.Down(InputButton.Attack1)) || (!Automatic && !Input.Pressed(InputButton.Attack1))) return false;

			var rate = PrimaryRate;
			if(rate <= 0) return true;

			return TimeSincePrimaryAttack > (1 / rate);
		}

		public virtual void AttackPrimary() {
			TimeSincePrimaryAttack = 0;

			if(!TakeAmmo(AmmoPerShot)) return;

			// Tell the clients to play the shoot effects
			ShootEffects();

			PlaySound(ShootSound);

			(Owner as AnimEntity).SetAnimBool("b_attack", true);

			// shoot the bullets, bulletcount for something like a shotgun with multiple bullets
			for(int i = 0; i < BulletCount; i++) {
				ShootBullet(BulletSpread, BulletForce, BulletDamage, BulletSize);
			}
		}

		public virtual void ShootBullet(float spread, float force, float damage, float bulletSize) {
			float f = 1f;
			var player = Owner as SpeedDialPlayer;
			if(player.MedTaken && player.CurrentDrug == Meds.DrugType.Ritindi) {

				f = 0.25f;
			}

			var forward = Owner.EyeRot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f * f;
			forward = forward.Normal;

			AmmoPanel.Current?.Bump();
			int index = 0;
			foreach(var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * Range, bulletSize)) {
				tr.Surface.DoBulletImpact(tr);

				if(index == 0) {
					BulletTracer(EffectEntity.GetAttachment("muzzle", true).Position, tr.EndPos);
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

		public virtual IEnumerable<TraceResult> TraceBullet(Vector3 start, Vector3 end, float radius = 2.0f) {

			var bullet = Trace.Ray(start, end)
					.UseHitboxes()
					.Ignore(Owner)
					.Ignore(this)
					.Size(radius)
					.Run();

			yield return bullet;

			var player = Owner as SpeedDialPlayer;

			if(player.MedTaken && player.CurrentDrug == Meds.DrugType.Ollie) {
				// pierce through the first player hit
				if(bullet.Entity is SpeedDialPlayer) {
					var dir = bullet.EndPos - bullet.StartPos;
					var penetrate = Trace.Ray(bullet.EndPos, bullet.EndPos + dir.Normal * 100f)
							.UseHitboxes()
							.Ignore(this)
							.Ignore(bullet.Entity)
							.Size(radius)
							.Run();

					yield return penetrate;
				} else {
					// ricochet off the wall
					var inDir = bullet.EndPos - bullet.StartPos;
					float dot = Vector3.Dot(inDir.Normal, bullet.Normal);

					if(dot < 0) {
						var dir = Vector3.Reflect(inDir, bullet.Normal);
						var ricochet = Trace.Ray(bullet.EndPos, end + dir * Range)
								.UseHitboxes()
								.Ignore(Owner)
								.Ignore(this)
								.Size(radius)
								.Run();

						yield return ricochet;
					}
				}
			}
		}

		[ClientRpc]
		protected virtual void ShootEffects() {
			Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
			Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");

			if(IsLocalPawn) {
				new Sandbox.ScreenShake.Perlin(ScreenShakeParameters.x, ScreenShakeParameters.y, ScreenShakeParameters.z, ScreenShakeParameters.w);
			}

			CrosshairPanel?.OnEvent("fire");
		}

		[ClientRpc]
		protected virtual void BulletTracer(Vector3 from, Vector3 to) {
			var ps = Particles.Create("particles/weapon_fx/bullet_trail.vpcf", to);
			ps.SetPos(0, from);
			ps.SetPos(1, to);
		}

		public bool TakeAmmo(int amount) {
			if(AmmoClip < amount)
				return false;

			AmmoClip -= amount;
			return true;
		}

		public override void OnCarryStart(Entity carrier) {
			base.OnCarryStart(carrier);

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
