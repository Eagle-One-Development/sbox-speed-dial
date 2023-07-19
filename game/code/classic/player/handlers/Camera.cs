using SpeedDial.Classic.UI;
using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Player;

public partial class ClassicCamera : CameraMode
{

	private bool shiftToggle = false;

	public virtual float CameraHeight => 400;
	public virtual float CameraAngle => 65;

	private Angles ang;
	private Angles tarAng;
	private Vector3 camOffset;
	private Vector3 camOffsetTarget;

	public bool CameraShift { get; set; }

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

			if ( pawn.Alive() )
			{
				if ( !Settings.ViewshiftToggle && Input.Down( "Run" ) )
				{
					CameraShift = true;
				}
				else if ( Settings.ViewshiftToggle && Input.Pressed( "Run" ) )
				{
					shiftToggle = !shiftToggle;
				}
				else
				{
					CameraShift = false;
				}
			}
			else
			{
				CameraShift = false;
			}

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
			// shift on clicking in joystick
			if ( !Settings.ViewshiftToggle && Input.Down( "View" ) )
			{
				CameraShift = true;
			}
			else if ( Settings.ViewshiftToggle && Input.Pressed( "View" ) )
			{
				shiftToggle = !shiftToggle;
			}
			else
			{
				CameraShift = false;
			}

			if ( MathF.Abs( Input.AnalogLook.pitch ) + MathF.Abs( Input.AnalogLook.yaw ) > 0 )
			{
				//var angle = MathF.Atan2(input.AnalogLook.pitch, input.AnalogLook.yaw).RadianToDegree();
				Angles newDir = new Vector3( Input.AnalogLook.pitch / 1.5f * -1.0f, Input.AnalogLook.yaw / 1.5f, 0 ).EulerAngles;
				ControllerLookInput = new Vector2( Input.AnalogLook.yaw, -Input.AnalogLook.pitch ).Normal;
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

	private Vector2 ControllerLookInput { get; set; } = Vector2.Zero;

	public override void Update()
	{
		if ( Game.LocalPawn is not BasePlayer pawn )
			return;

		var _pos = pawn.EyePosition + (Vector3.Down * 20); // relative to pawn EyePosition
		_pos += Vector3.Up * CameraHeight; // add camera height
										   // why didn't we just do this with Rotation.LookAt????
										   // [DOC] answer: cause we (I) wanted a fixed/clearly defined angle
		_pos -= Vector3.Forward * (float)(CameraHeight / Math.Tan( MathX.DegreeToRadian( CameraAngle ) )); // move camera back

		float mouseShiftFactor = 0.3f;//Sniper
		var wep = pawn.ActiveChild as Weapon;
		if ( wep is not null && wep.Blueprint.Scoped )
		{
			mouseShiftFactor = 0.5f;
		}

		float MouseX = Mouse.Position.x.Clamp( 0, Screen.Size.x );
		float MouseY = Mouse.Position.y.Clamp( 0, Screen.Size.y );

		camOffsetTarget = CameraShift || (Settings.ViewshiftToggle && shiftToggle)
			? Input.UsingController
				? (Vector3.Left * (ControllerLookInput.x * Screen.Size.x / 2) * mouseShiftFactor) + (Vector3.Forward * (ControllerLookInput.y * Screen.Size.y / 2) * mouseShiftFactor)
				: (Vector3.Left * -((MouseX - (Screen.Size.x / 2)) * mouseShiftFactor)) + (Vector3.Forward * -((MouseY - (Screen.Size.y / 2)) * mouseShiftFactor))
			: Vector3.Zero;
		camOffset = Vector3.Lerp( camOffset, camOffsetTarget, Time.Delta * 8f );

		_pos += camOffset;

		Position = _pos;

		Rotation = Rotation.FromAxis( Vector3.Left, CameraAngle );

		Sound.Listener = new()
		{
			Position = pawn.IsValid() ? pawn.EyePosition : Position,
			Rotation = Rotation
		};


		// debug stuff for aim location
		if ( Debug.Camera )
		{
			var direction = Screen.GetDirection( new Vector2( Mouse.Position.x, Mouse.Position.y ), 70, Rotation, Screen.Size );
			var HitPosition = LinePlaneIntersectionWithHeight( Position, direction, pawn.EyePosition.z );
			// 
			DebugOverlay.ScreenText( $"Pos {Position}", new Vector2( 300, 300 ), 2, Color.Green );
			DebugOverlay.ScreenText( $"Dir {direction}", new Vector2( 300, 300 ), 3, Color.Green );
			DebugOverlay.ScreenText( $"HitPos {HitPosition}", new Vector2( 300, 300 ), 4, Color.Green );
			// 
			var Distance = HitPosition - pawn.EyePosition;
			// 
			DebugOverlay.Line( pawn.EyePosition, pawn.EyePosition + Distance, Color.Green, 0, false );

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