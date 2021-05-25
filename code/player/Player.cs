using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		private TimeSince timeSinceDied = 0;

		public void InitialSpawn() {
			Respawn();
			//more initial spawn stuff maybe
		}

		public override void Respawn() {
			SetModel("models/biped_standard/biped_standard.vmdl");

			Camera = new SpeedDialCamera();
			Controller = new SpeedDialController();
			Animator = new PlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Host.AssertServer();

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			CreateHull();
			ResetInterpolation();
			SpeedDialGame.MoveToSpawn(this);
		}

		public override void OnKilled() {
			Game.Current?.OnKilled(this);

			BecomeRagdollOnClient(new Vector3(0, 0, 300), GetHitboxBone(0)); //force and bone, fix later with damage stuff in place

			timeSinceDied = 0;
			LifeState = LifeState.Dead;

			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;
		}

		public override void Simulate(Client cl) {
			if(LifeState == LifeState.Dead) {
				if(timeSinceDied > 1 && IsServer) {
					Respawn();
				}
				return;
			}

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());
		}
	}
}
