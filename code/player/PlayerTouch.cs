using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.Meds;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {
		public override void StartTouch(Entity other) {
			if(timeSinceDropped < 1) return;

			if(IsClient) return;

			if(other is PickupTrigger pt) {
				if(other.Parent is BaseSpeedDialWeapon wep1) {
					StartTouch(other.Parent);

					float magnitude = wep1.PhysicsBody.Velocity.Length;
					//Log.Info($"Velocity: {magnitude}");
					if(magnitude > 450f) {
						wep1.PhysicsBody.EnableAutoSleeping = false;
						Sound.FromEntity("weaponhit", this);
						KillMyself(wep1.previousOwner);
						wep1.Velocity *= -0.5f;
					}
				}

				if(!MedTaken && other.Parent is BaseMedication drug) {
					StartTouch(other.Parent);
					drug.PickUp();
					MedTaken = true;
					if(drug.drug != DrugType.Leaf) {

					} else {
						//Since Leaf lets you take an extra hit we don't need to do any kind of effect over time so we can just set the health to 200
						Health = 200f;

					}
					CurrentDrug = drug.drug;
					TimeSinceMedTaken = 0;
					MedTaken = true;
					MedDuration = drug.drugDuration;
					DBump( drug.drugName, this, drug.drugFlavor);

				}
				return;
			}
		}

		[ClientRpc]
		public void DBump(string s , SpeedDialPlayer p, string f )
		{
			if ( p == Local.Pawn )
			{
				DrugBump( s , f);
			}
			
		}

		public override void Touch(Entity other) {

			if(timeSinceDropped < 1f) return;

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
