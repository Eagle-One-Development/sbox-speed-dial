using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Player;

public partial class OneChamberSpectatorCamera : CameraMode
{

	public virtual float CameraHeight => 400;
	public virtual float CameraAngle => 65;

	private Angles ang;
	private Angles tarAng;

	public override void BuildInput()
	{
		var pawn = Game.LocalPawn;

		if ( pawn == null || pawn is not BasePlayer player )
		{
			return;
		}

		Angles angles;

		// handle look input
		if ( !Input.UsingController )
		{
			var direction = Screen.GetDirection( new Vector2( Mouse.Position.x, Mouse.Position.y ), 70, Rotation, Screen.Size );
			var HitPosition = LinePlaneIntersectionWithHeight( Position, direction, player.EyePosition.z - 20 );

			// since we got our cursor in world space because of the plane intersect above, we need to set it for the crosshair
			var mouse = HitPosition.ToScreen();
			Crosshair.UpdateMouse( new Vector2( mouse.x * Screen.Width, mouse.y * Screen.Height ) );

			//trace from camera into mouse direction, essentially gets the world location of the mouse
			var targetTrace = Trace.Ray( Position, Position + (direction * 1000) )
				.UseHitboxes()
				.EntitiesOnly()
				.Size( 1 )
				.Ignore( player )
				.Run();

			// aim assist when pointing on a player
			if ( targetTrace.Hit && targetTrace.Entity is ClassicPlayer )
			{
				if ( Debug.Camera )
					DebugOverlay.Line( player.EyePosition, targetTrace.Entity.AimRay.Position + (Vector3.Down * 20), Color.Red, 0, true );
				angles = (targetTrace.Entity.AimRay.Position + (Vector3.Down * 20) - (player.EyePosition - (Vector3.Up * 20))).EulerAngles;
			}
			else
			{
				angles = (HitPosition - (player.EyePosition - (Vector3.Up * 20))).EulerAngles;
			}

		}
		else
		{
			if ( MathF.Abs( Input.AnalogLook.pitch ) + MathF.Abs( Input.AnalogLook.yaw ) > 0 )
			{
				//var angle = MathF.Atan2(input.AnalogLook.pitch, input.AnalogLook.yaw).RadianToDegree();
				Angles newDir = new Vector3( Input.AnalogLook.pitch / 1.5f * -1.0f, Input.AnalogLook.yaw / 1.5f, 0 ).EulerAngles;
				angles = newDir;
			}
			else
			{
				// not moving joystick, don't update angles
				return;
			}
		}

		tarAng = angles;
		ang = Angles.Lerp( ang, tarAng, 24 * Time.Delta );

		player.InputViewAngles = ang;
	}

	public override void Update()
	{
		var pawn = Game.LocalPawn;

		if ( pawn == null || pawn is not BasePlayer player )
			return;

		var _pos = player.EyePosition + (Vector3.Down * 20); // relative to pawn EyePosition
		_pos += Vector3.Up * CameraHeight; // add camera height
										   // why didn't we just do this with Rotation.LookAt????
										   // [DOC] answer: cause we (I) wanted a fixed/clearly defined angle
		_pos -= Vector3.Forward * (float)(CameraHeight / Math.Tan( MathX.DegreeToRadian( CameraAngle ) )); // move camera back

		Position = _pos;

		Rotation = Rotation.FromAxis( Vector3.Left, CameraAngle );


		// debug stuff for aim location
		if ( Debug.Camera )
		{
			var direction = Screen.GetDirection( new Vector2( Mouse.Position.x, Mouse.Position.y ), 70, Rotation, Screen.Size );
			var HitPosition = LinePlaneIntersectionWithHeight( Position, direction, player.EyePosition.z );
			// 
			DebugOverlay.ScreenText( $"Pos {Position}", new Vector2( 300, 300 ), 2, Color.Green );
			DebugOverlay.ScreenText( $"Dir {direction}", new Vector2( 300, 300 ), 3, Color.Green );
			DebugOverlay.ScreenText( $"HitPos {HitPosition}", new Vector2( 300, 300 ), 4, Color.Green );
			// 
			var Distance = HitPosition - player.EyePosition;
			// 
			DebugOverlay.Line( player.EyePosition, player.EyePosition + Distance, Color.Green, 0, false );

			// TEMP CROSSHAIR
			DebugOverlay.Sphere( HitPosition, 5, Color.Green, Time.Delta, false );
		}

		FieldOfView = Screen.CreateVerticalFieldOfView( 70 );
		Viewer = null;
	}

	// resolve line plane intersect for mouse input
	public static Vector3 LinePlaneIntersectionWithHeight( Vector3 pos, Vector3 dir, float z )
	{
		float px, py, pz;

		//solve for temp, zpos = (zdir) * (temp) + (initialZpos)
		float temp = (z - pos.z) / dir.z;

		//plug in and solve for Px and Py
		px = (dir.x * temp) + pos.x;
		py = (dir.y * temp) + pos.y;
		pz = z;
		return new Vector3( px, py, pz );
	}
}
