using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.Meds;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {
		public override void StartTouch(Entity other) {
			if(IsClient) return;

			if(other is PickupTrigger pt) {
				if(other.Parent is BaseSpeedDialWeapon wep) {
					StartTouch(other.Parent); // what

					if(wep.PhysicsBody.IsValid()) {
						float magnitude = wep.PhysicsBody.Velocity.Length;
						//Log.Info($"Velocity: {magnitude}");
						if(magnitude > 450f && this != wep.PreviousOwner && wep.CanKill) {
							wep.PhysicsBody.EnableAutoSleeping = false;
							Sound.FromEntity("smack", this);
							CauseOfDeath = COD.Thrown;
							KillMyself(wep.PreviousOwner);
							wep.Velocity *= -0.5f;
							wep.CanKill = false;
						}
					}
				}

				if(!MedTaken && other.Parent is BaseMedication drug) {
					StartTouch(other.Parent);
					drug.PickUp(this);
					MedTaken = true;
					if(drug.Drug != DrugType.Leaf) {

					} else {
						//Since Leaf lets you take an extra hit we don't need to do any kind of effect over time so we can just set the health to 500
						Health = 500f;
					}

					DrugParticles = Particles.Create(drug.ParticleName);
					DrugParticles.SetForward(0, Vector3.Up);
					DrugParticles.SetEntityBone(0, this, GetBoneIndex("head"), Transform.Zero, true);

					CurrentDrug = drug.Drug;
					ResetTimeSinceMedTaken = true;
					MedTaken = true;
					MedDuration = drug.DrugDuration;
					DBump(drug.DrugName, this, drug.DrugFlavor);

				}
				return;
			}
		}

		[ClientRpc]
		public void DBump(string s, SpeedDialPlayer p, string f) {
			if(p == Local.Pawn) {
				DrugBump(s, f, true);
			}
		}

		public override void Touch(Entity other) {
			if(IsClient) return;

			if(other is PickupTrigger) {
				if(other.Parent is BaseSpeedDialWeapon) {
					Touch(other.Parent);
					pickup = true;
				}
				return;
			}
			pickUpEntity = other;
		}

		public override void EndTouch(Entity other) {
			base.EndTouch(other);
			if(other is PickupTrigger) {
				if(other.Parent is BaseSpeedDialWeapon) {
					Touch(other.Parent);
					pickUpEntity = null;
					pickup = false;
				}
				return;
			}
		}
	}
}
