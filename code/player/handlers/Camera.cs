using System.Net;
using System;
using Sandbox;
using SpeedDial.Weapons;

namespace SpeedDial.Player {
	public partial class SpeedDialCamera : Camera {

		[ClientVar("sd_viewshift_toggle")]
		public static bool ViewshiftToggle { get; set; } = false;
		private bool shiftToggle = false;

		public virtual float CameraHeight => 400;
		public virtual float CameraAngle => 65;

		private Angles ang;
		private Angles tarAng;
		private Vector3 camOffset;
		private Vector3 camOffsetTarget;

		public SpeedDialCamera() {
			Event.Register(this);
		}


		public bool CameraShift { get; set; }

		[Net, Local, Predicted]
		public bool Freeze { get; set; } = false;

		public override void BuildInput(InputBuilder input) {
			if(Freeze) return;
			var client = Local.Pawn;

			if(client == null) {
				return;
			}

			if(!ViewshiftToggle && input.Down(InputButton.Run)) {
				CameraShift = true;
			} else if(ViewshiftToggle && input.Pressed(InputButton.Run)) {
				shiftToggle = !shiftToggle;
			} else {
				CameraShift = false;
			}

			var pawn = Local.Pawn;
			if(pawn == null) return;

			var direction = Screen.GetDirection(new Vector2(Mouse.Position.x, Mouse.Position.y), 70, Rotation, Screen.Size);
			var HitPosition = LinePlaneIntersectionWithHeight(Position, direction, pawn.EyePos.z - 20);

			//trace from camera into mouse direction, essentially gets the world location of the mouse
			var targetTrace = Trace.Ray(Position, Position + direction * 1000)
				.UseHitboxes()
				.EntitiesOnly()
				.Size(1)
				.Ignore(pawn)
				.Run();

			Angles angles;

			// aim assist when pointing on a player
			if(targetTrace.Hit && targetTrace.Entity is SpeedDialPlayer) {
				Debug.Line(pawn.EyePos, targetTrace.Entity.EyePos + Vector3.Down * 20, Color.Red, 0, true);
				angles = (targetTrace.Entity.EyePos + Vector3.Down * 20 - (pawn.EyePos - Vector3.Up * 20)).EulerAngles;
			} else {
				angles = (HitPosition - (pawn.EyePos - Vector3.Up * 20)).EulerAngles;
			}

			// analog input stuff for later maybe

			// if ( (Math.Abs( input.AnalogLook.pitch ) + Math.Abs( input.AnalogLook.yaw )) > 0.0f )
			// {
			// 	if ( input.AnalogLook.Length > 0.25f){
			// 		Angles newDir = new Vector3( input.AnalogLook.pitch / 1.5f * -1.0f, input.AnalogLook.yaw / 1.5f, 0 ).EulerAngles;
			// 		tarAng.yaw = newDir.yaw;
			// 	}
			// }

			tarAng = angles;
			ang = Angles.Lerp(ang, tarAng, 12 * Time.Delta);

			input.ViewAngles = ang;
			input.InputDirection = input.AnalogMove;
		}

		[Event("SDEvents.Settings.Changed")]
		private void onSettingChange() {
			if(Host.IsClient && Settings.SettingsManager.GetSetting("Viewshift Toggle").TryGetBool(out bool? res))
				ViewshiftToggle = res.Value;
		}

		public override void Update() {
			if(Freeze) return;
			var pawn = Local.Pawn;

			if(pawn == null)
				return;

			Position = pawn.EyePos + Vector3.Down * 20; // relative to pawn eyepos
			Position += Vector3.Up * CameraHeight; // add camera height
			Position += -Vector3.Forward * (float)(CameraHeight / Math.Tan(MathX.DegreeToRadian(CameraAngle))); // move camera back

			float mouseShiftFactor = 0.3f;//Sniper
			if(pawn.ActiveChild is Sniper) {
				mouseShiftFactor = 0.5f;
			}

			float MouseX = Mouse.Position.x.Clamp(0, Screen.Size.x);
			float MouseY = Mouse.Position.y.Clamp(0, Screen.Size.y);

			if(CameraShift || ViewshiftToggle && shiftToggle) {
				camOffsetTarget = Vector3.Left * -((MouseX - Screen.Size.x / 2) * mouseShiftFactor) + Vector3.Forward * -((MouseY - Screen.Size.y / 2) * mouseShiftFactor);
			} else {
				camOffsetTarget = Vector3.Zero;
			}
			camOffset = Vector3.Lerp(camOffset, camOffsetTarget, Time.Delta * 8f);
			Position += camOffset;

			Rotation = Rotation.FromAxis(Vector3.Left, CameraAngle);


			// debug stuff for aim location
			if(Debug.Enabled) {
				var direction = Screen.GetDirection(new Vector2(Mouse.Position.x, Mouse.Position.y), 70, Rot, Screen.Size);
				var HitPosition = LinePlaneIntersectionWithHeight(Position, direction, pawn.EyePos.z);
				// 
				DebugOverlay.ScreenText(new Vector2(300, 300), 2, Color.Green, $"Pos {Position}");
				DebugOverlay.ScreenText(new Vector2(300, 300), 3, Color.Green, $"Dir {direction}");
				DebugOverlay.ScreenText(new Vector2(300, 300), 4, Color.Green, $"HitPos {HitPosition}");
				// 
				var Distance = HitPosition - pawn.EyePos;
				// 
				DebugOverlay.Line(pawn.EyePos, pawn.EyePos + Distance, Color.Green, 0, false);

				// TEMP CROSSHAIR
				DebugOverlay.Sphere(HitPosition, 5, Color.Green, false);
			}

			FieldOfView = 70;
			Viewer = null;
		}

		// THIS SHALL NOT BE TOUCHED FOR AS LONG AS IT WORKS, UNDERSTOOD?!?!?
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
