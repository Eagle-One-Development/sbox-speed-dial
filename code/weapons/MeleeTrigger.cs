using Sandbox;

namespace SpeedDial.Weapons {
	public partial class MeleeTrigger : ModelEntity {

		public void SetupTrigger(Vector3 a, Vector3 b, float radius = 2) {
			SetupPhysicsFromCapsule(PhysicsMotionType.Keyframed, new Capsule(a, b, radius));
			CollisionGroup = CollisionGroup.Weapon;
		}
	}
}
