using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

namespace SpeedDial {
	public partial class BasePickupTrigger : ModelEntity {
		public List<BasePlayer> TouchingPlayers = new();
		// in case we don't want to have to parent it
		public Entity ParentEntity;

		public override void Spawn() {
			base.Spawn();

			SetTriggerSize(32);
			Transmit = TransmitType.Never;
		}

		public void SetTriggerSize(float radius) {
			SetupPhysicsFromSphere(PhysicsMotionType.Keyframed, Vector3.Zero, radius);
			CollisionGroup = CollisionGroup.Trigger;
		}

		public override void StartTouch(Entity other) {
			if(other is BasePlayer player)
				TouchingPlayers.Add(player);
		}

		public override void EndTouch(Entity other) {
			if(other is BasePlayer player)
				TouchingPlayers.Remove(player);
		}
	}
}
