using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;

using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;
using SpeedDial.Classic.UI;
using SpeedDial.Classic.Drugs;

namespace SpeedDial.OneChamber.Player {
	public partial class OneChamberPlayer : ClassicPlayer {

		[Net]
		public int Lives { get; set; } = 3;

		[Net]
		public Entity StashedGun { get; set; }

		public override void Respawn() {
			Host.AssertServer();

			Model = Character.CharacterModel;

			LifeState = LifeState.Alive;
			Health = 100;
			DeathCause = CauseOfDeath.Generic;
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
			EnableLagCompensation = true;

			Game.Current.PawnRespawned(this);
			Game.Current.MoveToSpawnpoint(this);
			Game.Current.ActiveGamemode?.ActiveRound?.OnPawnRespawned(this);

			Frozen = false;
			GiveWeapon("oc_pistol");

			// just in case this was left open for some reason
			WinScreen.SetState(To.Single(Client), false);
		}

		public override void HandleDrugTaken(ClassicBaseDrug drug) {
			// no drugs in one chamber
			return;
		}

		public override void Simulate(Client cl) {
			base.Simulate(cl);
		}

		public override bool CanRespawn() {
			return Lives > 0;
		}

		public void AwardKill() {
			if(ActiveChild is Weapon weapon) {
				weapon.AmmoClip++;
			} else if (StashedGun is Weapon gun) {
				gun.AmmoClip++;
			}
		}

		public override void OnKilled() {
			Lives--;
			
			Frozen = true;

			EnableAllCollisions = false;
			EnableDrawing = false;

			// death effects, body + particles/decals
			BecomeRagdollOnClient(To.Everyone, new Vector3(Velocity.x / 2, Velocity.y / 2, 300), GetHitboxBone(0));
			BloodSplatter(To.Everyone);
			SoundFromScreen(To.Single(Client), "player_death");

			// give the killer his score etc during gameplay
			if(LastRecievedDamage.Attacker is ClassicPlayer attacker) {
				attacker.TimeSinceMurdered = 0;
				// TODO: find better kill confirm sound
				//SoundFromScreen(To.Single(attacker.Client), "kill_confirm");
			}

			if(LastRecievedDamage.Weapon is Weapon wep) {
				// HACK. this could be done better... too bad!
				if(wep.Blueprint.Special == WeaponSpecial.Melee) {
					DeathCause = CauseOfDeath.Melee;
				} else {
					DeathCause = CauseOfDeath.Bullet;
				}
			}

			LifeState = LifeState.Dead;
			Game.Current.PawnKilled(this, LastRecievedDamage);

			// lost last live
			if(!CanRespawn()) {
				Client.SwapPawn<ClassicSpectator>();
				Delete();
			}
		}

		public override void HandleAttack2() {
			// swap weapon to fists and vice versa
			if(ActiveChild is not null) {
				StashedGun = ActiveChild;
				ActiveChild = null;
			} else {
				if(StashedGun is null) {
					GiveWeapon("oc_pistol");
				} else {
					ActiveChild = StashedGun;
					StashedGun = null;
				}
				if((ActiveChild as Weapon).AmmoClip > 0) {
					PlaySound("sd_pickup.loaded");
				} else {
					PlaySound("sd_pickup.empty");
				}
			}
		}

		public override void ThrowWeapon() {
			// no gun throwing
			return;
		}
	}
}
