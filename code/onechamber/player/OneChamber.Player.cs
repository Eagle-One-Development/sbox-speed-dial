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
			GiveWeapon("sd_pistol");

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
				// swap out pawn for spectator pawn
				var newpawn = new ClassicSpectator();
				newpawn.Transform = Transform;
				Client.Pawn = newpawn;
				Client.GetPawn<ClassicSpectator>().InitialRespawn();
				Delete();
			}
		}

		public override void ThrowWeapon() {
			// we don't allow for weapons to be thrown
			return;
		}
	}
}
