using System;
using Sandbox;
using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.Meds {
	public partial class BaseMedication : ModelEntity, IRespawnableEntity {

		public virtual string WorldModel => "models/drugs/leaf/leaf.vmdl";
		public virtual float RotationSpeed => 75f;
		public virtual string DrugName => "POLVO";
		public virtual string DrugFlavor => "vanilla";
		public virtual float DrugDuration => 4f;
		public virtual DrugType Drug => DrugType.Polvo;
		public virtual string icon => "materials/ui/smile.png";
		public virtual string PickupSound => "sd_leaf_take";
		public virtual Color OutlineColor => new Color(1, 1, 1, 1);

		[Property]
		public virtual float RespawnTime { get; set; }

		private Vector3 initialPosition = Vector3.Zero;

		[Net]
		public TimeSince TimeSinceSpawned { get; set; }


		public PickupTrigger PickupTrigger { get; protected set; }
		private float rotationAngle;
		public virtual string ParticleName => "particles/drug_fx/sd_polvo/sd_polvo.vpcf";


		public override void Spawn() {
			base.Spawn();

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs(CollisionLayer.Debris); // so player movement doesn't walk into it

			SetModel(WorldModel);

			GlowState = GlowStates.GlowStateOn;
			GlowDistanceStart = 0;
			GlowDistanceEnd = 1000;
			GlowColor = OutlineColor;
			GlowActive = true;

			ResetInterpolation();

			PickupTrigger = new();
			PickupTrigger.Parent = this;
			PickupTrigger.ResetInterpolation();
			PickupTrigger.Position = Position;
			PickupTrigger.EnableAllCollisions = false;

			TimeSinceSpawned = 0;

			ItemRespawn.AddRecordFromEntity(this);
		}

		[Event("server.tick")]
		public void Simulate() {
			if(TimeSinceSpawned > 0.5f) {
				PickupTrigger.EnableAllCollisions = true;
			}
			if(initialPosition == Vector3.Zero) {
				initialPosition = Position;
			}

			rotationAngle += RotationSpeed * Time.Delta;
			Rotation = Rotation.FromAxis(Vector3.Up, rotationAngle);

			Position = initialPosition + Vector3.Up * MathF.Sin(Time.Now * 2f) * 7f;
		}

		public virtual void PickUp(SpeedDialPlayer player) {
			player.PlayUISound(To.Single(player), PickupSound);
			ItemRespawn.Taken(this);
			Delete();
		}
	}

	public enum DrugType {
		Polvo,
		Leaf,
		Ollie,
		Ritindi
	}
}
