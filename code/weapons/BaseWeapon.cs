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
	public partial class BaseSpeedDialWeapon : BaseWeapon {
		public virtual AmmoType AmmoType => AmmoType.Pistol;
		public virtual int ClipSize => 16;
		public virtual float ReloadTime => 0.17f;
		public virtual int Bucket => 1;
		public virtual int BucketWeight => 100;

		[Net]
		public Entity previousOwner { get; set; }

		[Net, Predicted]
		public int AmmoClip { get; set; }

		[Net, Predicted]
		public TimeSince TimeSinceReload { get; set; }

		[Net, Predicted]
		public bool IsReloading { get; set; }

		[Net, Predicted]
		public TimeSince TimeSinceDeployed { get; set; }

		public PickupTrigger PickupTrigger { get; protected set; }
		TimeSince lifetime;


		public virtual int AmmoToAward => 5;

		public int AvailableAmmo() {
			if(Owner is SpeedDialPlayer owner) {
				if(owner == null) return 0;
				return owner.AmmoCount(AmmoType);
			}
			return -1;
		}

		public override void ActiveStart(Entity ent) {
			base.ActiveStart(ent);
			Log.Info("Weapon Active Start");
			TimeSinceDeployed = 0;
		}

		public override void Spawn() {
			base.Spawn();

			SetModel("weapons/rust_pistol/rust_pistol.vmdl");

			PickupTrigger = new();
			PickupTrigger.Parent = this;
			PickupTrigger.Position = Position;
			PickupTrigger.EnableTouchPersists = true;
		}

		public void ApplyThrowVelocity(Vector3 rot) {
			PhysicsBody.Velocity = Velocity + (rot) * 500;
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
			await Task.DelaySeconds(0.2f);
			if(PhysicsBody.IsValid())
				PhysicsBody.GravityScale = 1.0f;
		}

		public override void Reload() {
			if(IsReloading)
				return;

			if(AmmoClip >= ClipSize)
				return;

			TimeSinceReload = 0;

			if(Owner is SpeedDialPlayer player) {
				if(player.AmmoCount(AmmoType) <= 0)
					return;

				StartReloadEffects();
			}

			IsReloading = true;

			(Owner as AnimEntity).SetAnimBool("b_reload", true);

			StartReloadEffects();
		}

		public override void Simulate(Client owner) {

			lifetime = 0;

			if(TimeSinceDeployed < 0.6f)
				return;

			if(!this.IsValid())
				return;

			if(CanPrimaryAttack()) {
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}

			if(CanSecondaryAttack()) {
				TimeSinceSecondaryAttack = 0;
				AttackSecondary();
			}

			if(Owner != null) {
				previousOwner = Owner;
			}

			if(TimeSinceDeployed < 0.6f)
				return;

			if(IsReloading && TimeSinceReload > ReloadTime) {
				OnReloadFinish();
			}
		}

		public virtual void OnReloadFinish() {
			IsReloading = false;
		}

		[ClientRpc]
		public virtual void StartReloadEffects() {
			// ex viewmodel shit
		}

		public override void AttackPrimary() {
			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = 0;

			// Tell the clients to play the shoot effects
			ShootEffects();

			// ShootBullet is coded in a way where we can have bullets pass through shit
			// or bounce off shit, in which case it'll return multiple results
			foreach(var tr in TraceBullet(Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 5000)) {
				tr.Surface.DoBulletImpact(tr);

				if(!IsServer) continue;
				if(!tr.Entity.IsValid()) continue;

				// We turn predictiuon off for this, so aany exploding effects don't get culled etc
				using(Prediction.Off()) {
					var damage = DamageInfo.FromBullet(tr.EndPos, Owner.EyeRot.Forward * 100, 15)
						.UsingTraceResult(tr)
						.WithAttacker(Owner)
						.WithWeapon(this);

					tr.Entity.TakeDamage(damage);
				}
			}
		}

		[ClientRpc]
		protected virtual void ShootEffects() {
			Host.AssertClient();

			Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");

			if(IsLocalPawn) {
				new Sandbox.ScreenShake.Perlin();
			}

			CrosshairPanel?.OnEvent("fire");
		}

		public virtual void ShootBullet(float spread, float force, float damage, float bulletSize) {
			var forward = Owner.EyeRot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			AmmoPanel.Current?.Bump();

			//
			// ShootBullet is coded in a way where we can have bullets pass through shit
			// or bounce off shit, in which case it'll return multiple results
			//
			foreach(var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * 5000, bulletSize)) {
				tr.Surface.DoBulletImpact(tr);

				if(!IsServer) continue;
				if(!tr.Entity.IsValid()) continue;

				//
				// We turn predictiuon off for this, so any exploding effects don't get culled etc
				//
				using(Prediction.Off()) {
					var damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
						.UsingTraceResult(tr)
						.WithAttacker(Owner)
						.WithWeapon(this);

					tr.Entity.TakeDamage(damageInfo);
				}
			}
		}

		public bool TakeAmmo(int amount) {
			if(AmmoClip < amount)
				return false;

			AmmoClip -= amount;
			return true;
		}

		public void AwardAmmo() {
			if(IsClient)
				Log.Info("Updated clip on client in weapon");
			if(IsServer)
				Log.Info("Updated clip on server in weapon");
			AmmoClip = Math.Clamp(AmmoClip + AmmoToAward, 0, ClipSize);
		}

		[ClientRpc]
		public virtual void DryFire() {
			// CLICK
		}

		public bool IsUsable() {
			if(AmmoClip > 0) return true;
			return AvailableAmmo() > 0;
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
