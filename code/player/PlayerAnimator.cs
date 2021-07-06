using System.Reflection.Emit;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpeedDial.Weapons;

namespace SpeedDial.Player {
	public class PlayerAnimator : PawnAnimator {
		TimeSince TimeSinceFootShuffle = 60;

		public override void Simulate() {
			var idealRotation = Rotation.LookAt(Input.Rotation.Forward.WithZ(0), Vector3.Up);

			DoRotation(idealRotation);
			DoWalk(idealRotation);

			//
			// Let the animation graph know some shit
			//
			// bool noclip = HasTag("noclip");

			// SetParam("b_grounded", GroundEntity != null || noclip);
			// SetParam("b_noclip", noclip);

			Vector3 aimPos = Pawn.EyePos + Input.Rotation.Forward * 200;
			Vector3 lookPos = aimPos;

			//
			// Look in the direction what the player's input is facing
			//
			SetLookAt("lookat_pos", lookPos);
			SetLookAt("aimat_pos", aimPos);


			if(Pawn.ActiveChild is BaseCarriable carry) {
				carry.SimulateAnimator(this);
			} else {
				SetParam("holdtype", 5);
				SetParam("aimat_weight", 0.5f);
			}

		}

		public virtual void DoRotation(Rotation idealRotation) {

			float turnSpeed = 0.01f;

			//
			// If we're moving, rotate to our ideal rotation
			//
			Rotation = Rotation.Slerp(Rotation, idealRotation, WishVelocity.Length * Time.Delta * turnSpeed);

			//
			// Clamp the foot rotation to within 120 degrees of the ideal rotation
			//
			Rotation = Rotation.Clamp(idealRotation, 0, out var change);

			//
			// If we did restrict, and are standing still, add a foot shuffle
			//
			if(change > 1 && WishVelocity.Length <= 1) TimeSinceFootShuffle = 0;

			// SetParam("b_shuffle", TimeSinceFootShuffle < 0.1);
		}

		void DoWalk(Rotation idealRotation) {
			//
			// These tweak the animation speeds to something we feel is right,
			// so the foot speed matches the floor speed. Your art should probably
			// do this - but that ain't how we roll
			//
			SetParam("walkspeed_scale", 2.0f / 300.0f);
			// SetParam("runspeed_scale", 2.0f / 300.0f);
			// SetParam("duckspeed_scale", 2.0f / 300.0f);

			//
			// Work out our movement relative to our body rotation
			//
			var moveDir = WishVelocity;
			var forward = idealRotation.Forward.Dot(moveDir.Normal);
			var sideward = idealRotation.Right.Dot(moveDir.Normal);

			//
			// Set our speeds on the animgraph
			//
			var speedScale = Pawn.Scale;

			SetParam("forward", forward);
			SetParam("sideward", sideward);
			SetParam("wishspeed", speedScale > 0.0f ? WishVelocity.Length / speedScale : 0.0f);
		}
	}
}
