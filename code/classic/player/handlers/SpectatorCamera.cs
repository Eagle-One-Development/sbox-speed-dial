using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Player;

public partial class OneChamberSpectatorCamera : CameraMode {

	public virtual float CameraHeight => 400;
	public virtual float CameraAngle => 65;

	private Angles ang;
	private Angles tarAng;

	public override void BuildInput(InputBuilder input) {
		var pawn = Local.Pawn;

		if(pawn == null) {
			return;
		}

		Angles angles;

		// always set movement input
		input.InputDirection = input.AnalogMove;

		// handle look input
		if(!input.UsingController) {
			var direction = Screen.GetDirection(new Vector2(Mouse.Position.x, Mouse.Position.y), 70, Rotation, Screen.Size);
			var HitPosition = LinePlaneIntersectionWithHeight(Position, direction, pawn.EyePosition.z - 20);

			// since we got our cursor in world space because of the plane intersect above, we need to set it for the crosshair
			var mouse = HitPosition.ToScreen();
			Crosshair.UpdateMouse(new Vector2(mouse.x * Screen.Width, mouse.y * Screen.Height));

			//trace from camera into mouse direction, essentially gets the world location of the mouse
			var targetTrace = Trace.Ray(Position, Position + direction * 1000)
				.UseHitboxes()
				.EntitiesOnly()
				.Size(1)
				.Ignore(pawn)
				.Run();

			// aim assist when pointing on a player
			if(targetTrace.Hit && targetTrace.Entity is ClassicPlayer) {
				if(Debug.Camera)
					DebugOverlay.Line(pawn.EyePosition, targetTrace.Entity.EyePosition + Vector3.Down * 20, Color.Red, 0, true);
				angles = (targetTrace.Entity.EyePosition + Vector3.Down * 20 - (pawn.EyePosition - Vector3.Up * 20)).EulerAngles;
			} else {
				angles = (HitPosition - (pawn.EyePosition - Vector3.Up * 20)).EulerAngles;
			}

		} else {
			if(MathF.Abs(input.AnalogLook.pitch) + MathF.Abs(input.AnalogLook.yaw) > 0) {
				//var angle = MathF.Atan2(input.AnalogLook.pitch, input.AnalogLook.yaw).RadianToDegree();
				Angles newDir = new Vector3(input.AnalogLook.pitch / 1.5f * -1.0f, input.AnalogLook.yaw / 1.5f, 0).EulerAngles;
				angles = newDir;
			} else {
				// not moving joystick, don't update angles
				return;
			}
		}

		tarAng = angles;
		ang = Angles.Lerp(ang, tarAng, 24 * Time.Delta);

		input.ViewAngles = ang;
	}

	public override void Update() {
		var pawn = Local.Pawn;

		if(pawn == null)
			return;

		var _pos = pawn.EyePosition + Vector3.Down * 20; // relative to pawn EyePosition
		_pos += Vector3.Up * CameraHeight; // add camera height
										   // why didn't we just do this with Rotation.LookAt????
										   // [DOC] answer: cause we (I) wanted a fixed/clearly defined angle
		_pos -= Vector3.Forward * (float)(CameraHeight / Math.Tan(MathX.DegreeToRadian(CameraAngle))); // move camera back

		Position = _pos;

		Rotation = Rotation.FromAxis(Vector3.Left, CameraAngle);


		// debug stuff for aim location
		if(Debug.Camera) {
			var direction = Screen.GetDirection(new Vector2(Mouse.Position.x, Mouse.Position.y), 70, Rotation, Screen.Size);
			var HitPosition = LinePlaneIntersectionWithHeight(Position, direction, pawn.EyePosition.z);
			// 
			DebugOverlay.ScreenText(new Vector2(300, 300), 2, Color.Green, $"Pos {Position}");
			DebugOverlay.ScreenText(new Vector2(300, 300), 3, Color.Green, $"Dir {direction}");
			DebugOverlay.ScreenText(new Vector2(300, 300), 4, Color.Green, $"HitPos {HitPosition}");
			// 
			var Distance = HitPosition - pawn.EyePosition;
			// 
			DebugOverlay.Line(pawn.EyePosition, pawn.EyePosition + Distance, Color.Green, 0, false);

			// TEMP CROSSHAIR
			DebugOverlay.Sphere(HitPosition, 5, Color.Green, false);
		}

		FieldOfView = 70;
		Viewer = null;
	}

	// resolve line plane intersect for mouse input
	public static Vector3 LinePlaneIntersectionWithHeight(Vector3 pos, Vector3 dir, float z) {
		float px, py, pz;

		//solve for temp, zpos = (zdir) * (temp) + (initialZpos)
		float temp = (z - pos.z) / dir.z;

		//plug in and solve for Px and Py
		px = dir.x * temp + pos.x;
		py = dir.y * temp + pos.y;
		pz = z;
		return new Vector3(px, py, pz);
	}
}
