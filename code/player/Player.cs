using Sandbox;
using SpeedDial.Weapons;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		[Net, Local]
		private TimeSince timeSinceDied { get; set; } = 0;

		[Net]
		public float RespawnTime { get; set; } = 1f;

		[Net]
		public Color32 playerColor { get; set; }

		public SpeedDialPlayer() {
			Inventory = new SpeedDialInventory(this);
		}

		public void InitialSpawn() {

			if(GetClientOwner().SteamId == 76561198000823482) {
				playerColor = new Color32(250, 176, 3); // bak
			} else if(GetClientOwner().SteamId == 76561198203314521) { // gurke 76561198203314521
				playerColor = new Color32(70, 0, 70);
			} else if(GetClientOwner().SteamId == 76561198095231052) { // generic
				playerColor = new Color32(27, 49, 63);
			} else {
				playerColor = Color.Random;
			}
			Respawn();
		}


		public override void Respawn() {
			SetModel("models/biped_standard/biped_standard.vmdl");

			RenderColor = playerColor;

			Camera = new SpeedDialCamera();
			Controller = new SpeedDialController();
			Animator = new PlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Host.AssertServer();

			KillCombo = 0;

			Inventory.Add(new Pistol(), true);

			GiveAmmo(AmmoType.Pistol, 1000);

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

			// fuck the current decal stuff, this doesn't work
			//Log.Info("DECAL");
			var rot = Rotation.LookAt(tr.Normal) * Rotation.FromAxis(Vector3.Forward, 5);
			var pos = tr.EndPos;
			if(Host.IsClient) {
				Decals.Place(Material.Load("materials/decals/blood1.vmat"), tr.Entity, tr.Bone, pos, 5, rot);

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

			DebugOverlay.ScreenText(new Vector2(300, 300), 1, Color.Green, $"{KillCombo}x {KillScore} Score {TimeSinceMurdered}\ts");
		}
	}
}
