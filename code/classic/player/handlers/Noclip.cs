namespace SpeedDial.Classic.Player;

public partial class ClassicNoclipController : PawnController
{
	public override void Simulate()
	{
		var p = Pawn as BasePlayer;

		base.Simulate();
		var vel = new Vector3( p.InputForward, p.InputLeft, 0 );

		vel = vel.Normal * 4000;

		if ( Input.Down( InputButton.Duck ) )
			vel *= 0.2f;

		Velocity += vel * Time.Delta;

		Position += Velocity * Time.Delta;

		Velocity = Velocity.Approach( 0, Velocity.Length * Time.Delta * 10 );

		EyeRotation = p.InputRotation;
		WishVelocity = Velocity;
		GroundEntity = null;
		BaseVelocity = Vector3.Zero;

		SetTag( "noclip" );
	}
}
