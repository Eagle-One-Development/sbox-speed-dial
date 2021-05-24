using System;
using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialCamera : Camera {

		private readonly float CameraHeight = 200;

		private readonly float CameraAngle = 60;

		public override void Update() {
			var pawn = Local.Pawn;

			if(pawn == null)
				return;

			Pos = pawn.Position + Vector3.Up * CameraHeight + (Vector3.Forward * -(float)(CameraHeight * Math.Tan(CameraAngle)));

			Rot = Rotation.FromAxis(Vector3.Left, CameraAngle);

			FieldOfView = 90;

			Viewer = null;
		}
	}
}
