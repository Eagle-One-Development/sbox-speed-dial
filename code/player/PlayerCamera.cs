using System;
using Sandbox;

namespace SpeedDial.Player {
	public partial class SpeedDialCamera : Camera {

		public virtual float CameraHeight => 350;

		public virtual float CameraAngle => 60;

		public virtual float CameraForwardOffset => 48; //my trigonometry might be fucked, but right now this makes it work nicely

		public override void Update() {
			var pawn = Local.Pawn;

			if(pawn == null)
				return;

			//DebugOverlay.Sphere(pawn.Position, 5, Color.Green, false);

			Pos = pawn.Position + Vector3.Up * CameraHeight + (Vector3.Forward * -(float)(CameraHeight * Math.Tan(CameraAngle) + CameraForwardOffset));

			Rot = Rotation.FromAxis(Vector3.Left, CameraAngle);

			FieldOfView = 90;

			Viewer = null;
		}
	}
}
