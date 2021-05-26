using System;
using Sandbox;

namespace SpeedDial.Player {



	public partial class SpeedDialCamera : Camera {

		public virtual float CameraHeight => 400;
		public virtual float CameraAngle => 65;

		private Angles ang;
		private Angles tarAng;
		private Vector3 camOffset;
		private Vector3 camOffsetTarget;


		public bool CameraShift { get; set; }

		public override void BuildInput(InputBuilder input) {
			var client = Local.Pawn;

			if(client == null) {
				return;
			}

			if(input.Down(InputButton.Run)) {
				CameraShift = true;
			} else {
				CameraShift = false;
			}

			Vector2 screenCenter = Screen.Size * (Vector2)client.Position.ToScreen();
			Vector3 mouseDir = screenCenter - Mouse.Position;

			var pawn = Local.Pawn;
			if(pawn == null) return;

			var direction = Screen.GetDirection(new Vector2(Mouse.Position.x, Mouse.Position.y), 70, Rot, Screen.Size);
			var HitPosition = LinePlaneIntersectionWithHeight(Pos, direction, pawn.EyePos.z);
			var angles = (HitPosition - pawn.EyePos).EulerAngles;

			// analog input stuff

			// if ( (Math.Abs( input.AnalogLook.pitch ) + Math.Abs( input.AnalogLook.yaw )) > 0.0f )
			// {
			// 	if ( input.AnalogLook.Length > 0.25f){
			// 		Angles newDir = new Vector3( input.AnalogLook.pitch / 1.5f * -1.0f, input.AnalogLook.yaw / 1.5f, 0 ).EulerAngles;
			// 		tarAng.yaw = newDir.yaw;
			// 	}
			// }
			tarAng = angles;
			ang = Angles.Lerp(ang, tarAng, 10 * Time.Delta);

			input.ViewAngles = ang;
			input.InputDirection = input.AnalogMove;
		}

		public override void Update() {
			var pawn = Local.Pawn;

			if(pawn == null)
				return;

			Pos = pawn.EyePos; // relative to pawn eyepos
			Pos += Vector3.Up * CameraHeight; // add camera height
			Pos += -Vector3.Forward * (float)(CameraHeight / Math.Tan(MathX.DegreeToRadian(CameraAngle))); // move camera back

			if(CameraShift) {
				camOffsetTarget = Vector3.Left * -((Mouse.Position.x - Screen.Size.x / 2) * 0.3f) + Vector3.Forward * -((Mouse.Position.y - Screen.Size.y / 2) * 0.3f);
			} else {
				camOffsetTarget = Vector3.Zero;
			}
			camOffset = Vector3.Lerp(camOffset, camOffsetTarget, Time.Delta * 8f);
			Pos += camOffset;

			Rot = Rotation.FromAxis(Vector3.Left, CameraAngle);

			var direction = Screen.GetDirection(new Vector2(Mouse.Position.x, Mouse.Position.y), 70, Rot, Screen.Size);

			var HitPosition = LinePlaneIntersectionWithHeight(Pos, direction, pawn.EyePos.z);

			DebugOverlay.ScreenText(new Vector2(300, 300), 2, Color.Green, $"Pos {Pos}");
			DebugOverlay.ScreenText(new Vector2(300, 300), 3, Color.Green, $"Dir {direction}");
			DebugOverlay.ScreenText(new Vector2(300, 300), 4, Color.Green, $"HitPos {HitPosition}");

			var Distance = HitPosition - pawn.EyePos;
			//vectDistance = B - A
			//vectDirection = vectDistance / lenght(vectDistance)

			DebugOverlay.Line(pawn.EyePos, pawn.EyePos + Distance, Color.Green, 0, false);



			DebugOverlay.Sphere(HitPosition, 5, Color.Green, false);
			//DebugOverlay.Line(pawn.EyePos, HitPosition, Color.Green, 0, false);

			FieldOfView = 70;
			Viewer = null;
		}

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
}
