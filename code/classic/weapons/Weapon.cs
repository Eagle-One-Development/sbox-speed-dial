using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using Hammer;

using SpeedDial.Classic.Entities;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Drugs;
using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Weapons;

public struct ScreenshakeParameters {
	[Property] public float Length { get; set; } = 1;
	[Property] public float Speed { get; set; } = 1;
	[Property] public float Size { get; set; } = 1;
	[Property] public float Rotation { get; set; } = 1;

	public ScreenshakeParameters(float Length, float Speed, float Size, float Rotation) {
		this.Length = Length;
		this.Speed = Speed;
		this.Size = Size;
		this.Rotation = Rotation;
	}
}

public enum WeaponHoldType {
	Unarmed = 0,
	Melee = 1,
	Pistol = 2,
	Smg = 3,
	Rifle = 4,
	Shotgun = 5
}

public enum WeaponFireMode {
	Automatic,
	SemiAutomatic,
}

public enum WeaponSpecial {
	None,
	Burst,
	Melee
}

[Library("sdwep"), AutoGenerate]
public partial class WeaponTemplate : Asset {
	//important
	[Category("Info")] public string WeaponClass { get; set; } = "sd_wep";
	[Category("Info")] public string WeaponTitle { get; set; } = "Speed-Dial Weapon";
	[Category("Info")] public string WeaponDescription { get; set; } = "This is a Speed-Dial Weapon";

	//stats
	[Category("Stats")] public WeaponFireMode FireMode { get; set; } = WeaponFireMode.SemiAutomatic;
	[Category("Stats")] public float FireRate { get; set; } = 5.0f;
	[Category("Stats")] public float BulletDamage { get; set; } = 100.0f;
	[Category("Stats")] public float BulletSize { get; set; } = 1.0f;
	[Category("Stats")] public float BulletForce { get; set; } = 1.0f;
	[Category("Stats")] public float BulletRange { get; set; } = 4096.0f;
	[Category("Stats")] public int BulletCount { get; set; } = 1;
	[Category("Stats")] public float BulletSpread { get; set; } = 0.1f;
	[Category("Stats")] public float VerticalSpreadMultiplier { get; set; } = 1.0f;
	[Category("Stats")] public int AmmoPerShot { get; set; } = 1;
	[Category("Stats")] public int ClipSize { get; set; } = 10;

	//special
	[Category("Special")] public WeaponSpecial Special { get; set; } = WeaponSpecial.None;
	[Category("Special")] public int BurstLength { get; set; } = 3; // used for burst
	[Category("Special")] public float FireDelay { get; set; } = 0.3f; // used for burst or melee
	[Category("Special")] public bool Penetrate { get; set; } = false; // used for wall penetration (ie sniper)
	[Category("Special")] public bool Scoped { get; set; } = false; // used for extended view shift range
	[Category("Special")] public bool RandomSpawnable { get; set; } = false;
	[Category("Special"), BitFlags] public GamemodeEntity.Gamemodes ExcludedGamemodes { get; set; }

	//model
	[Category("Model"), ResourceType("vmdl")] public string WorldModelPath { get; set; } = "models/weapons/pistol/prop_pistol.vmdl";
	[Category("Model")] public WeaponHoldType HoldType { get; set; } = WeaponHoldType.Pistol;
	[Category("Model")] public string HoldAttach { get; set; } = "pistol_attach";

	//vfx
	[Category("VFX"), ResourceType("vpcf")] public string MuzzleParticle { get; set; } = "particles/pistol_muzzleflash.vpcf";
	[Category("VFX")] public string MuzzleAttach { get; set; } = "muzzle";
	[Category("VFX"), ResourceType("vpcf")] public string EjectParticle { get; set; } = "particles/pistol_ejectbrass.vpcf";
	[Category("VFX")] public string EjectAttach { get; set; } = "ejection_point";
	[Category("VFX")] public ScreenshakeParameters ScreenshakeParameters { get; set; }

	//ui
	[Category("UI")] public float UIEffectsScalar { get; set; } = 1; // used for ui panel bump strength
	[Category("UI"), ResourceType("png")] public string Icon { get; set; } = "materials/ui/weapons/pistol.png";

	//sounds
	[Category("Sounds"), ResourceType("sound")] public string ShootSound { get; set; } = "sounds/simpleguns/pistol/sd_pistol.shoot.sound";
	[Category("Sounds"), ResourceType("sound")] public string DryFireSound { get; set; } = "sounds/simpleguns/misc/sd_dryfrire.sound";
	[Category("Sounds"), ResourceType("sound")] public string EmptyPickupSound { get; set; } = "sounds/weapon_fx/sd_pickup.empty.sound";
	[Category("Sounds"), ResourceType("sound")] public string LoadedPickupSound { get; set; } = "sounds/weapon_fx/sd_pickup.loaded.sound";


	[Skip] public Model WorldModel { get; private set; }
	[Skip] public Texture IconTexture { get; private set; }


	[Skip] public static List<WeaponTemplate> All = new();

	public static WeaponTemplate GetTemplate(string weaponclass) {
		return All.FirstOrDefault(x => x.WeaponClass == weaponclass);
	}

	public static bool Exists(string weaponclass) {
		return All.Any(x => x.WeaponClass == weaponclass);
	}

	public static Weapon Create(string name) {
		var template = GetTemplate(name);
		return Create(template);
	}

	public static Weapon Create(WeaponTemplate template) {
		var wep = new Weapon();
		wep.ApplyTemplate(template);
		return wep;
	}

	public static WeaponTemplate GetRandomSpawnable() {
		return All.Where(x => x.RandomSpawnable).Random();
	}

	protected override void PostLoad() {
		// WeaponClass needs to be set
		if(string.IsNullOrWhiteSpace(WeaponClass)) {
			Log.Debug($"unable to load weapon \"{Path}\" due to empty weapon class");
			return;
		}

		// WeaponClass needs to be unique
		if(All.Any(x => x.WeaponClass == WeaponClass)) {
			Log.Debug($"unable to load weapon \"{Path}\" due to duplicate weapon class");
			return;
		}

		// get sound event name from full path
		ShootSound = System.IO.Path.GetFileNameWithoutExtension(ShootSound);
		DryFireSound = System.IO.Path.GetFileNameWithoutExtension(DryFireSound);
		EmptyPickupSound = System.IO.Path.GetFileNameWithoutExtension(EmptyPickupSound);
		LoadedPickupSound = System.IO.Path.GetFileNameWithoutExtension(LoadedPickupSound);

		// precache particles and sounds
		Precache.Add(MuzzleParticle);
		Precache.Add(EjectParticle);

		Precache.Add($"{ShootSound}.sound");
		Precache.Add($"{DryFireSound}.sound");
		Precache.Add($"{EmptyPickupSound}.sound");
		Precache.Add($"{LoadedPickupSound}.sound");

		WorldModel = Model.Load(WorldModelPath);
		IconTexture = Texture.Load(Icon);

		Log.Debug($"loaded weapon {WeaponClass}");

		All.Add(this);
	}
}

public partial class Weapon : BaseCarriable {
	[Net] public WeaponTemplate Template { get; private set; }
	public string WeaponClass => Template.WeaponClass;

	public void ApplyTemplate(WeaponTemplate template) {
		Template = template;
		Model = template.WorldModel;
		AmmoClip = Template.ClipSize;
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
		anim.SetParam("holdtype", (int)Template.HoldType);
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

		if(Template.Special == WeaponSpecial.Burst) {
			BurstSimulate();
		} else if(Template.Special == WeaponSpecial.Melee) {
			MeleeSimulate();
		}
	}

	public virtual bool CanPrimaryAttack() {
		if((Owner as ClassicPlayer).Frozen) return false;
		if(Owner is ClassicPlayer) {
			if(!Owner.IsValid() || (Template.FireMode == WeaponFireMode.Automatic && !Input.Down(InputButton.Attack1)) || (!(Template.FireMode == WeaponFireMode.Automatic) && !Input.Pressed(InputButton.Attack1))) return false;
		}

		var rate = Template.FireRate;
		if(rate <= 0) return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	public virtual void AttackPrimary() {
		if(Template.Special == WeaponSpecial.Burst) {
			BurstPrimary();
		} else if(Template.Special == WeaponSpecial.Melee) {
			MeleePrimary();
		} else {
			TimeSincePrimaryAttack = 0;

			if(!TakeAmmo(Template.AmmoPerShot)) {
				PlaySound("sd_dryfrire");
				return;
			}// no ammo, no shooty shoot

			// shoot the bullets, bulletcount for something like a shotgun with multiple bullets
			for(int i = 0; i < Template.BulletCount; i++) {
				ShootBullet(Template.BulletSpread, Template.BulletForce, Template.BulletDamage, Template.BulletSize, i);
			}

			ShootEffects();
		}
	}

	public virtual void ShootEffects() {
		if(IsLocalPawn) {
			_ = new Sandbox.ScreenShake.Perlin(Template.ScreenshakeParameters.Length, Template.ScreenshakeParameters.Speed, Template.ScreenshakeParameters.Size, Template.ScreenshakeParameters.Rotation);
			WeaponPanel.Fire(Template.UIEffectsScalar);
			Crosshair.Fire();
		}
		Particles.Create(Template.MuzzleParticle, EffectEntity, Template.MuzzleAttach);
		Particles.Create(Template.EjectParticle, EffectEntity, Template.EjectAttach);
		PlaySound(Template.ShootSound);
		(Owner as AnimEntity).SetAnimBool("b_attack", true); // shoot anim
	}

	public virtual void ShootBullet(float spread, float force, float damage, float bulletSize, int seed) {
		Rand.SetSeed(Time.Tick + seed);

		var player = Owner as ClassicPlayer;

		var forward = Owner.EyeRotation.Forward;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f * ((player.ActiveDrug && player.DrugType is DrugType.Ritindi) ? 0.25f : 1f);
		forward = forward.Normal;
		forward.z *= Template.VerticalSpreadMultiplier;

		int index = 0;
		foreach(var tr in TraceBullet(Owner.EyePosition, Owner.EyePosition + forward * Template.BulletRange, bulletSize)) {
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

		if(Template.Penetrate) {
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
					var ricochet = Trace.Ray(bullet.EndPos, end + dir * Template.BulletRange)
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
		SetParent(player, Template.HoldAttach, Transform.Zero);

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
