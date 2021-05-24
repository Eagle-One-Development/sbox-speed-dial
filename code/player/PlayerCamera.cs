using System;
using Sandbox;

namespace SpeedDial.Player {



	public partial class SpeedDialCamera : Camera {

		public virtual float CameraHeight => 400;

		public virtual float CameraAngle => 75;

		public Angles ang;
		public Angles tarAng;

		public override void BuildInput(InputBuilder input) {
			var client = Local.Pawn;

			if(client == null) {
				return;
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
			//ang = Angles.Lerp(ang, tarAng, 10 * Time.Delta);

			input.ViewAngles = angles;
			input.InputDirection = input.AnalogMove;

		}

		public override void Update() {
			var pawn = Local.Pawn;

			if(pawn == null)
				return;

			//DebugOverlay.Sphere(pawn.Position, 5, Color.Green, false);

			Pos = pawn.Position + (Vector3.Up * CameraHeight) - Vector3.Forward * (float)(CameraHeight / Math.Tan(MathX.DegreeToRadian(CameraAngle)));


			Rot = Rotation.FromAxis(Vector3.Left, CameraAngle);

			FieldOfView = 70;

			Viewer = null;
		}
	}
}
