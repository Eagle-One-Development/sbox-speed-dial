using System;
using Sandbox;

namespace SpeedDial.Player
{
	public partial class SpeedDialCamera : Camera
	{

		public virtual float CameraHeight => 400;

		public virtual float CameraAngle => 75;

		public override void Update()
		{
			var pawn = Local.Pawn;

			if ( pawn == null )
				return;

			//DebugOverlay.Sphere(pawn.Position, 5, Color.Green, false);

			Pos = pawn.Position + (Vector3.Up * CameraHeight) - ((Vector3.Forward * (-(float)(CameraHeight * Math.Tan( CameraAngle )))) / 2f);

			Rot = Rotation.FromAxis( Vector3.Left, CameraAngle );

			FieldOfView = 70;

			Viewer = null;
		}
	}
}
