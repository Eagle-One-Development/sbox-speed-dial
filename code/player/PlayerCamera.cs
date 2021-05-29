using System.Net;
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

			var pawn = Local.Pawn;
			if(pawn == null) return;

			DebugOverlay.Text(pawn.EyePos, $"{pawn.EyePos.z}", Color.Green);

			var direction = Screen.GetDirection(new Vector2(Mouse.Position.x, Mouse.Position.y), 70, Rot, Screen.Size);
			var HitPosition = LinePlaneIntersectionWithHeight(Pos, direction, pawn.EyePos.z - 20);

			var hitposTraceDown = Trace.Ray(HitPosition, HitPosition + Vector3.Down * 100)
				.WorldAndEntities()
				.Size(10)
				.Ignore(pawn)
				.Run();

			var hitposTraceUp = Trace.Ray(HitPosition, HitPosition + Vector3.Up * 100)
				.WorldAndEntities()
				.Size(10)
				.Ignore(pawn)
				.Run();

			if(hitposTraceDown.Hit) {
				DebugOverlay.Line(HitPosition, hitposTraceDown.EndPos, Color.Cyan);
				DebugOverlay.Sphere(hitposTraceDown.EndPos, 2, Color.Cyan, false);
			}
			if(hitposTraceUp.Hit) {
				DebugOverlay.Line(HitPosition, hitposTraceUp.EndPos, Color.Red);
				DebugOverlay.Sphere(hitposTraceUp.EndPos, 2, Color.Red, false);
			}


			// if(hitposTrace.Hit && (int)(hitposTrace.EndPos - HitPosition).Length != 44) {
			//Log.Info($"HIGH/LOW GROUND {(hitposTrace.EndPos - HitPosition).Length}");
			// }

			var targetTrace = Trace.Ray(pawn.EyePos, HitPosition)
				.Size(25)
				.EntitiesOnly()
				.Ignore(pawn)
				.Run();

			Angles angles;

			// aim assist when pointing on a player
			if(targetTrace.Hit && targetTrace.Entity is SpeedDialPlayer player && (targetTrace.EndPos - HitPosition).Length <= 40) {
				if(SpeedDialGame.DebugEnabled) {
					DebugOverlay.ScreenText(new Vector2(300, 300), 5, Color.Green, $"HitPlayer {player} {(targetTrace.EndPos - HitPosition).Length}");
					DebugOverlay.Line(pawn.EyePos, targetTrace.Entity.EyePos - Vector3.Up * 20, Color.Red, 0, false);
				}
				angles = (player.EyePos - Vector3.Up * 20 - (pawn.EyePos - Vector3.Up * 20)).EulerAngles;
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


			// debug stuff for aim location
			if(SpeedDialGame.DebugEnabled) {
				var direction = Screen.GetDirection(new Vector2(Mouse.Position.x, Mouse.Position.y), 70, Rot, Screen.Size);
				var HitPosition = LinePlaneIntersectionWithHeight(Pos, direction, pawn.EyePos.z);
				// 
				DebugOverlay.ScreenText(new Vector2(300, 300), 2, Color.Green, $"Pos {Pos}");
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
