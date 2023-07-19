namespace SpeedDial.Classic.Player;

public class ClassicAnimator : PawnAnimator
{
	public override void Simulate()
	{
		var p = Pawn as BasePlayer;

		var idealRotation = Rotation.LookAt( p.InputRotation.Forward.WithZ( 0 ), Vector3.Up );

		DoRotation( idealRotation );
		DoWalk( idealRotation );

		Vector3 aimPos = p.AimRay.Position + (p.InputRotation.Forward * 200);
		Vector3 lookPos = aimPos;

		// Look in the direction what the player's input is facing
		SetLookAt( "lookat_pos", lookPos );
		SetLookAt( "aimat_pos", aimPos );

		var pawn = Pawn as BasePlayer;
		if ( pawn.ActiveChild is BaseCarriable carry )
		{
			carry.SimulateAnimator( this );
		}
		else
		{
			SetAnimParameter( "holdtype", 0 );
			SetAnimParameter( "aimat_weight", 0.5f );
		}
	}

	public virtual void DoRotation( Rotation idealRotation )
	{
		float turnSpeed = 0.01f;
		// If we're moving, rotate to our ideal rotation
		Rotation = Rotation.Slerp( Rotation, idealRotation, WishVelocity.Length * Time.Delta * turnSpeed );

		// Clamp the foot rotation to within 120 degrees of the ideal rotation
		Rotation = Rotation.Clamp( idealRotation, 0, out var change );
	}

	private void DoWalk( Rotation idealRotation )
	{
		//
		// These tweak the animation speeds to something we feel is right,
		// so the foot speed matches the floor speed. Your art should probably
		// do this - but that ain't how we roll
		//
		SetAnimParameter( "walkspeed_scale", 2.0f / 300.0f );
		var groundspeed = Velocity.WithZ( 0 ).Length;
		SetAnimParameter( "move_groundspeed", groundspeed );

		//
		// Work out our movement relative to our body rotation
		//
		var dir = Velocity;
		var forward = 300 * idealRotation.Forward.Dot( dir.Normal );
		var sideward = 300 * idealRotation.Right.Dot( dir.Normal );
		var wishDir = WishVelocity;

		//
		// Set our speeds on the animgraph
		//
		var speedScale = Pawn.Scale;
		SetAnimParameter( "velocity", dir.Length );
		SetAnimParameter( "forward", forward );
		SetAnimParameter( "sideward", sideward );
		SetAnimParameter( "wishspeed", speedScale > 0.0f ? WishVelocity.Length / speedScale : 0.0f );
	}
}
