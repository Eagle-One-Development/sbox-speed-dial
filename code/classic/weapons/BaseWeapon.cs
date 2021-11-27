using System.Collections.Generic;
using System.Linq;
using System;

using Sandbox;

using SpeedDial.Classic.Entities;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Drugs;
using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Weapons {
	[Hammer.Skip]
	[Library(Spawnable = false)]
	public partial class ClassicBaseWeapon : BaseCarriable {
		public virtual int ClipSize => 16;
		public virtual float ReloadTime => 0.17f;

		[Net]
		public Entity PreviousOwner { get; set; }
		public ClassicWeaponSpawn WeaponSpawn { get; set; }

		[Net, Predicted]
		public int AmmoClip { get; set; }

		public BasePickupTrigger PickupTrigger { get; protected set; }
		[Net] public TimeSince TimeSinceAlive { get; set; }
		public bool DespawnAfterTime = false;
		[Net, Predicted]
		public TimeSince TimeSincePrimaryAttack { get; set; }
		public virtual float PrimaryRate => 5.0f;
		public virtual int BulletCount => 1;
		public virtual float BulletSpread => 0.1f;
		public virtual float VerticalBulletSpread => 1f;
		public virtual float BulletForce => 1;
		public virtual float BulletDamage => 100;
		public virtual float BulletSize => 1;
		public virtual bool Automatic => false;
		public virtual string ShootSound => "sd_pistol_shoot";
		public virtual string WorldModel => "models/light_arrow.vmdl";
		public virtual Vector4 ScreenShakeParameters => new(1, 1, 1, 1);
		public virtual float Range => 4096;
		public virtual int AmmoPerShot => 1;
		public virtual int HoldType => 2;
		public virtual string AttachementName => "pistol_attach";
		public virtual string EjectionParticle => "particles/pistol_ejectbrass.vpcf";
		[Net] public bool CanImpactKill { get; set; } = true;

		public override void Spawn() {
			base.Spawn();

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs(CollisionLayer.Debris); // so player movement doesn't walk into it

			SetModel(WorldModel);

			AmmoClip = ClipSize;

			PickupTrigger = new();
			PickupTrigger.Parent = this;
			PickupTrigger.ParentEntity = this;
			PickupTrigger.Position = Position;
			PickupTrigger.EnableTouchPersists = true;

			GlowDistanceStart = 0;
			GlowDistanceEnd = 1000;

			GlowColor = Color.White;

			GlowState = GlowStates.GlowStateOn;
			GlowActive = true;
		}

		private void SetGlow(bool state) {
			if(state) {
				GlowState = GlowStates.GlowStateOn;
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
				GlowState = GlowStates.GlowStateOff;
				GlowActive = false;
			}
		}

		[Event.Tick]
		public void Tick() {
			if(TimeSinceAlive > 10 && Owner == null && DespawnAfterTime && PickupTrigger.IsValid() && !PickupTrigger.TouchingPlayers.Any()) {
				if(IsServer)
					Delete();
			}
			if(Debug.Weapons) {
				if(IsServer)
					DebugOverlay.Text(Position, $"{GetType().Name}\nalive since: {TimeSinceAlive}\ndespawn: {DespawnAfterTime}", Owner is null ? Color.White : Color.Green, Time.Delta, 1000);
			}
		}


		public override void SimulateAnimator(PawnAnimator anim) {
			anim.SetParam("holdtype", HoldType);
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
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}
		}

		public virtual bool CanPrimaryAttack() {
			if((Owner as ClassicPlayer).Frozen) return false;
			if(Owner is ClassicPlayer) {
				if(!Owner.IsValid() || (Automatic && !Input.Down(InputButton.Attack1)) || (!Automatic && !Input.Pressed(InputButton.Attack1))) return false;
			}
			// else {
			// 	if(!Owner.IsValid() || (Automatic && !(Owner as SpeedDialBotPlayer).ShootAtPlayer) || (!Automatic && !(Owner as SpeedDialBotPlayer).ShootAtPlayer) || (Owner as SpeedDialBotPlayer).TimeSinceShoot < (Owner as SpeedDialBotPlayer).ShootDelay) return false;
			// }

			var rate = PrimaryRate;
			if(rate <= 0) return true;

			return TimeSincePrimaryAttack > (1 / rate);
		}

		public virtual void AttackPrimary(bool overrideBullet = false, bool overrideShootEffects = false) {
			TimeSincePrimaryAttack = 0;
			// if(Owner is SpeedDialBotPlayer bot) {
			// 	bot.TimeSinceShoot = 0f;
			// }

			if(!overrideBullet) {
				if(!TakeAmmo(AmmoPerShot)) {
					PlaySound("sd_dryfrire");
					return;
				}// no ammo, no shooty shoot

				// shoot the bullets, bulletcount for something like a shotgun with multiple bullets
				for(int i = 0; i < BulletCount; i++) {

					ShootBullet(BulletSpread, BulletForce, BulletDamage, BulletSize, i);
				}
			}

			if(!overrideShootEffects) {

				// clientside shoot effects
				if(IsServer) {
					ShootEffects(); // muzzle and brass eject
				}
				PlaySound(ShootSound); // shoot sound

				(Owner as AnimEntity).SetAnimBool("b_attack", true); // shoot anim

			}
		}

		public virtual void ShootBullet(float spread, float force, float damage, float bulletSize, int seed) {
			Rand.SetSeed(Time.Tick + seed);

			var player = Owner as ClassicPlayer;

			var forward = Owner.EyeRot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f * ((player.ActiveDrug && player.DrugType is DrugType.Ritindi) ? 0.25f : 1f);
			forward = forward.Normal;
			//forward = new Vector3( forward.x, forward.y, forward.z * VerticalBulletSpread );
			forward.z *= VerticalBulletSpread;

			AmmoPanel.Fire();
			Crosshair.Fire();
			int index = 0;
			foreach(var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * Range, bulletSize)) {
				tr.Surface.DoBulletImpact(tr);

				// blood plip where player was hit
				if(tr.Entity is ClassicPlayer hitply) {
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
					.UseLagCompensation()
					.Run();

			yield return bullet;

			var player = Owner as ClassicPlayer;

			// TODO: sniper
			// if(this is Sniper && ClassicGamemode.Instance.SniperCanPenetrate) {
			// 	var dir = (bullet.EndPos - bullet.StartPos).Normal;
			// 	if(bullet.Hit && wallBangedDistance < MaxWallbangDistance) {
			// 		var inNormal = bullet.Normal;
			// 		var inPoint = bullet.EndPos - inNormal * (size / 2);
			// 		//bullet
			// 		//DebugOverlay.Line(start, inPoint, Color.Green, 10, false);
			// 		//inpoint
			// 		//DebugOverlay.Sphere(inPoint, 0.5f, Color.Green, false, 10);
			// 		// normal
			// 		//DebugOverlay.Line(inPoint, inPoint + inNormal * 3, Color.Magenta, 10, false);

			// 		// adding dir to not be inside the inPoint
			// 		var wallbangTest = Trace.Ray(inPoint + dir, inPoint + dir * (MaxWallbangDistance - 1))
			// 						.HitLayer(CollisionLayer.WORLD_GEOMETRY)
			// 						.Ignore(Owner)
			// 						.Ignore(this)
			// 						.Size(1)
			// 						.UseLagCompensation()
			// 						.Run();

			// 		if(wallbangTest.Hit) {
			// 			var outNormal = wallbangTest.Normal;
			// 			var outPoint = wallbangTest.EndPos - outNormal * 0.5f;

			// 			//outpoint
			// 			//DebugOverlay.Sphere(outPoint, 0.1f, Color.Red, false, 10);

			// 			if(outNormal != Vector3.Zero && inNormal.Dot(outNormal) >= 0) {
			// 				//normal
			// 				//DebugOverlay.Line(outPoint, outPoint + outNormal * 3, Color.Magenta, 10, false);

			// 				//wallbang
			// 				//DebugOverlay.Line(inPoint, outPoint, Color.Cyan, 10, false);

			// 				var distance = (inPoint - outPoint).Length;
			// 				var totalDistance = wallBangedDistance + distance;

			// 				if(totalDistance < MaxWallbangDistance) {
			// 					foreach(var bullet2 in TraceBullet(outPoint + dir * 2, outPoint + dir * 1000, 1, totalDistance)) {
			// 						yield return bullet2;
			// 					}
			// 				}
			// 			}
			// 		}
			// 	}
			// }

			if(player.ActiveDrug && player.DrugType == DrugType.Ollie) {
				// pierce through the first player hit
				if(bullet.Entity is ClassicPlayer) {
					var dir = bullet.EndPos - bullet.StartPos;
					var penetrate = Trace.Ray(bullet.EndPos, bullet.EndPos + dir.Normal * 100f)
							.UseHitboxes()
							.Ignore(this)
							.Ignore(bullet.Entity)
							.Size(size)
							.UseLagCompensation()
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
								.UseLagCompensation()
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
			// TODO: debug
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

			SetParent(player, AttachementName, Transform.Zero);

			Owner = player;
			MoveType = MoveType.None;
			EnableAllCollisions = false;
			EnableDrawing = false;

			SetGlow(false);

			if(PickupTrigger.IsValid()) {
				PickupTrigger.EnableTouch = false;
			}

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

		public static Type GetRandomSpawnableType() {
			// this shit is dumb
			// ideally I'd use LibraryAttribute's Spawnable for this but it doesn't work with run-time types like this so fuck it, interface it is
			var types = Library.GetAll<ClassicBaseWeapon>().Where(x => x.GetInterfaces().Contains(typeof(ISpawnable)));
			return types.Random();
		}
	}
}
