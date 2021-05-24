using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {
		public override void Respawn() {
			SetModel("models/citizen/citizen.vmdl");

			Camera = new SpeedDialCamera();
			Controller = new SpeedDialController();
			Animator = new StandardPlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}

		public override void Simulate(Client cl) {
			base.Simulate(cl);
			// simulate
		}
	}
}
