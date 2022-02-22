namespace SpeedDial.Classic.Player;

public class ClassicAnimator : PawnAnimator {
	public override void Simulate() {
		var idealRotation = Rotation.LookAt(Input.Rotation.Forward.WithZ(0), Vector3.Up);

		DoRotation(idealRotation);
		DoWalk(idealRotation);

		Vector3 aimPos = Pawn.EyePosition + Input.Rotation.Forward * 200;
		Vector3 lookPos = aimPos;

		// Look in the direction what the player's input is facing
		SetLookAt("lookat_pos", lookPos);
		SetLookAt("aimat_pos", aimPos);

		if(Pawn.ActiveChild is BaseCarriable carry) {
			carry.SimulateAnimator(this);
		} else {
			SetParam("holdtype", 0);
			SetParam("aimat_weight", 0.5f);
		}
	}

	public virtual void DoRotation(Rotation idealRotation) {
		float turnSpeed = 0.01f;
		// If we're moving, rotate to our ideal rotation
		Rotation = Rotation.Slerp(Rotation, idealRotation, WishVelocity.Length * Time.Delta * turnSpeed);

		// Clamp the foot rotation to within 120 degrees of the ideal rotation
		Rotation = Rotation.Clamp(idealRotation, 0, out var change);
	}

	void DoWalk(Rotation idealRotation) {
		//
		// These tweak the animation speeds to something we feel is right,
		// so the foot speed matches the floor speed. Your art should probably
		// do this - but that ain't how we roll
		//
		SetParam("walkspeed_scale", 2.0f / 300.0f);
		var groundspeed = Velocity.WithZ(0).Length;
		SetParam("move_groundspeed", groundspeed);

		//
		// Work out our movement relative to our body rotation
		//
		var dir = Velocity;
		var forward = 300 * idealRotation.Forward.Dot(dir.Normal);
		var sideward = 300 * idealRotation.Right.Dot(dir.Normal);
		var wishDir = WishVelocity;

		//
		// Set our speeds on the animgraph
		//
		var speedScale = Pawn.Scale;
		SetParam("velocity", dir);
		SetParam("forward", forward);
		SetParam("sideward", sideward);
		SetParam("wishspeed", speedScale > 0.0f ? WishVelocity.Length / speedScale : 0.0f);
	}
}
