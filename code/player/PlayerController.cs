using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialController : BasePlayerController {
		public float DefaultSpeed { get; set; } = 300.0f;
		public float Acceleration { get; set; } = 10.0f;
		public float AirAcceleration { get; set; } = 50.0f;
		public float FallSoundZ { get; set; } = -30.0f;
		public float GroundFriction { get; set; } = 4.0f;
		public float StopSpeed { get; set; } = 100.0f;
		public float Size { get; set; } = 20.0f;
		public float DistEpsilon { get; set; } = 0.03125f;
		public float GroundAngle { get; set; } = 46.0f;
		public float Bounce { get; set; } = 0.0f;
		public float MoveFriction { get; set; } = 1.0f;
		public float StepSize { get; set; } = 18.0f;
		public float MaxNonJumpVelocity { get; set; } = 140.0f;
		public float BodyGirth { get; set; } = 32.0f;
		public float BodyHeight { get; set; } = 72.0f;
		public float EyeHeight { get; set; } = 64.0f;
		public float Gravity { get; set; } = 800.0f;
		public float AirControl { get; set; } = 30.0f;
		public bool Swimming { get; set; } = false;
		public bool AutoJump { get; set; } = false;

		[Net, Local]
		public bool Freeze { get; set; } = false;

		public Unstuck Unstuck;


		public SpeedDialController() {
			Unstuck = new Unstuck(this);
		}

		/// <summary>
		/// This is temporary, get the hull size for the player's collision
		/// </summary>
		public override BBox GetHull() {
			var girth = BodyGirth * 0.5f;
			var mins = new Vector3(-girth, -girth, 0);
			var maxs = new Vector3(+girth, +girth, BodyHeight);

			return new BBox(mins, maxs);
		}

		protected Vector3 mins;
		protected Vector3 maxs;

		public virtual void SetBBox(Vector3 mins, Vector3 maxs) {
			if(this.mins == mins && this.maxs == maxs)
				return;

			this.mins = mins;
			this.maxs = maxs;
		}

		/// <summary>
		/// Update the size of the bbox. We should really trigger some shit if this changes.
		/// </summary>
		public virtual void UpdateBBox() {
			var girth = BodyGirth * 0.5f;

			var mins = new Vector3(-girth, -girth, 0) * Pawn.Scale;
			var maxs = new Vector3(+girth, +girth, BodyHeight) * Pawn.Scale;

			SetBBox(mins, maxs);
		}

		protected float SurfaceFriction;


		public override void FrameSimulate() {
			if(Freeze) return;
			base.FrameSimulate();
			EyeRot = Input.Rotation;
		}

		public override void Simulate() {
			if(Freeze) return;
			EyePosLocal = Vector3.Up * (EyeHeight * Pawn.Scale);
			UpdateBBox();

			EyePosLocal += TraceOffset;
			EyeRot = Input.Rotation;

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

			//
			// Work out wish velocity.. just take input, rotate it to view, clamp to -1, 1
			//
			WishVelocity = new Vector3(Input.Forward, Input.Left, 0);
			var inSpeed = WishVelocity.Length.Clamp(0, 1);

			WishVelocity = WishVelocity.WithZ(0);

			WishVelocity = WishVelocity.Normal * inSpeed;
			WishVelocity *= DefaultSpeed;


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

		void WalkMove() {
			var wishdir = WishVelocity.Normal;
			var wishspeed = WishVelocity.Length;

			WishVelocity = WishVelocity.WithZ(0);
			WishVelocity = WishVelocity.Normal * wishspeed;

			Velocity = Velocity.WithZ(0);
			Accelerate(wishdir, wishspeed, 0, Acceleration);
			Velocity = Velocity.WithZ(0);


			Velocity += BaseVelocity;

			try {
				if(Velocity.Length < 1.0f) {
					Velocity = Vector3.Zero;
					return;
				}

				// first try just moving to the destination	
				var dest = (Position + Velocity * Time.Delta).WithZ(Position.z);

				var pm = TraceBBox(Position, dest);

				if(pm.Fraction == 1) {
					Position = pm.EndPos;
					StayOnGround();
					return;
				}

				StepMove();
			} finally {

				// Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
				Velocity -= BaseVelocity;
			}

			StayOnGround();
		}

		private void StepMove() {
			var startPos = Position;
			var startVel = Velocity;

			//
			// First try walking straight to where they want to go.
			//
			TryPlayerMove();

			//
			// mv now contains where they ended up if they tried to walk straight there.
			// Save those results for use later.
			//	
			var withoutStepPos = Position;
			var withoutStepVel = Velocity;

			//
			// Try again, this time step up and move across
			//
			Position = startPos;
			Velocity = startVel;
			var trace = TraceBBox(Position, Position + Vector3.Up * (StepSize + DistEpsilon));
			if(!trace.StartedSolid) Position = trace.EndPos;
			TryPlayerMove();

			//
			// If we move down from here, did we land on ground?
			//
			trace = TraceBBox(Position, Position + Vector3.Down * (StepSize + DistEpsilon * 2));
			if(!trace.Hit || Vector3.GetAngle(Vector3.Up, trace.Normal) > GroundAngle) {
				// didn't step on ground, so just use the original attempt without stepping
				Position = withoutStepPos;
				Velocity = withoutStepVel;
				return;
			}


			if(!trace.StartedSolid)
				Position = trace.EndPos;

			var withStepPos = Position;

			float withoutStep = (withoutStepPos - startPos).WithZ(0).Length;
			float withStep = (withStepPos - startPos).WithZ(0).Length;

			//
			// We went further without the step, so lets use that
			//
			if(withoutStep > withStep) {
				Position = withoutStepPos;
				Velocity = withoutStepVel;
				return;
			}
		}

		/// <summary>
		/// Add our wish direction and speed onto our velocity
		/// </summary>
		public virtual void Accelerate(Vector3 wishdir, float wishspeed, float speedLimit, float acceleration) {
			// This gets overridden because some games (CSPort) want to allow dead (observer) players
			// to be able to move around.
			// if ( !CanAccelerate() )
			//     return;

			if(speedLimit > 0 && wishspeed > speedLimit)
				wishspeed = speedLimit;

			// See if we are changing direction a bit
			var currentspeed = Velocity.Dot(wishdir);

			// Reduce wishspeed by the amount of veer.
			var addspeed = wishspeed - currentspeed;

			// If not going to add any speed, done.
			if(addspeed <= 0)
				return;

			// Determine amount of acceleration.
			var accelspeed = acceleration * Time.Delta * wishspeed * SurfaceFriction;

			// Cap at addspeed
			if(accelspeed > addspeed)
				accelspeed = addspeed;

			Velocity += wishdir * accelspeed;
		}

		/// <summary>
		/// Remove ground friction from velocity
		/// </summary>
		public virtual void ApplyFriction(float frictionAmount = 1.0f) {

			// Calculate speed
			var speed = Velocity.Length;
			if(speed < 0.1f) return;

			// Bleed off some speed, but if we have less than the bleed
			//  threshold, bleed the threshold amount.
			float control = (speed < StopSpeed) ? StopSpeed : speed;

			// Add the amount to the drop amount.
			var drop = control * Time.Delta * frictionAmount;

			// scale the velocity
			float newspeed = speed - drop;
			if(newspeed < 0) newspeed = 0;

			if(newspeed != speed) {
				newspeed /= speed;
				Velocity *= newspeed;
			}

		}

		public virtual void AirMove() {
			var wishdir = WishVelocity.Normal;
			var wishspeed = WishVelocity.Length;

			Accelerate(wishdir, wishspeed, AirControl, AirAcceleration);

			Velocity += BaseVelocity;

			TryPlayerMove();

			Velocity -= BaseVelocity;
		}

		public virtual void TryPlayerMove() {
			MoveHelper mover = new MoveHelper(Position, Velocity);
			mover.Trace = mover.Trace.Size(mins, maxs).Ignore(Pawn);
			mover.MaxStandableAngle = GroundAngle;

			mover.TryMove(Time.Delta);

			Position = mover.Position;
			Velocity = mover.Velocity;
		}

		void CategorizePosition(bool bStayOnGround) {
			SurfaceFriction = 1.0f;

			// Doing this before we move may introduce a potential latency in water detection, but
			// doing it after can get us stuck on the bottom in water if the amount we move up
			// is less than the 1 pixel 'threshold' we're about to snap to.	Also, we'll call
			// this several times per frame, so we really need to avoid sticking to the bottom of
			// water on each call, and the converse case will correct itself if called twice.
			//CheckWater();

			var point = Position - Vector3.Up * 2;
			var vBumpOrigin = Position;

			//
			//  Shooting up really fast.  Definitely not on ground trimed until ladder shit
			//
			bool bMovingUpRapidly = Velocity.z > MaxNonJumpVelocity;
			bool bMovingUp = Velocity.z > 0;

			bool bMoveToEndPos = false;

			if(GroundEntity != null) // and not underwater
			{
				bMoveToEndPos = true;
				point.z -= StepSize;
			} else if(bStayOnGround) {
				bMoveToEndPos = true;
				point.z -= StepSize;
			}

			if(bMovingUpRapidly || Swimming) // or ladder and moving up
			{
				ClearGroundEntity();
				return;
			}

			var pm = TraceBBox(vBumpOrigin, point, 4.0f);

			if(pm.Entity == null || Vector3.GetAngle(Vector3.Up, pm.Normal) > GroundAngle) {
				ClearGroundEntity();
				bMoveToEndPos = false;

				if(Velocity.z > 0)
					SurfaceFriction = 0.25f;
			} else {
				UpdateGroundEntity(pm);
			}

			if(bMoveToEndPos && !pm.StartedSolid && pm.Fraction > 0.0f && pm.Fraction < 1.0f) {
				Position = pm.EndPos;
			}

		}

		/// <summary>
		/// We have a new ground entity
		/// </summary>
		public virtual void UpdateGroundEntity(TraceResult tr) {
			GroundNormal = tr.Normal;

			// VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
			// A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
			// This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
			SurfaceFriction = tr.Surface.Friction * 1.25f;
			if(SurfaceFriction > 1) SurfaceFriction = 1;

			GroundEntity = tr.Entity;

			if(GroundEntity != null) {
				BaseVelocity = GroundEntity.Velocity;
			}

		}

		/// <summary>
		/// We're no longer on the ground, remove it
		/// </summary>
		public virtual void ClearGroundEntity() {
			if(GroundEntity == null) return;

			GroundEntity = null;
			GroundNormal = Vector3.Up;
			SurfaceFriction = 1.0f;
		}

		/// <summary>
		/// Traces the current bbox and returns the result.
		/// liftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
		/// position. This is good when tracing down because you won't be tracing through the ceiling above.
		/// </summary>
		public override TraceResult TraceBBox(Vector3 start, Vector3 end, float liftFeet = 0.0f) {
			return TraceBBox(start, end, mins, maxs, liftFeet);
		}

		/// <summary>
		/// Try to keep a walking player on the ground when running down slopes etc
		/// </summary>
		public virtual void StayOnGround() {
			var start = Position + Vector3.Up * 2;
			var end = Position + Vector3.Down * StepSize;

			// See how far up we can go without getting stuck
			var trace = TraceBBox(Position, start);
			start = trace.EndPos;

			// Now trace down from a known safe position
			trace = TraceBBox(start, end);

			if(trace.Fraction <= 0) return;
			if(trace.Fraction >= 1) return;
			if(trace.StartedSolid) return;
			if(Vector3.GetAngle(Vector3.Up, trace.Normal) > GroundAngle) return;

			Position = trace.EndPos;
		}

		void RestoreGroundPos() {
			if(GroundEntity == null || GroundEntity.IsWorld)
				return;
		}

		void SaveGroundPos() {
			if(GroundEntity == null || GroundEntity.IsWorld)
				return;
		}
	}
}
