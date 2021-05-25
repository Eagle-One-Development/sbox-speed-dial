using Sandbox;
using SpeedDial.Weapons;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		private TimeSince timeSinceDied = 0;

		[Net, Local, Predicted]
		public float RespawnTime { get; set; } = 1;

		public SpeedDialPlayer() {
			Inventory = new SpeedDialInventory(this);
		}

		public void InitialSpawn() {
			Respawn();
			//more initial spawn stuff maybe

		}

		public override void Respawn() {
			SetModel("models/citizen/citizen.vmdl");

			Camera = new SpeedDialCamera();
			Controller = new SpeedDialController();
			Animator = new PlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Host.AssertServer();

			Inventory.Add(new Pistol(), true);
			Log.Info("BIPPO");

			GiveAmmo(AmmoType.Pistol, 100);

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			CreateHull();
			ResetInterpolation();
			SpeedDialGame.MoveToSpawn(this);
		}

		public override void OnKilled() {
			Game.Current?.OnKilled(this);

			BecomeRagdollOnClient(new Vector3(Velocity.x / 2, Velocity.y / 2, 300), GetHitboxBone(0)); //force and bone, fix later with damage stuff in place

			var tr = Trace.Ray(Position + Vector3.Up * 48, Position + Vector3.Down * 500)
					.UseHitboxes()
					.Ignore(this)
					.Size(1)
					.Run();

			DebugOverlay.Line(Position + Vector3.Up * 48, Position + Vector3.Down * 500, Color.Green, 10);

			if(tr.Hit)
				DebugOverlay.Sphere(tr.EndPos, 3, Color.Green, false, 10);

			var decalPath = "materials/decals/blood1.vmat";
			if(decalPath != null) {
				if(DecalDefinition.ByPath.TryGetValue(decalPath, out var decal)) {
					Log.Info("DECAL");
					decal.PlaceUsingTrace(tr);
				}
			}

			Inventory.DeleteContents();

			timeSinceDied = 0;
			LifeState = LifeState.Dead;

			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;
		}

		public override void Simulate(Client cl) {
			if(LifeState == LifeState.Dead) {
				if(timeSinceDied > RespawnTime && IsServer) {
					Respawn();
				}
				return;
			}

			if(Input.ActiveChild != null) {
				ActiveChild = Input.ActiveChild;
			}

			SimulateActiveChild(cl, ActiveChild);

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());
		}
	}
}
