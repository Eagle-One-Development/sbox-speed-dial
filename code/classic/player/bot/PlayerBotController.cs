using Sandbox;

namespace SpeedDial.Classic.Player {
	public partial class SpeedDialBotController : SpeedDialController {
		public float Forward { get; set; }
		public float Left { get; set; }

		public override void FrameSimulate() {
			if((Pawn as SpeedDialPlayer).Freeze) return;
			base.FrameSimulate();
		}

		public override void Simulate() {
			if((Pawn as SpeedDialPlayer).Freeze) { WishVelocity = Vector3.Zero; return; }
			EyePosLocal = Vector3.Up * (EyeHeight * Pawn.Scale);
			UpdateBBox();

			EyePosLocal += TraceOffset;
			EyeRot = (Pawn as SpeedDialBotPlayer).LookRot;

			RestoreGroundPos();

			if(Unstuck.TestAndFix())
				return;


			//
			// Start Gravity
			//
			Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;
			Velocity += new Vector3(0, 0, BaseVelocity.z) * Time.Delta;
			BaseVelocity = BaseVelocity.WithZ(0);

			// Fricion is handled before we add in any base velocity. That way, if we are on a conveyor, 
			//  we don't slow when standing still, relative to the conveyor.
			bool bStartOnGround = GroundEntity != null;
			//bool bDropSound = false;
			if(bStartOnGround) {

				Velocity = Velocity.WithZ(0);

				if(GroundEntity != null) {
					ApplyFriction(GroundFriction * SurfaceFriction);
				}
			}

			float f = 1f;
			if((Pawn as SpeedDialPlayer).MedTaken && (Pawn as SpeedDialPlayer).CurrentDrug == Meds.DrugType.Polvo) {
				f = 2f;
			}
			//
			// Work out wish velocity.. just take input, rotate it to view, clamp to -1, 1
			//

			WishVelocity = new Vector3(Forward, Left, 0);
			var inSpeed = WishVelocity.Length.Clamp(0, 1);

			WishVelocity = WishVelocity.WithZ(0);

			WishVelocity = WishVelocity.Normal * inSpeed;
			WishVelocity *= DefaultSpeed * f;


			bool bStayOnGround = false;
			if(GroundEntity != null) {
				bStayOnGround = true;
				WalkMove();
			} else {
				AirMove();
			}

			CategorizePosition(bStayOnGround);

			// FinishGravity
			Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;



			if(GroundEntity != null) {
				Velocity = Velocity.WithZ(0);
			}

			SaveGroundPos();

			if(Debug) {
				DebugOverlay.Box(Position + TraceOffset, mins, maxs, Color.Red);
				DebugOverlay.Box(Position, mins, maxs, Color.Blue);

				var lineOffset = 0;
				if(Host.IsServer) lineOffset = 10;

				DebugOverlay.ScreenText(lineOffset + 0, $"        Position: {Position}");
				DebugOverlay.ScreenText(lineOffset + 1, $"        Velocity: {Velocity}");
				DebugOverlay.ScreenText(lineOffset + 2, $"    BaseVelocity: {BaseVelocity}");
				DebugOverlay.ScreenText(lineOffset + 3, $"    GroundEntity: {GroundEntity} [{GroundEntity?.Velocity}]");
				DebugOverlay.ScreenText(lineOffset + 4, $" SurfaceFriction: {SurfaceFriction}");
				DebugOverlay.ScreenText(lineOffset + 5, $"    WishVelocity: {WishVelocity}");
			}

		}
	}
}
