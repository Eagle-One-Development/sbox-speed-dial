using System;
using Sandbox;

namespace SpeedDial.Player {



	public partial class SpeedDialCamera : Camera {

		public virtual float CameraHeight => 400;
		public virtual float CameraAngle => 65;

		public Angles ang;
		public Angles tarAng;

		public bool CameraShift { get; set; }

		public override void BuildInput(InputBuilder input) {
			var client = Local.Pawn;

			if(client == null) {
				return;
			}

			if(input.Down(InputButton.Run)) {
				CameraShift = true;
				Log.Info($"run {Time.Delta}");
			} else {
				CameraShift = false;
			}

			Vector2 screenCenter = Screen.Size * (Vector2)client.Position.ToScreen();
			Vector3 mouseDir = screenCenter - Mouse.Position;
			var angles = new Vector3(mouseDir.y, mouseDir.x).EulerAngles;

			// analog input stuff

			// if ( (Math.Abs( input.AnalogLook.pitch ) + Math.Abs( input.AnalogLook.yaw )) > 0.0f )
			// {
			// 	if ( input.AnalogLook.Length > 0.25f){
			// 		Angles newDir = new Vector3( input.AnalogLook.pitch / 1.5f * -1.0f, input.AnalogLook.yaw / 1.5f, 0 ).EulerAngles;
			// 		tarAng.yaw = newDir.yaw;
			// 	}
			// }
			tarAng = angles;
			ang = Angles.Lerp(ang, tarAng, 8 * Time.Delta);

			input.ViewAngles = ang;
			input.InputDirection = input.AnalogMove;
		}

		public override void Update() {
			var pawn = Local.Pawn;

			if(pawn == null)
				return;

			//DebugOverlay.Sphere(pawn.Position, 5, Color.Green, false);


			Pos = pawn.EyePos; // relative to pawn eyepos
			Pos += Vector3.Up * CameraHeight; // add camera height
			Pos += -Vector3.Forward * (float)(CameraHeight / Math.Tan(MathX.DegreeToRadian(CameraAngle))); // move camera back
			if(CameraShift)
				Pos += Vector3.Left * -(Mouse.Position.x * 0.1f) + Vector3.Forward * -(Mouse.Position.y * 0.1f);

			//Pos = Vector3.Lerp(Pos, Pos + Vector3.Left * -(Mouse.Position.x * 0.1f) + Vector3.Forward * -(Mouse.Position.y * 0.1f), 8 * Time.Delta);

			DebugOverlay.ScreenText(new Vector2(500, 500), 1, Color.Green, CameraShift.ToString());
			DebugOverlay.ScreenText(new Vector2(500, 500), 2, Color.Green, Pos.ToString());

			Rot = Rotation.FromAxis(Vector3.Left, CameraAngle);

			FieldOfView = 70;

			Viewer = null;
		}
	}
}
