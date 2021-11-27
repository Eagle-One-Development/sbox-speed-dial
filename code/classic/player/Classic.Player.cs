using System;
using System.Linq;

using Sandbox;

using SpeedDial.Classic.Drugs;
using SpeedDial.Classic.Weapons;
using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Player {
	public partial class ClassicPlayer : BasePlayer {
		[Net] public bool Frozen { get; set; }
		[Net] public Character Character { get; set; }
		[Net] public bool ActiveDrug { get; set; }
		[Net] public DrugType DrugType { get; set; }
		[Net] public TimeSince TimeSinceDrugTaken { get; set; }
		public Particles DrugParticles;
		public virtual float DrugDuration => 8f;
		public override float RespawnTime => 1.5f;
		[Net] TimeSince TimeSinceMurdered { get; set; }

		public override void InitialRespawn() {
			Character = Character.All.Random();
			Respawn();
		}

		public override void Respawn() {
			Host.AssertServer();

			SetModel(Character.Model);

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;

			CreateHull();
			ResetInterpolation();

			Controller = new ClassicController();
			Animator = new ClassicAnimator();
			Camera = new ClassicCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			LagCompensation = true;

			Frozen = false;
			GiveWeapon<ClassicBaseWeapon>(Character.WeaponClass);

			Game.Current.PawnRespawned(this);
			Game.Current.MoveToSpawnpoint(this);
		}

		public override void Simulate(Client cl) {
			base.Simulate(cl);

			if(ActiveChild is not null) {
				TimeSinceWeaponCarried = 0;
			}

			if(Input.Pressed(InputButton.Attack2) && ActiveChild != null && !Frozen) {
				ThrowWeapon();
			}

			if(ActiveChild == null && Input.Pressed(InputButton.Attack1) && TimeSinceMeleeStarted >= 0.6f) {
				StartMelee();
			}

			if(ActiveMelee) {
				SimulateMelee();
			}

			// handle weapon pickup after throwing so you can swap
			// TODO: hold input for pickup when activechild is null?
			// last bit is to prevent the player from immediately grabbing it out of the air again after throwing if spamming rightclick
			if(ActiveChild is null && Pickup && PickupWeapon is not null && PickupWeapon.IsValid() && Input.Pressed(InputButton.Attack2) && (TimeSincePickup > (PickupWeapon.PreviousOwner == this ? 0.3f : 0))) {
				var weapon = PickupWeapon;
				Pickup = false;
				PickupWeapon = null;

				TimeSincePickup = 0;

				ActiveChild = weapon;
				weapon.OnCarryStart(this);
			}

			if(ActiveDrug) {
				SimulateDrug();
			}

			// this should probably be in a better place... too bad!
			SetAnimBool("b_polvo", ActiveDrug && DrugType is DrugType.Polvo);

			// DEBUG: spawn a random gun
			if(Debug.Enabled && Input.Pressed(InputButton.Zoom)) {
				using(Prediction.Off()) {
					if(IsServer) {
						var ent = Library.Create<ClassicBaseWeapon>(ClassicBaseWeapon.GetRandomSpawnableType());
						ent.Position = EyePos;
					}
				}
			}

			// reset combo
			if(TimeSinceMurdered > 5f) {
				Client.SetValue("combo", 0);
			}
		}

		public void RefreshCharacter() {
			SetModel(Character.Model);
			GiveWeapon<ClassicBaseWeapon>(Character.WeaponClass);
		}

		public void SimulateDrug() {
			if(TimeSinceDrugTaken >= DrugDuration) {
				ActiveDrug = false;
				DrugParticles?.Destroy();
				return;
			}
		}

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
					if(tr.Entity is ClassicPlayer player) {
						//TODO: COD
						//player.CauseOfDeath = COD.Melee;
					}
					tr.Entity.TakeDamage(damage);
				}
			}
		}

		public override void TakeDamage(DamageInfo info) {
			if(ActiveDrug && DrugType == DrugType.Leaf) {
				// leaf makes us get less damage
				info.Damage /= 5f;
			}
			base.TakeDamage(info);
		}

		public override void OnKilled() {
			base.OnKilled();
			Frozen = true;

			// reset drug
			ActiveDrug = false;
			DrugParticles?.Destroy(true);

			EnableAllCollisions = false;
			EnableDrawing = false;

			// chuck weapon away in a random direction
			DropWeapon(out var weapon);
			if(weapon.IsValid()) {
				weapon.Velocity += Vector3.Random.WithZ(0).Normal * 150 + Vector3.Up * 150;
				weapon.PhysicsBody.AngularVelocity = new Vector3(0, 0, 60f);
				weapon.CanImpactKill = false;
			}

			// death effects, body + particles/decals, screen hint
			BecomeRagdollOnClient(To.Everyone, new Vector3(Velocity.x / 2, Velocity.y / 2, 300), GetHitboxBone(0));
			BloodSplatter(To.Everyone);
			ScreenHints.FireEvent(To.Single(Client), "WHACKED", "+WIP");
			SoundFromScreen(To.Single(Client), "player_death");

			// give the killer his score etc
			if(LastRecievedDamage.Attacker is ClassicPlayer attacker) {
				attacker.TimeSinceMurdered = 0;
				attacker.AwardKill();
				SoundFromScreen(To.Single(attacker.Client), "kill_confirm");
			}

			// reset combo
			Client.SetValue("combo", 0);
		}

		public override void StartTouch(Entity other) {
			// this is a pickuptrigger, we could pick it up
			if(other is BasePickupTrigger trigger) {
				if(trigger.ParentEntity is ClassicBaseWeapon weapon) {
					PickupWeapon = weapon;
					Pickup = true;
				}
				// this is a gun, it could kill us
			} else if(other is ClassicBaseWeapon wep) {
				if(wep.PhysicsBody.IsValid()) {
					if(wep.CanImpactKill && this != wep.PreviousOwner && wep.Velocity.Length > 450f) {
						Sound.FromEntity("smack", this);
						// TODO: COD
						//CauseOfDeath = COD.Thrown;
						ImpactKill(wep.PreviousOwner);

						// bounce off the body
						wep.Velocity *= -0.3f;
						wep.CanImpactKill = false;
					}
				}
			}
		}

		public void ImpactKill(Entity attacker) {
			DamageInfo info = new();
			info.Damage = 200;
			info.Attacker = attacker;
			TakeDamage(info);
			PlaySound("smack");
		}

		public override void Touch(Entity other) {
			if(other is BasePickupTrigger trigger) {
				if(trigger.ParentEntity is ClassicBaseWeapon weapon) {
					if(PickupWeapon is null || !Pickup) {
						PickupWeapon = weapon;
						Pickup = true;
					}
					// handle drugs
					// do this in touch since drugs can run out while we're standing on one
				} else if(trigger.ParentEntity is ClassicBaseDrug drug && !ActiveDrug) {
					drug.Taken(this);
				}
			}
		}

		public override void EndTouch(Entity other) {
			if(other is BasePickupTrigger trigger) {
				if(trigger.ParentEntity is ClassicBaseWeapon weapon) {
					if(weapon == PickupWeapon) {
						PickupWeapon = null;
						Pickup = false;
					}
				}
			}
		}

		[ServerCmd("set_char")]
		public static void SetCharacter(int index) {
			if(ConsoleSystem.Caller.Pawn is ClassicPlayer player) {
				if(index > Character.All.Count) return;
				var character = Character.All.ElementAtOrDefault(index);
				if(character is null) {
					Log.Warning("character index invalid");
					return;
				}
				player.Character = character;
				// if(Instance.Round is PreRound) 
				player.RefreshCharacter();
			}
		}
	}
}
